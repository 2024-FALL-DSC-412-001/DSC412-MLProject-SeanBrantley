using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private AudioSource myAudioSource;

    [SerializeField] private AudioClip[] songArray;
    private int songIndex = 0;

    private void Awake()
    {
        // Singleton pattern to ensure only one MusicManager exists
        /*if (FindObjectsOfType<MusicManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }

        myAudioSource = GetComponent<AudioSource>();*/
    }

    private void Update()
    {
        // Check if the current scene is the Main Menu
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            if (myAudioSource.isPlaying)
            {
                myAudioSource.Stop();
            }
        }
        else
        {
            // If music is not playing, play the next song
            if (!myAudioSource.isPlaying)
            {
                myAudioSource.clip = songArray[songIndex];
                myAudioSource.Play();

                songIndex = (songIndex + 1) % songArray.Length;
            }
        }
    }
}
