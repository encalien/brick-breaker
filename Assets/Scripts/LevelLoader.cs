using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    GameKeeper gameKeeper;
    void Start()
    {
        gameKeeper = FindObjectOfType<GameKeeper>();
    }

    public void StartGame()
    {
        if (gameKeeper)
        {
            gameKeeper.Reset();
        }
        NextLevel();
    }

    public void NextLevel()
    {
        if (gameKeeper)
        {
            gameKeeper.level++;
        }
        SceneManager.LoadScene(1);
    }

    public void GameOver()
    {
        SceneManager.LoadScene("Game Over");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
