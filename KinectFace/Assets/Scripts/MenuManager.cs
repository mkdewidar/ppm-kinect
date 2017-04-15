using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages all the gui elements for the main menu.
/// </summary>
public class MenuManager : MonoBehaviour
{
    public void onStartGame()
    {
        SessionManager.Instance.TransitionState(SessionManager.SessionState.GAME);
    }
}
