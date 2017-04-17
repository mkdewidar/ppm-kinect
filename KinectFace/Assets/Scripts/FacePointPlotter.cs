using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePointPlotter : MonoBehaviour {

    [SerializeField]
    Transform pointPrefab;

    [SerializeField]
    KinectSource kinectSource;

	void Start () {
        for (int index = 0; index < kinectSource.facePoints.Length; index++)
        {
            Instantiate(pointPrefab, Vector3.zero, Quaternion.identity, transform);
        }
	}
	
	void Update () {
        Transform[] pointTransforms = GetComponentsInChildren<Transform>();
        Vector3[] pointCoords = kinectSource.facePoints;

        for (int index = 0; index < pointCoords.Length; index++)
        {
            pointTransforms[index].position = pointCoords[index];
        }
	}
}
