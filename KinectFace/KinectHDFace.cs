using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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
        const int COLOUR_FRAME_WIDTH = 1920;
        const int COLOUR_FRAME_HEIGHT = 1080;

        private KinectSensor _kinectSensor = null;
        private MultiSourceFrameReader _multiFrameReader = null;
        private HighDefinitionFaceFrameReader _faceReader = null;
        private HighDefinitionFaceFrameSource _faceSource = null;
        private FaceAlignment _faceAlignment = null;

        // colour image for the Kinect colour frames, displayed in window
        //private Image _colourImage;

        // canvas for drawing points on the face, displayed in window
        private Canvas _windowCanvas = null;

        // Allows access to the creates 3D mesh of the face (set of vertices)
        private FaceModel _faceModel = null;

        // List of vertices which are aligned with the face
        private List<Ellipse> _faceVertices = new List<Ellipse>();

        // Stopwatch for FPS counter
        private Stopwatch _frameTime = new Stopwatch();
        private Logfps _logfps = new Logfps();

        public KinectHDFace(KinectSensor sensor, Canvas drawingCanvas)
        {
            _frameTime.Start();
            _windowCanvas = drawingCanvas;
            _kinectSensor = sensor;
            //_colourImage = colourImg;

            if (_kinectSensor != null)
            {
                _multiFrameReader = _kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color
                    | FrameSourceTypes.Body);
                _multiFrameReader.MultiSourceFrameArrived += _OnMultiFrameArrived;

                _faceSource = new HighDefinitionFaceFrameSource(_kinectSensor);
                _faceReader = _faceSource.OpenReader();
                _faceReader.FrameArrived += _FaceFrameHandler;

                _faceModel = new FaceModel();
                _faceAlignment = new FaceAlignment();
            }
        }

        /// <summary>
        /// Handles all the frames that arrive from the multi source reader in one go.
        /// 
        /// It calls the respective frame handlers for each source.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The reference to the multi source frame</param>
        private void _OnMultiFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {

            //frame rate counter stuff:
            _frameTime.Stop();
            long currentFrameTime = _frameTime.ElapsedMilliseconds;
            _frameTime.Reset();
            _logfps.addNewFrameTime(currentFrameTime);
            _frameTime.Start();

            var multiSourceFrame = e.FrameReference.AcquireFrame();
            if (multiSourceFrame != null)
            {
                _ColorFrameHandler(multiSourceFrame.ColorFrameReference);

                _BodyFrameHandler(multiSourceFrame.BodyFrameReference);
            }
        }

        private void _ColorFrameHandler(ColorFrameReference frameRef)
        {
            using (ColorFrame frame = frameRef.AcquireFrame())
            {
                if (frame != null)
                {
                    // The below algorithm is from the internet
                    // I have just found ways to take colourFrame width/height and convert 
                    // it to necessary types
                    int width = frame.FrameDescription.Width;
                    int height = frame.FrameDescription.Height;

                    byte[] pixels = new byte[width * height * ((PixelFormats.Bgr32.BitsPerPixel + 7) / 8)];

                    if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
                    {
                        frame.CopyRawFrameDataToArray(pixels);
                    }
                    else
                    {
                        frame.CopyConvertedFrameDataToArray(pixels, ColorImageFormat.Bgra);
                    }

                    int stride = width * PixelFormats.Bgra32.BitsPerPixel / 8;

                    //_colourImage.Source = BitmapSource.Create(width, height,
                    //    96, 96, PixelFormats.Bgr32, null, pixels, stride);

                    ImageBrush canvasBackgroundBrush = new ImageBrush();
                    canvasBackgroundBrush.ImageSource = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgr32, null, pixels, stride);

                    _windowCanvas.Background = canvasBackgroundBrush;
                }
            }
        }

        private void _BodyFrameHandler(BodyFrameReference frameRef)
        {
            using (BodyFrame frame = frameRef.AcquireFrame())
            {
                if (frame != null)
                {
                    Body[] bodies = new Body[frame.BodyCount];
                    frame.GetAndRefreshBodyData(bodies);

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
        }

        private void _FaceFrameHandler(object sender, HighDefinitionFaceFrameArrivedEventArgs e)
        {
            HighDefinitionFaceFrame faceFrame = e.FrameReference.AcquireFrame();

            // Checks face is tracked from body frame handler
            if (faceFrame != null && faceFrame.IsFaceTracked)
            {
                Logfps._faceDetected = true;
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

            // Returns vertices as a list of read-only CameraSpacePoints
            // according to docs, points are in metres?
            var faceAlignedVertices = _faceModel.CalculateVerticesForAlignment(_faceAlignment);

            // Check that we have vertices to draw that have been aligned
            if (faceAlignedVertices.Count > 0)
            {

                // If the list of face vertices is 0 (None have been recorded yet...)
                if (_faceVertices.Count == 0)
                {
                    for (int i = 0; i < faceAlignedVertices.Count; i++)
                    {
                        Ellipse faceVertex = new Ellipse { Width = 2.0, Height = 2.0,
                            Fill = new SolidColorBrush(Colors.White) };

                        _faceVertices.Add(faceVertex);
                    }

                    foreach (Ellipse faceVertex in _faceVertices)
                    {
                        // Make each face vertex a child of the drawing canvas
                        _windowCanvas.Children.Add(faceVertex);
                    }
                }

                for (int i = 0; i < faceAlignedVertices.Count; i++)
                {
                    // 3D Camera point. Has to be converted to 2D "depth point"
                    CameraSpacePoint faceSpacePoint = faceAlignedVertices[i];

                    // This is how we convert metres -> pixel location
                    // The Kinect Sensor has an in-built "coordinate mapper"
                    ColorSpacePoint pixelLocation = _kinectSensor.CoordinateMapper
                        .MapCameraPointToColorSpace(faceSpacePoint);


                    // Align face vertices on the canvas by setting left and top properties to the X and Y
                    // Mapped by coordinate mapper; attempted to scale to width and height of canvas
                    Canvas.SetLeft(_faceVertices[i], pixelLocation.X * (_windowCanvas.Width / COLOUR_FRAME_WIDTH) - _faceVertices[i].Width / 2);
                    Canvas.SetTop(_faceVertices[i], pixelLocation.Y * (_windowCanvas.Height / COLOUR_FRAME_HEIGHT) - _faceVertices[i].Height / 2);
                }
                
             }
        }
    }
}
