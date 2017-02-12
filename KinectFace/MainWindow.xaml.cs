using System.Windows;
using Microsoft.Kinect;

namespace KinectFace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor _kinectSensor;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void onWindowLoad(object sender, RoutedEventArgs e)
        {
            canvas.Width = this.Width * 0.8;
            canvas.Height = this.Height;

            _kinectSensor = KinectSensor.GetDefault();
            _kinectSensor.Open();

            // the open call above is actually async so this has it wait a bit so 
            // the error message doesn't show up unecessarily
            System.Threading.Thread.Sleep(1000);

            while (!_kinectSensor.IsAvailable)
            {
                // the result of the message box
                MessageBoxResult result = MessageBox.Show(
                    "Please ensure a Kinect v2 is connected.\n\nDo you want to try again?",
                    "No Kinect Detected", MessageBoxButton.OKCancel, MessageBoxImage.Error);

                if (result == MessageBoxResult.Cancel)
                {
                    Application.Current.Shutdown();
                    // by default shutdown doesn't leave the function immediately
                    // return ensures that it won't try to finish the constructor
                    return;
                }
                else
                {
                    _kinectSensor = KinectSensor.GetDefault();
                    _kinectSensor.Open();
                    System.Threading.Thread.Sleep(1000);
                }
            }

            KinectHDFace FaceTracker = new KinectHDFace(_kinectSensor, canvas);
        }
    }
}
