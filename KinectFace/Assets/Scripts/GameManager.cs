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
    public static KinectSource kinectInstance;

    // All the minigame scenes
    private string[] _gameScenes = { "Smile" };
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

        kinectInstance = FindObjectOfType<KinectSource>();
	}

    public void NextMiniGame()
    {
        SceneManager.UnloadSceneAsync(_gameScenes[_currentSceneIndex]);
        _currentSceneIndex++;
        SceneManager.LoadScene(_gameScenes[_currentSceneIndex]);
    }
}
