using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class ColorSourceManager : MonoBehaviour {

    public int colorWidth { get; private set; }
    public int colorHeight { get; private set; }
    public Texture2D texture { get; set; }

    private KinectSensor _sensor;
    private ColorFrameReader _reader;
    private byte[] _colorPixels;

    private Renderer _renderer;

    void Start()
    {
        _sensor = KinectSensor.GetDefault();

        if (_sensor != null)
        {
            _reader = _sensor.ColorFrameSource.OpenReader();

            var frameDesc = _sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            colorWidth = frameDesc.Width;
            colorHeight = frameDesc.Height;

            texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);
            _colorPixels = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];

            if (!_sensor.IsOpen)
            {
                _sensor.Open();
            }
        }

        _renderer = GetComponent<Renderer>();
        _renderer.material.SetTextureScale("_MainTex", new Vector2(1, -1));
    }

    void Update()
    {
        // the pixels of the color source are applied to a texture which is renderered as a material
        if (_reader != null)
        {
            var frame = _reader.AcquireLatestFrame();

            if (frame != null)
            {
                frame.CopyConvertedFrameDataToArray(_colorPixels, ColorImageFormat.Rgba);
                texture.LoadRawTextureData(_colorPixels);
                texture.Apply();

                frame.Dispose();
                frame = null;
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
