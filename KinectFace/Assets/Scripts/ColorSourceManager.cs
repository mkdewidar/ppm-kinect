using System.Linq;
using UnityEngine;
using Windows.Kinect;
using Microsoft.Kinect.Face;

public class ColorSourceManager : MonoBehaviour {

    public int colorWidth { get; private set; }
    public int colorHeight { get; private set; }
    public Texture2D texture { get; set; }
    public Vector3[] facePoints { get; private set; }
    public System.EventHandler facePointsReady;

    private KinectSensor _sensor;
    private byte[] _colorPixels;
    private MultiSourceFrameReader _multiFrameReader;
    private HighDefinitionFaceFrameReader _faceReader;
    private HighDefinitionFaceFrameSource _faceSource;
    private FaceAlignment _faceAlignment;
    private FaceModel _faceModel;
    private Renderer _renderer;

    void Awake()
    {
        _sensor = KinectSensor.GetDefault();

        if (_sensor != null)
        {
            _multiFrameReader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color);
            _multiFrameReader.MultiSourceFrameArrived += _OnMultiFrameArrived;

            var frameDesc = _sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            colorWidth = frameDesc.Width;
            colorHeight = frameDesc.Height;

            _faceSource = HighDefinitionFaceFrameSource.Create(_sensor);
            _faceReader = _faceSource.OpenReader();
            _faceReader.FrameArrived += _FaceFrameHandler;

            _faceModel = FaceModel.Create();

            texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);
            _colorPixels = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];

            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }
        }

        _renderer = GetComponent<Renderer>();
        // 1 and -1 are the settings that let it output be like a mirror
        _renderer.material.SetTextureScale("_MainTex", new Vector2(1, -1));

        facePoints = new Vector3[1347];
    }

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
                frame.CopyConvertedFrameDataToArray(_colorPixels, ColorImageFormat.Rgba);
                texture.LoadRawTextureData(_colorPixels);
                texture.Apply();

                _renderer.material.mainTexture = texture;
            }
        }
        Debug.Log("Rendering color frame");
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
        using (HighDefinitionFaceFrame faceFrame = e.FrameReference.AcquireFrame())
        {
            // Checks face is tracked from body frame handler
            if (faceFrame != null && faceFrame.IsFaceTracked)
            {
                faceFrame.GetAndRefreshFaceAlignmentResult(_faceAlignment);
                CameraSpacePoint[] vertsInCamSpace = _faceModel.CalculateVerticesForAlignment(_faceAlignment).ToArray<CameraSpacePoint>();

                for (int index = 0; index < vertsInCamSpace.Length; index++)
                {
                    facePoints[index] = new Vector3(vertsInCamSpace[index].X * 10, vertsInCamSpace[index].Y * 10, vertsInCamSpace[index].Z * 10);
                }
            }

            Debug.Log("Handling face frame");
            facePointsReady(this, new System.EventArgs());
        }

    }

    void OnApplicationQuit()
    {
        if (_multiFrameReader != null)
        {
            _multiFrameReader.Dispose();
            _multiFrameReader = null;
        }

        if (_sensor != null)
        {
            if (_sensor.IsOpen)
            {
                _sensor.Close();
            }

            _sensor = null;
        }
    }
}
