using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<GameManager>();
            }

            return _instance;
        }
    }
    
    // All the minigame scenes
    private string[] _gameScenes = { };
    private int _currentSceneIndex;
    private bool _paused;
    private static GameManager _instance;

	void Start ()
    {
        _currentSceneIndex = 0;
        _paused = false;
        DontDestroyOnLoad(gameObject);
	}

    public void NextMiniGame()
    {
        SceneManager.UnloadSceneAsync(_gameScenes[_currentSceneIndex]);
        _currentSceneIndex++;
        SceneManager.LoadScene(_gameScenes[_currentSceneIndex]);
    }
}
