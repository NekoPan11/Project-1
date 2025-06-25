using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public AudioSource menuSound;
    public void PlayGame()
    {
        menuSound.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ExitGame()
    {
        menuSound.Play();
        Application.Quit();
    }
    public void Button_Lobby()
    {
        menuSound.Play();
        SceneManager.LoadScene(0);
    }
}