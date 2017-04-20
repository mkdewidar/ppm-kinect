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
    private string[] _gameScenes = { "Smile", "Blow" };
    private int _currentSceneIndex;
    private bool _paused;
    private static GameManager _instance;

	void Start()
    {
        if ((_instance != null) && (_instance != this))
        {
            Debug.LogError("ERROR: There are other game managers, this shouldn't have happened");

            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        _currentSceneIndex = 0;
        _paused = false;

        SceneManager.LoadScene(_gameScenes[_currentSceneIndex]);
	}

    public void NextMiniGame()
    {
        
        Debug.Log(_currentSceneIndex);
        _currentSceneIndex++;
        Debug.Log(_gameScenes[_currentSceneIndex]);
      
        SceneManager.LoadScene(_gameScenes[_currentSceneIndex]);
        //SceneManager.UnloadSceneAsync(_gameScenes[_currentSceneIndex-1]);
    }
}
