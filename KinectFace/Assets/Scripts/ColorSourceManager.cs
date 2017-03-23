using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Windows.Kinect;
using Microsoft.Kinect.Face;

public class ColorSourceManager : MonoBehaviour {

    public int colorWidth { get; private set; }
    public int colorHeight { get; private set; }
    public Texture2D texture { get; set; }
    public Vector3[] facePoints { get; private set; } 

    private KinectSensor _sensor;
    private MultiSourceFrameReader _reader;
    private byte[] _colorPixels;
    private HighDefinitionFaceFrameSource _faceSource;
    private HighDefinitionFaceFrameReader _faceReader;
    private FaceAlignment _faceAlignment;
    private FaceModel _faceModel;
    private Renderer _renderer;

    void Awake()
    {
        _sensor = KinectSensor.GetDefault();

        if (_sensor != null)
        {
            _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);

            _faceSource = HighDefinitionFaceFrameSource.Create(_sensor);
            _faceReader = _faceSource.OpenReader();

            FrameDescription frameDesc = _sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            colorWidth = frameDesc.Width;
            colorHeight = frameDesc.Height;

            _faceAlignment = FaceAlignment.Create();
            _faceModel = FaceModel.Create();


            texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);
            _colorPixels = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];

            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }
        }

        _renderer = GetComponent<Renderer>();
        _renderer.material.SetTextureScale("_MainTex", new Vector2(1, -1));
        
        // there are approx 1347 points that can be tracked
        facePoints = new Vector3[1347];
    }

    void Update()
    {
        // the pixels of the color source are applied to a texture which is renderered as a material
        if (_reader != null)
        {
            MultiSourceFrame multiSourceFrame = _reader.AcquireLatestFrame();

            if (multiSourceFrame != null)
            {
                ColorFrame colorFrame = multiSourceFrame.ColorFrameReference.AcquireFrame();
                if (colorFrame != null)
                {
                    colorFrame.CopyConvertedFrameDataToArray(_colorPixels, ColorImageFormat.Rgba);
                    texture.LoadRawTextureData(_colorPixels);
                    texture.Apply();

                    colorFrame.Dispose();
                }

                BodyFrame bodyFrame = multiSourceFrame.BodyFrameReference.AcquireFrame();
                if (bodyFrame != null)
                {
                    Body[] bodies = new Body[bodyFrame.BodyCount];
                    bodyFrame.GetAndRefreshBodyData(bodies);

                    // Select last body in list which is being tracked
                    Body body = bodies.Where(thisBody => thisBody.IsTracked).FirstOrDefault();

                    // If it's not already tracking...
                    if (!_faceSource.IsTrackingIdValid)
                    {
                        // And if the body from the List<Body> isn't null...
                        if (body != null)
                        {
                            _faceSource.TrackingId = body.TrackingId;
                        }
                    }

                    bodyFrame.Dispose();
                }

                HighDefinitionFaceFrame faceFrame = _faceReader.AcquireLatestFrame();
                if (faceFrame != null)
                {
                    faceFrame.GetAndRefreshFaceAlignmentResult(_faceAlignment);
                    CameraSpacePoint[] vertInCamSpace = _faceModel.CalculateVerticesForAlignment(_faceAlignment).ToArray<CameraSpacePoint>();

                    for (int index = 0; index < vertInCamSpace.Length; index++)
                    {
                        facePoints[index] = new Vector3(vertInCamSpace[index].X * 10, vertInCamSpace[index].Y * 10, vertInCamSpace[index].Z * 10);
                    }

                    faceFrame.Dispose();
                }
            }
        }
        _renderer.material.mainTexture = texture;
    }



    void OnApplicationQuit()
    {
        if (_reader != null)
        {
            _reader.Dispose();
            _reader = null;
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
