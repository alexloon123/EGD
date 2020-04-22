using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


public class title : MonoBehaviour
{

    public Conductor heartbeat;

    void Update()
    {
        if(heartbeat.speed > 4)
        {
            playGame();
        }
    }

    public void playGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
