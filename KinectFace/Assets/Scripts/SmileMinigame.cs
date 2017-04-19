
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains all the logic for the smiling minigame.
/// </summary>
public class SmileMinigame : ExerciseManager
{
    public GameObject flower;
    private GameObject actualflower;
    private int smileCount;
	override protected void Start()
    {
        base.Start();
        // The current exercise needs to be loaded from somewhere, we need to have the poses saved in some file
        _currentExercise = PoseFileHandler.LoadExercise("smile");
        actualflower = Instantiate(flower, new Vector3(0, -8), Quaternion.identity);
	}
	
	void Update()
    {
        if (!_kinect)
        {
            _kinect = KinectSource.instance;
        }
        else
        {
            Pose currentPose = _kinect.GetCurrentPose();
            if (_currentExercise.IsSimilar(currentPose, _tolerance))
            {
                smileCount++;
                if (smileCount > 100)
                {
                    Debug.Log("You have completed the smile exercise! onto the next one! -> ");
                    GameManager.instance.NextMiniGame();
                }
                else
                {
                    Debug.Log(smileCount);
                    float flowerGrow = ((float)smileCount / 100) * 90;
                    Debug.Log(flowerGrow);
                    actualflower.transform.Translate(new Vector3(0, 0.1f));
                }
                //Debug.Log("You've finished the smile exercise! To the next one! -> ");
                //GameManager.instance.NextMiniGame();
            }
        }
	}
}
