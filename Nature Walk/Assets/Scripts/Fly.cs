using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour
{
    public float flySpeed = 3f; // Adjust this to control the speed of flying
    public OVRCameraRig cameraRig;
    public float height = 4.5f;
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        // Get input from the thumbstick
        Vector2 thumbstickInput = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        // Debug.Log(thumbstickInput);
        // Calculate the movement direction based on the thumbstick input
        Vector3 moveDirection = new Vector3(thumbstickInput.x, 0f, thumbstickInput.y);
        moveDirection = transform.TransformDirection(moveDirection); // Transform relative to the camera's orientation
        
        // Move the camera position
        Quaternion rotate = cameraRig.centerEyeAnchor.rotation;
        rotate *= Quaternion.Euler(0, 90, 0);
        transform.position += rotate * moveDirection * flySpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}