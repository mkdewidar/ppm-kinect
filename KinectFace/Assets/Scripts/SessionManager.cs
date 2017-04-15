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
    public static SessionManager Instance;

    private enum SessionState { MENU, GAME, STATECOUNT }
    private SessionState sessionState;
    [SerializeField]
    // The name of the scenes for each of the states, they are 
    // loaded whenever the game is moved into that state.
    private string[] SessionScenes;

	void Start ()
    {
        if ((Instance != null) && (Instance != this))
        {
            Debug.LogError("ERROR: There are other session managers, this shouldn't have happened");

            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        sessionState = SessionState.MENU;
        SceneManager.LoadScene(SessionScenes[(int)sessionState]);

        Instance = this;
	}
	
    /// <summary>
    /// Closes all the other scenes in the game and loads this new state's scene.
    /// </summary>
    /// <param name="newState">The state that we are transitioning to</param>
    void TransitionState(SessionState newState)
    {
        SceneManager.LoadScene(SessionScenes[(int)newState]);

        sessionState = newState;
    }
}
