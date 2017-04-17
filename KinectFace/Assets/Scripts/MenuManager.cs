using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all the gui elements for the main menu.
/// </summary>
public class MenuManager : MonoBehaviour
{
    private bool nameInputEnabled = false;
    private string exerciseNameInput = "blow";
    PoseFileHandler poseFileHandler = null;


    public void onStartGame()
    {
        SessionManager.instance.TransitionState(SessionManager.SessionState.GAME);
    }

    public void onShowKinect()
    {
        poseFileHandler = new PoseFileHandler();
    }

    public void onCapturePose()
    {
        if (poseFileHandler == null)
        {
            onShowKinect();
        }
        else
        {
            poseFileHandler.SaveNewExercise(exerciseNameInput);
        }
    }
}
