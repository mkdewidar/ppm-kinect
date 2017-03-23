using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Kinect.Face;

public class FacePointPlotter : MonoBehaviour {

    [SerializeField]
    Transform pointPrefab;
    [SerializeField]
    ColorSourceManager colorSource;

	void Start () {
        // creates as many points as needed
        for (int index = 0; index < colorSource.facePoints.Length; index++)
        {
            Instantiate(pointPrefab, Vector3.zero, Quaternion.identity, transform);
        }

        colorSource.facePointsReady += _OnFacePointsReady;
	}
	
	private void _OnFacePointsReady (object sender, System.EventArgs e) {
        Transform[] pointTransforms = GetComponentsInChildren<Transform>();
        Vector3[] pointCoords = colorSource.facePoints;

        for (int index = 0; index < pointCoords.Length; index++)
        {
            pointTransforms[index].position = pointCoords[index];
        }

        Debug.Log("Drawn face points");
    }
}
