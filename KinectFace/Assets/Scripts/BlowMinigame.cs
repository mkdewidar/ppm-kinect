using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowMinigame : ExerciseManager
{
    private int blowCount;
    public GameObject tree;

	override protected void Start ()
    {
        blowCount = 0;
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
                blowCount++;
                if (blowCount > 100)
                {
                    Debug.Log("You have completed this exercise! onto the next one! -> ");
                    //GameManager.instance.NextMiniGame();
                }
            }
        }
	}
}
