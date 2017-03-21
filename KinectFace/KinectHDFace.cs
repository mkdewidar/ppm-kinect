using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Kinect;
using Microsoft.Kinect.Face;

using Math = System.Math;

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


        //List stores int point indexes for key facial features for comparison
        //Only uses single points per feature per side, could be extended with additional nodes
        //.length method would return total number of elements, subsequently, TARGET_FEATURE_COUNT
        //should be equal to the rows in this array
        private int[,] _targetFeatureList = new int[,]
        {
            //mouth corners
            {(int) HighDetailFacePoints.MouthLeftcorner, (int)HighDetailFacePoints.MouthRightcorner},
            //eyebrows
            {(int) HighDetailFacePoints.LefteyebrowOuter, (int)HighDetailFacePoints.RighteyebrowOuter},
            {(int) HighDetailFacePoints.LefteyebrowCenter, (int)HighDetailFacePoints.RighteyebrowCenter},

            //cheeks
            {(int) HighDetailFacePoints.LeftcheekCenter, (int) HighDetailFacePoints.RightcheekCenter},
            {(int) HighDetailFacePoints.Leftcheekbone, (int) HighDetailFacePoints.Rightcheekbone}
        };

        private CameraSpacePoint _referencePoint;


        public KinectHDFace(KinectSensor sensor, Canvas drawingCanvas)
        {
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
                faceFrame.GetAndRefreshFaceAlignmentResult(_faceAlignment);

                _DrawFacePoints();
            }
        }

        private void _DrawFacePoints()
        {
            if (_faceModel == null | _faceAlignment == null)
            {
                return;
            }

            // Returns vertices as a list of read-only CameraSpacePoints
            // according to docs, points are in metres?
            var faceAlignedVertices = _faceModel.CalculateVerticesForAlignment(_faceAlignment);
            _referencePoint = faceAlignedVertices[(int)HighDetailFacePoints.NoseTop];

            //clear previous frame's points
            _windowCanvas.Children.Clear();
            _faceVertices = new List<Ellipse>();

            // Check that we have vertices to draw that have been aligned
            if (faceAlignedVertices.Count > 0)
            {

                // If the list of face vertices is 0 (None have been recorded yet...)
                if (_faceVertices.Count == 0)
                {
                    //initialise the ellipses for the features 
                    for (int i = 0; i < (_targetFeatureList.GetUpperBound(0) + 1); i++)
                    {
                        createFeatureEllipse(i, faceAlignedVertices[_targetFeatureList[i, 0]], faceAlignedVertices[_targetFeatureList[i, 1]]);
                    }

                    // create a ellipse for the reference point
                    createEllipseInstance(Colors.White);

                    foreach (Ellipse faceVertex in _faceVertices)
                    {
                        // Make each face vertex a child of the drawing canvas
                        _windowCanvas.Children.Add(faceVertex);
                    }
                }

                for (int i = 0; i < (_targetFeatureList.GetUpperBound(0) + 1); i++)
                {
                    for (int j = 0; j < (_targetFeatureList.GetUpperBound(1) + 1); j++)
                    {
                        // 3D Camera point. Has to be converted to 2D "depth point"
                        CameraSpacePoint faceSpacePoint = faceAlignedVertices[_targetFeatureList[i, j]];

                        // This is how we convert metres -> pixel location
                        // The Kinect Sensor has an in-built "coordinate mapper"
                        ColorSpacePoint pixelLocation = _kinectSensor.CoordinateMapper
                            .MapCameraPointToColorSpace(faceSpacePoint);


                        // Align face vertices on the canvas by setting left and top properties to the X and Y
                        // Mapped by coordinate mapper; attempted to scale to width and height of canvas
                        Canvas.SetLeft(_faceVertices[(i * 2 + j)], pixelLocation.X * (_windowCanvas.Width / COLOUR_FRAME_WIDTH) - _faceVertices[(i * 2 + j)].Width / 2);
                        Canvas.SetTop(_faceVertices[(i * 2 + j)], pixelLocation.Y * (_windowCanvas.Height / COLOUR_FRAME_HEIGHT) - _faceVertices[(i * 2 + j)].Height / 2);
                    }
                }

                // reference point
                ColorSpacePoint referencePoint = _kinectSensor.CoordinateMapper.MapCameraPointToColorSpace(_referencePoint);
                Canvas.SetLeft(_faceVertices.Last(), referencePoint.X * (_windowCanvas.Width / COLOUR_FRAME_WIDTH) - _faceVertices.Last().Width / 2);
                Canvas.SetTop(_faceVertices.Last(), referencePoint.Y * (_windowCanvas.Height / COLOUR_FRAME_HEIGHT) - _faceVertices.Last().Height / 2);

            }
        }

        private void createFeatureEllipse(int index, CameraSpacePoint left, CameraSpacePoint right)
        {
            //calculate vectors between the reference and the left and right feature
            double leftLength = Math.Sqrt(Math.Pow((left.X - _referencePoint.X), 2) + Math.Pow((left.Y - _referencePoint.Y), 2) + Math.Pow((left.Z - _referencePoint.Z), 2)) * 1000;
            double rightLength = Math.Sqrt(Math.Pow((right.X - _referencePoint.X), 2) + Math.Pow((right.Y - _referencePoint.Y), 2) + Math.Pow((right.Z - _referencePoint.Z), 2)) * 1000;

            //Calculates difference and scales up to 0 - 255 range (ish)
            double vectDiff = Math.Abs(leftLength - rightLength) * 50;

            if (vectDiff > 255)
            {
                vectDiff = 255;
            }

            Color featureColor = Colors.Black;
            featureColor.R = System.Convert.ToByte(vectDiff);
            featureColor.G = System.Convert.ToByte(vectDiff);
            featureColor.B = System.Convert.ToByte(vectDiff);

            for (int i = 0; i < 2; i++)
            {
                createEllipseInstance(featureColor);
            }
        }

        private void createEllipseInstance(Color col)
        {
            Ellipse newEllipse = new Ellipse
            {
                Width = 2.0,
                Height = 2.0,
                Fill = new SolidColorBrush(col)
            };

            _faceVertices.Add(newEllipse);
        }
    }
}
