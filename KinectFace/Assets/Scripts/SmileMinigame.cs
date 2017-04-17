using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PoseData;

/// <summary>
/// Contains all the logic for the smiling minigame.
/// </summary>
public class SmileMinigame : ExerciseManager
{
	override protected void Start()
    {
        base.Start();
        // The current exercise needs to be loaded from somewhere, we need to have the poses saved in some file
        // _currentExercise = ;
	}
	
	void Update()
    {
        if (kinect != null)
        {
            Pose currentPose = kinect.GetCurrentPose();
            currentPose.tolerance = 1f;
            if (CheckUserPose(currentPose))
            {
                Debug.Log("You've finished this exercise! To the next one! -> ");
                GameManager.instance.NextMiniGame();
            }
        }
	}
}
