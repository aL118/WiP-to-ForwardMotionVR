using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

/*
Passthrough tutorial: https://developer.oculus.com/documentation/unity/unity-passthrough-tutorial/
Wifi connect: https://developer.oculus.com/documentation/native/android/ts-adb/
*/
public class Tracker : MonoBehaviour
{
    public GameObject head;
    public GameObject handR;
    public GameObject handL;
    public TMP_Text messageText;
    public OVRCameraRig camera;
    bool moveForward = true;
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var headPosition = head.transform.position;
        var handRPosition = handR.transform.position;
        var handLPosition = handL.transform.position;
        // Debug.Log("head: "+headPosition);
        // Debug.Log("right: "+handRPosition);
        // Debug.Log("left: "+handLPosition);
        var message = "head: "+headPosition+"\nright: "+handRPosition+"\nleft: "+handLPosition;
        messageText.SetText(message);
        WriteString(headPosition,handRPosition,handLPosition);
        
        if (moveForward) {
            camera.transform.position += speed * Vector3.forward;
        }
    }

    void WriteString(Vector3 head,Vector3 handR,Vector3 handL)
    {
       string path = "Assets/Resources/test.csv";
       //Write some text to the test.txt file
       StreamWriter writer = new StreamWriter(path, true);
       var message = head+", "+handR+", "+handL;
       writer.WriteLine(message);
       writer.Close();
    }
}
