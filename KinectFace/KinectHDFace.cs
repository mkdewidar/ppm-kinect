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

        public KinectHDFace(KinectSensor sensor, Canvas drawingCanvas)
        {
            // Assigning drawing canvas for colour frames
            _kinectDrawCanvas = drawingCanvas;

            // Assign the default Kinect Sensor; must be connected and open
            _kinectSensor = sensor;

            // If assignment was success
            if (_kinectSensor != null && _kinectSensor.IsAvailable)
            {
                // find a multi-source frame reader and fire _DrawColourFrame when frames arrive
                _multiReader = _kinectSensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color);
                _multiReader.MultiSourceFrameArrived += _DrawColourFrame;
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
    }
}
