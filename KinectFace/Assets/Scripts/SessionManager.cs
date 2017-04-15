using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the game session that the user is in, this manages which state 
///     of the game the user is in (menu, game etc).
/// 
/// It is a singleton implementation that will only ever exist once
/// </summary>
public class SessionManager : MonoBehaviour
{
    public static SessionManager instance
    {
        get
        {
            if (_instance == null)
            {
                // We can't rely on the start for the Session Manager to run before 
                // we try and return the instance below, so we just set it now 
                // and start is called whenever unity decides too
                _instance = new GameObject().AddComponent<SessionManager>();
            }
            return _instance;
        }
    }
    public enum SessionState { MENU, GAME, STATECOUNT }
    
    // The name of the scenes for each of the states, they are 
    // loaded whenever the game is moved into that state.
    private string[] _sessionScenes = { "Menu", "Game" };
    private static SessionManager _instance;
    private SessionState _sessionState;

	void Start ()
    {
        if ((_instance != null) && (_instance != this))
        {
            Debug.LogError("ERROR: There are other session managers, this shouldn't have happened");

            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        _sessionState = SessionState.MENU;
        SceneManager.LoadScene(_sessionScenes[(int)_sessionState]);

        _instance = this;
	}
	
    /// <summary>
    /// Closes all the other scenes in the game and loads this new state's scene.
    /// </summary>
    /// <param name="newState">The state that we are transitioning to</param>
    public void TransitionState(SessionState newState)
    {
        SceneManager.LoadScene(_sessionScenes[(int)newState]);

        _sessionState = newState;
    }
}
