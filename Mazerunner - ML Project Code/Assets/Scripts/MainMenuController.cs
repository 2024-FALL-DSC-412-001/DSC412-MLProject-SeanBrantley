using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
   public void LoadGame()
   {
       // Delete all stored player data when a new game starts
       PlayerPrefs.DeleteAll();

       SceneManager.LoadScene("Level 01");
   }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
