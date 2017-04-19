using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            Transform[] pointTransformsAll = GetComponentsInChildren<Transform>();
            Transform[] pointTransforms = new Transform[pointTransformsAll.Length-1] ;
            int indexer = 0;
            //getcomponentsinchildren also gets the component of the parent, this loop removes it
            foreach (Transform child in pointTransformsAll)
            {
                if(child!= this.transform)
                {
                    pointTransforms[indexer] = child;
                    indexer++;
                }
            }
            
            Vector3[] pointCoords = kinectSource.facePoints;

            for (int index = 0; index < pointCoords.Length; index++)
            {
                Vector3 temp = pointCoords[index];
                temp.Scale(new Vector3(2, 2, 0));
                pointTransforms[index].position = temp;
            }
        }
	}
}
