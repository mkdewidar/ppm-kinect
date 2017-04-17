using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PoseFileHandler
{
    private KinectSource _kinect;
    private static string _exercisesFile = Application.dataPath + "/ExerciseCaptures/exercises.csv";

    public PoseFileHandler()
    {
        _kinect = KinectSource.instance;
    }

    public void SaveNewExercise(string exerciseName)
    {
        if (_kinect == null)
        {
            _kinect = KinectSource.instance;
        }

        Pose currentPose = _kinect.GetCurrentPose();
        
        string exerciseEntry = exerciseName;

        foreach (Vector2 point in currentPose.faceRefPoints)
        {
            exerciseEntry += ",";
            string pointEntry = point.x + ":" + point.y;
            exerciseEntry += pointEntry;
        }

        exerciseEntry += "\n";

        File.AppendAllText(_exercisesFile, exerciseEntry);
    }

    public static Pose LoadExercise(string exerciseName)
    {
        string[] exercises = File.ReadAllLines(_exercisesFile);

        foreach (string exercise in exercises)
        {
            string[] exerciseData = exercise.Split(',');

            if (exerciseData[0] != exerciseName)
            {
                continue;
            }
            else
            {
                return CreatePose(exerciseData);
            }
        }

        return new Pose(null);
    }

    private static Pose CreatePose(string[] exercise)
    {
        List<Vector2> posePoints = new List<Vector2>();

        for (int i = 1; i < exercise.Length; i++)
        {
            string[] pointCoordinates = exercise[i].Split(':');

            float x = float.Parse(pointCoordinates[0]);
            float y = float.Parse(pointCoordinates[1]);

            Vector2 point = new Vector2(x, y);
            posePoints.Add(point);
        }

        return new Pose(posePoints);
    }
}
