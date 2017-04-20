using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlowMinigame : ExerciseManager
{
    private int blowCount;
    public GameObject tree;
    private GameObject actualtree;

	override protected void Start ()
    {
        blowCount = 0;
        base.Start();
        _currentExercise = PoseFileHandler.LoadExercise("blow");
        _currentExercise._sensitivity = 100;
        _tolerance = 0.09f;
        
        actualtree = Instantiate(tree, new Vector3(-2, -3), Quaternion.identity);
       // actualtree.transform.Rotate(new Vector3(0, 0, 90));
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
                    Debug.Log("You have completed the blow this exercise! onto the next one! -> ");
                    //GameManager.instance.NextMiniGame();
                }
                else
                {
                    //Debug.Log(blowCount);
                    float treefall = ((float)blowCount / 100) * 90;
                    //Debug.Log(treefall);
                    actualtree.transform.Rotate(new Vector3(0, 0, 1));
                }
            }
        }
	}
}
