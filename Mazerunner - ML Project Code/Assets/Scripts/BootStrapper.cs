using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootStrapper : MonoBehaviour
{
    public GameObject gameTimerPrefab;
    public string timerTextObjectName = "Timer Text (TMP)";

    private void Awake()
    {
        if (GameTimer.Instance == null)
        {
            Instantiate(gameTimerPrefab);
        }

        GameTimer.Instance.UpdateTimerTextReference(timerTextObjectName);
    }
}
