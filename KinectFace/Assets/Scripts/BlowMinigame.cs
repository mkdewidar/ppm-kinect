using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowMinigame : ExerciseManager
{
	override protected void Start ()
    {
        base.Start();
        _currentExercise = PoseFileHandler.LoadExercise("blow");
	}
	
	void Update ()
    {
		if (_kinect != null)
        {
            Pose currentPose = _kinect.GetCurrentPose();

            if (_currentExercise.IsSimilar(currentPose, _tolerance))
            {
                Debug.Log("You have completed this exercise! onto the next one! -> ");
                //GameManager.instance.NextMiniGame();
            }
        }
	}
}
