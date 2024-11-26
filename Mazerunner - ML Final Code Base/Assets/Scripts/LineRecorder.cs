using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LineRecorder : MonoBehaviour
{
    private GameObject player;
    private List<string> recordedPositions = new List<string>();
    private float startTime;
    private bool isRecording = true;
    private string currentLevel;

    private void Start()
    {
        startTime = Time.time;
        StartCoroutine("RecordPosition");
        Debug.Log("LineRecorder started.");
    }

    public void SetPlayer(GameObject playerObject)
    {
        player = playerObject;
        Debug.Log("Player object set in LineRecorder.");
    }

    public void SetLevel(string levelName)
    {
        currentLevel = levelName;
        Debug.Log("Current level set in LineRecorder: " + levelName);
    }

    private IEnumerator RecordPosition()
    {
        while (isRecording)
        {
            if (player != null)
            {
                float elapsedTime = Time.time - startTime;
                int roundedTime = Mathf.RoundToInt(elapsedTime);

                float roundedX = Mathf.Round(player.transform.position.x * 100f) / 100f;
                float roundedZ = Mathf.Round(player.transform.position.z * 100f) / 100f;

                recordedPositions.Add(roundedTime + "," + roundedX + "," + roundedZ);
            }
            yield return new WaitForSeconds(1);
        }
    }

    public void StopRecording()
    {
        isRecording = false;
        WriteToFile();
        Debug.Log("LineRecorder stopped recording.");
    }

    private void OnApplicationQuit()
    {
        if (isRecording)
        {
            WriteToFile();
        }
    }

    private void WriteToFile()
    {
        string directoryPath = Application.persistentDataPath + "/LineData";
        string filePath = directoryPath + "/PlayerData_" + currentLevel + ".csv";

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        using (StreamWriter file = new StreamWriter(filePath))
        {
            file.WriteLine("Time,X,Z");

            foreach (string line in recordedPositions)
            {
                file.WriteLine(line);
            }
        }

        Debug.Log("Data saved to: " + filePath);
    }
}