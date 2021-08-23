using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Canvas canvas;
    public void PlayGame()
    {
        Time.timeScale = 1;
        canvas.GetComponent<AudioSource>().Stop();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit(); // Will quit only after build.
    }
}
