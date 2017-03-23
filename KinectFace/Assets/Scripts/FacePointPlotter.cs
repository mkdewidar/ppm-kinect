using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	}
	
	void Update () {
        Transform[] pointTransforms = GetComponentsInChildren<Transform>();
        Vector3[] pointCoords = colorSource.facePoints;

        for (int index = 0; index < pointCoords.Length; index++)
        {
            pointTransforms[index].position = pointCoords[index];
        }
	}
}
