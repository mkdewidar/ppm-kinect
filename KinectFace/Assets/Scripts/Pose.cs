using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pose {
    public List<Vector2> faceRefPoints;

    /// <summary>
    /// Whether or not this pose is similar to the other pose passed in.
    /// </summary>
    /// <param name="otherPose">The pose we're checking if it overlaps with this one.</param>
    /// <param name="tolerance">How much tolerance to allow in the x and y for this pose.</param>
    /// <returns>True if the other pose is within range of this one.</returns>
    public bool IsSimilar(Pose otherPose, float tolerance)
    {
        for (int i = 0; i < faceRefPoints.Count; i++)
        {
            float upperBound_x = faceRefPoints[i].x + tolerance;
            float lowerBound_x = faceRefPoints[i].x - tolerance;

            float upperBound_y = faceRefPoints[i].y + tolerance;
            float lowerBound_y = faceRefPoints[i].y - tolerance;

            if ((otherPose.faceRefPoints[i].x < lowerBound_x && otherPose.faceRefPoints[i].x > upperBound_x) &&
                (otherPose.faceRefPoints[i].y < lowerBound_y && otherPose.faceRefPoints[i].y > upperBound_y))
            {
                return false;
            }
        }

        return true;
    }
}
