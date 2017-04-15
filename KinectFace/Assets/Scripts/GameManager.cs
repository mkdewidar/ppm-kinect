using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
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

    private static GameManager _instance;
    // All the minigame scenes
    public string[] GameScenes = { };
    public int currentSceneIndex;
    public bool paused;

	void Start ()
    {
        currentSceneIndex = 0;
        paused = false;
        DontDestroyOnLoad(gameObject);
	}

    public void NextMiniGame()
    {
        SceneManager.UnloadSceneAsync(GameScenes[currentSceneIndex]);
        currentSceneIndex++;
        SceneManager.LoadScene(GameScenes[currentSceneIndex]);
    }
}
