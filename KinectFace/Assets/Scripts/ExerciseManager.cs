using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExerciseManager : MonoBehaviour
{
    protected Pose _currentExercise;
    // A reference to the kinect necessary to get the current pose
    protected KinectSource _kinect;
    protected float _tolerance;

    /// <summary>
    /// A base class Start function to setup the kinect reference and other common stuff.
    /// Even when overriden by subclass Unity can still call it when necessary.
    /// </summary>
    virtual protected void Start()
    {
        _kinect = KinectSource.instance;
        _tolerance = 0.1f;
        Debug.Log("Kinect reference setup");
    }
}