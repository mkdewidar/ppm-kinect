using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Kinect;
using Microsoft.Kinect.Face;

namespace KinectFace
{
    /// <summary>
    /// <para>Kinect Class that allows use of HD Face features.</para>
    /// <para>Constructor requires a Kinect sensor and canvas for drawing.</para>
    /// </summary>
    public class KinectHDFace
    {
        // Kinect Sensor class - method is used later to detect sensor.
        private KinectSensor _kinectSensor = null;

        // WPF Canvas to draw to; this is so that we can display colour frames.
        Canvas _kinectDrawCanvas = null;

        // Multi-source frame reader to allow colour image behind vertices
        private MultiSourceFrameReader _multiReader = null;

        // Collects the body frame data from the kinect sensor.
        private BodyFrameSource _bodyFrameSource = null;

        // Reads the body frame data from the source.
        private BodyFrameReader _bodyFrameReader = null;

        // Collects the HD Face frames - the body is required for this.
        private HighDefinitionFaceFrameSource _faceSource = null;

        // Reads HD Face data from the source.
        private HighDefinitionFaceFrameReader _faceReader = null;

        // This is used to create a set of vertices which are aligned with the face in real-time.
        private FaceAlignment _faceAlignment = null;

        // Allows access to the creates 3D mesh of the face (set of vertices)
        private FaceModel _faceModel = null;

        // List of vertices which are aligned with the face
        private List<Ellipse> _faceVertices = new List<Ellipse>();

        public KinectHDFace(KinectSensor sensor, Canvas drawingCanvas)
        {
            // Assigning drawing canvas for colour frames
            _kinectDrawCanvas = drawingCanvas;

            // Assign the default Kinect Sensor; must be connected and open
            _kinectSensor = sensor;

            // If assignment was success and sensor is available
            if (_kinectSensor != null)
            {
                // find a multi-source frame reader and fire _DrawColourFrame when frames arrive
                _multiReader = _kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color);
                _multiReader.MultiSourceFrameArrived += _DrawColourFrame;

                _bodyFrameSource = _kinectSensor.BodyFrameSource;
                _bodyFrameReader = _bodyFrameSource.OpenReader();

                // When a body frame arrives, call FrameHandler() using listener
                _bodyFrameReader.FrameArrived += _BodyFrameHandler;

                // Now handle HD Face components
                _faceSource = new HighDefinitionFaceFrameSource(_kinectSensor);
                _faceReader = _faceSource.OpenReader();

                // When a face frame arrives, call FaceFrameHandler() using listener
                _faceReader.FrameArrived += _FaceFrameHandler;


                // Initialise face model and alignment for vertices
                _faceModel = new FaceModel();
                _faceAlignment = new FaceAlignment();
            }
        }

        private void _DrawColourFrame(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            // Currently still multiple sources, need to acquire frame and take colour
            using (var colourFrame = e.FrameReference.AcquireFrame().ColorFrameReference.AcquireFrame())
            {
                if (colourFrame != null)
                {
                    // The below algorithm is from the internet
                    // I have just found ways to take colourFrame width/height and convert it to necessary types
                    int width = colourFrame.FrameDescription.Width;
                    int height = colourFrame.FrameDescription.Height;

                    byte[] pixels = new byte[width * height * ((PixelFormats.Bgr32.BitsPerPixel + 7) / 8)];

                    if (colourFrame.RawColorImageFormat == ColorImageFormat.Bgra)
                    {
                        colourFrame.CopyRawFrameDataToArray(pixels);
                    }
                    else
                    {
                        colourFrame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
                    }

                    int stride = width * PixelFormats.Bgra32.BitsPerPixel / 8;

                    ImageBrush canvasBackgroundBrush = new ImageBrush();
                    canvasBackgroundBrush.ImageSource = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);

                    _kinectDrawCanvas.Background = canvasBackgroundBrush;
                }
            }
        }

        private void _BodyFrameHandler(object sender, BodyFrameArrivedEventArgs e)
        {
            BodyFrame bodyFrame = e.FrameReference.AcquireFrame();

            if (bodyFrame != null)
            {
                // Create list of bodies in frame, load bodies into it
                Body[] bodies = new Body[bodyFrame.BodyCount];
                bodyFrame.GetAndRefreshBodyData(bodies);

                // Select last body in list which is being tracked
                Body body = bodies.Where(thisBody => thisBody.IsTracked).LastOrDefault();

                // If it's not already tracking...
                if (!_faceSource.IsTrackingIdValid)
                {
                    // And if the body from the List<Body> isn't null...
                    if (body != null)
                    {
                        _faceSource.TrackingId = body.TrackingId;
                    }
                }
            }
        }

        private void _FaceFrameHandler(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            HighDefinitionFaceFrame faceFrame = e.FrameReference.AcquireFrame();

            // Checks face is tracked from body frame handler
            if (faceFrame != null && faceFrame.IsFaceTracked)
            {
                faceFrame.GetAndRefreshFaceAlignmentResult(_faceAlignment);

                DrawFacePoints();
            }
        }

        private void DrawFacePoints()
        {
            if (_faceModel == null)
            {
                return;
            }

            // Returns vertices as a list of read-only CameraSpacePoints - according to docs, points are in metres?
            var faceAlignedVertices = _faceModel.CalculateVerticesForAlignment(_faceAlignment);

            // Check that we have vertices to draw that have been aligned
            if (faceAlignedVertices.Count > 0)
            {

                // If the list of face vertices is 0 (None have been recorded yet...)
                if (_faceVertices.Count == 0)
                {
                    for (int i = 0; i < faceAlignedVertices.Count; i++)
                    {
                        Ellipse faceVertex = new Ellipse { Width = 2.0, Height = 2.0, Fill = new SolidColorBrush(Colors.White) };

                        _faceVertices.Add(faceVertex);
                    }

                    foreach (Ellipse faceVertex in _faceVertices)
                    {
                        // Make each face vertex a child of the drawing canvas
                        _kinectDrawCanvas.Children.Add(faceVertex);
                    }
                }

                for (int i = 0; i < faceAlignedVertices.Count; i++)
                {
                    // 3D Camera point. Has to be converted to 2D "depth point"
                    CameraSpacePoint faceSpacePoint = faceAlignedVertices[i];

                    // This is how we convert metres -> pixel location
                    // The Kinect Sensor has an in-built "coordinate mapper"
                    DepthSpacePoint pixelLocation = _kinectSensor.CoordinateMapper.MapCameraPointToDepthSpace(faceSpacePoint);

                    // Draw face vertices to the canvas by setting left and top properties to the X and Y
                    // Mapped by coordinate mapper
                    Canvas.SetLeft(_faceVertices[i], pixelLocation.X);
                    Canvas.SetTop(_faceVertices[i], pixelLocation.Y);
                }
            }
        }
    }
}
