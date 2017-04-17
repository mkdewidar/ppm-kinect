using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoseData;

namespace PoseData
{
    public struct Pose
    {
        public List<Vector2> faceRefPoints;
        public float tolerance;

        public Pose(List<Vector2> posePoints, float poseTolerance)
        {
            faceRefPoints = posePoints;
            tolerance = poseTolerance;
        }
    }
}

public abstract class ExerciseManager : MonoBehaviour
{
    protected Pose _currentExercise;
    // A reference to the kinect necessary to get the current pose
    protected KinectSource kinect;

    /// <summary>
    /// A base class Start function to setup the kinect reference and other common stuff.
    /// Even when overriden by subclass Unity can still call it when necessary.
    /// </summary>
    virtual protected void Start()
    {
        kinect = KinectSource.instance;
        Debug.Log("Kinect reference setup");
    }

    protected bool CheckUserPose(Pose currentPose)
    {
        // Currently only supports poses with the same face points!
        // Compares pose refPoints[i] to see if they are equal.
        // Assumes refPoints at index i are matching facial features.
        if (currentPose.faceRefPoints.Count != _currentExercise.faceRefPoints.Count)
        {
            Debug.LogError("Error: currentPose and _currentExercise counts do not match!");
        }
        else
        {
            for (int i = 0; i < currentPose.faceRefPoints.Count; i++)
            {
                bool pointsCollide = CheckPointsCollide(
                    currentPose.faceRefPoints[i], 
                    _currentExercise.faceRefPoints[i]
                    );

                if (!pointsCollide)
                {
                    return false;
                }
            }
        }

        return true;
    }

    protected bool CheckPointsCollide(Vector2 userFacePoint, Vector2 exerciseFacePoint)
    {
        float upperBound_x = exerciseFacePoint.x + _currentExercise.tolerance;
        float lowerBound_x = exerciseFacePoint.x - _currentExercise.tolerance;

        float upperBound_y = exerciseFacePoint.y + _currentExercise.tolerance;
        float lowerBound_y = exerciseFacePoint.y - _currentExercise.tolerance;

        if (userFacePoint.x > lowerBound_x && userFacePoint.x < upperBound_x)
        {
            if (userFacePoint.y > lowerBound_y && userFacePoint.y < upperBound_y)
            {
                return true;
            }
        }

        return false;
    }
}
