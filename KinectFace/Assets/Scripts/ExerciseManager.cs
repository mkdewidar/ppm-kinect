using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoseData;

namespace PoseData
{
    public struct ReferencePoint
    {
        public float x { get; private set; }
        public float y { get; private set; }
    }

    public struct Pose
    {
        public List<ReferencePoint> faceRefPoints { get; private set; }
        public float tolerance { get; private set; }
    }
}

public abstract class ExerciseManager : MonoBehaviour
{
    static public Pose _currentExercise { get; set; }

    public static bool CheckUserPose(Pose currentPose)
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

    private static bool CheckPointsCollide(ReferencePoint userFacePoint, ReferencePoint exerciseFacePoint)
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
