using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
// https://developer.oculus.com/documentation/unity/unity-passthrough-tutorial/
public class Tracker : MonoBehaviour
{
    // StreamWriter instance to write to the CSV file
    private StreamWriter writer;
    public bool writeMode = false;
    // Method to write data to CSV file
    public GameObject cube;
    public GameObject head;
    public GameObject handR;
    public GameObject handL;
    public TMP_Text messageText;

    private double lastInterval;
    private double firstStart;
    public void WriteToCSV()
    {
        var headPosition = head.transform.position;
        var handRPosition = handR.transform.position;
        var handLPosition = handL.transform.position;
        string headV = headPosition.x + "," + headPosition.y + "," + headPosition.z;
        string handRV = handRPosition.x + "," + handRPosition.y + "," + handRPosition.z;
        string handLV = handLPosition.x + "," + handLPosition.y + "," + handLPosition.z;
        double timePassed = Time.realtimeSinceStartupAsDouble - lastInterval;
        if(firstStart==0){
            firstStart = timePassed;
            timePassed = 0;
        }else{
            timePassed -= firstStart;
        }
        messageText.SetText("Time: "+Math.Round(timePassed, 2).ToString());
        string line = timePassed+","+headV+","+handRV+","+handLV;
        // Write data to the CSV file
        writer.WriteLine(line);

        // Flush the stream to ensure that all data is written to the file
        writer.Flush();
    }
    // Start is called before the first frame update
    void Start()
    {
        DateTime currentDate = DateTime.Now;
        string fileName = "speed-4.csv";//currentDate.ToString("yyyy-MM-dd-HH-mm-ss")+".csv";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        // Create or open the CSV file for writing
        writer = new StreamWriter(path, true); // Pass 'true' to append to the file if it already exists
        lastInterval = Time.realtimeSinceStartupAsDouble;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            writeMode=!writeMode;
            var cubeRenderer = cube.GetComponent<Renderer>();
            // Call SetColor using the shader property name "_Color" and setting the color to red
            if(cubeRenderer.material.color==Color.white){
                cubeRenderer.material.SetColor("_Color", Color.red);
            }else{
                cubeRenderer.material.SetColor("_Color", Color.white);
            }
            
        }
        if (writeMode){
            WriteToCSV();
        }
    }

    private void OnDestroy()
    {
        // Close the StreamWriter when the script is destroyed
        if (writer != null)
        {
            writer.Close();
        }
    }
}
