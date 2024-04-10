using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

        if (moveForward) {
            camera.transform.position += speed * Vector3.forward;
        }
    }
}
