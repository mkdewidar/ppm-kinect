using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all the gui elements for the main menu.
/// </summary>
public class MenuManager : MonoBehaviour
{
    KinectSource _kinect;
    private bool nameInputEnabled = false;
    private string exerciseNameInput = "blow";
    PoseFileHandler poseFileHandler = null;


    public void onStartGame()
    {
        SessionManager.instance.TransitionState(SessionManager.SessionState.GAME);
    }

    public void onShowKinect()
    {
        
    }

    public void onCapturePose()
    {
        if (_kinect == null)
        {
            _kinect = KinectSource.instance;
        }
        else
        {
            poseFileHandler = new PoseFileHandler();
            poseFileHandler.SaveNewExercise(exerciseNameInput);
        }
        
    }
}
