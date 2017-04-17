using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePointPlotter : MonoBehaviour {

    [SerializeField]
    Transform pointPrefab;

    [SerializeField]
    KinectSource kinectSource;

	void Awake() {
        if (kinectSource != null)
        {
            for (int index = 0; index < kinectSource.facePoints.Length; index++)
            {
                Instantiate(pointPrefab, Vector3.zero, Quaternion.identity, transform);
            }
        }
	}
	
	void Update () {
        if (kinectSource != null)
        {
            if (transform.childCount == 0)
            {
                // we never got to setup our children, so lets setup now
                Awake();
            }
            Transform[] pointTransforms = GetComponentsInChildren<Transform>();
            Vector3[] pointCoords = kinectSource.facePoints;

            for (int index = 0; index < pointCoords.Length; index++)
            {
                pointTransforms[index].position = pointCoords[index];
            }
        }
	}
}
