using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using Unity.Barracuda;
public class Fly : MonoBehaviour
{
    public float flySpeed; // Adjust this to control the speed of flying
    public OVRCameraRig cameraRig;
    public GameObject head;
    // public float height = 4f;

    public Terrain midland1;
    public Terrain midland2;
    public Terrain midland3;
    public Terrain midland4;
    public GameObject faraway;
    public TMP_Text messageText;
    public TMP_Text statText;
    public TMP_Text debug;
    public NNModel modelSource;

    int planeLength = 200;
    int initialOffset = -20;
    int sideOffset = -50;
    int cycle = 0;
    private ArrayList pastWindow = new ArrayList();
    private Model m_RuntimeModel;
    private IWorker worker;

    // https://developer.oculus.com/documentation/unity/unity-passthrough-tutorial-passthrough-window/
    void Start()
    {
        m_RuntimeModel = ModelLoader.Load(modelSource);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, m_RuntimeModel);
    }

    void Update()
    {
        predictSpeed();
        // Get input from the thumbstick
        Vector2 thumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        // Calculate the movement direction based on the thumbstick input
        Vector3 moveDirection = new Vector3(thumbstickInput.x, 0f, thumbstickInput.y);
        moveDirection = transform.TransformDirection(moveDirection); // Transform relative to the camera's orientation
        
        // Move the camera position
        // Quaternion rotate = cameraRig.centerEyeAnchor.rotation;
        // rotate *= Quaternion.Euler(0, 90, 0);
        // transform.position += flySpeed * Time.deltaTime;
        float speed = thumbstickInput.x * flySpeed;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime);
        // messageText.SetText("Speed: "+speed.ToString());

        if(cycle < (int)transform.position.z/planeLength){
            cycle = (int)transform.position.z/planeLength;
            Vector3 next = new Vector3(sideOffset,0,(cycle+2)*planeLength+initialOffset);
            if(cycle>1){
                if(cycle%4==1){
                    midland4.transform.position = next;
                }else if(cycle%4==2){
                    midland1.transform.position = next;
                }else if(cycle%4==3){
                    midland2.transform.position = next;
                }else {
                    midland3.transform.position = next;
                }
            }
            // terrain disappears after 1420
            // Debug.Log(cycle+" "+midland1.transform.position.z+" "+midland2.transform.position.z+" "+midland3.transform.position.z+" "+midland4.transform.position.z);
        }
        faraway.transform.position = new Vector3(sideOffset,0,transform.position.z+2*planeLength+initialOffset);
    }
    void predictSpeed(){
        var headPosition = head.transform.position;
        statText.SetText("Head.y: "+headPosition.y.ToString());
        int window=100;
        if(pastWindow.Count<window){
            pastWindow.Add(headPosition.y);
        }else{
            pastWindow.RemoveAt(0);
            pastWindow.Add(headPosition.y);
        }
        float[] raw = (float[])pastWindow.ToArray(typeof(float));
        if(pastWindow.Count==window){           

            var inputTensor = new Tensor(1, 1, normalize(raw));
            debug.SetText(raw.Length.ToString()+" "+pastWindow[pastWindow.Count-1].ToString()+" "+inputTensor);
            // worker.Execute(inputTensor);

            // var output = worker.PeekOutput();
           
            // messageText.SetText("*Speed: "+output);
            
            inputTensor.Dispose();
            // output.Dispose();
            // worker.Dispose();
        }else{
            debug.SetText(raw.Length.ToString()+" "+pastWindow[pastWindow.Count-1].ToString()+" "+raw[raw.Length-1].ToString());
        }
        
    }
    float mean(float []a, int n) {
        float sum = 0;   
        for (int i = 0; i < n; i++)
            sum += a[i];  
        float mean = (float)sum /  (float)n;
        return mean;
    }
    float variance(float []a, int n) {
        float m = mean(a,n);
     
        // Compute sum squared 
        // differences with mean.
        float sqDiff = 0;
         
        for (int i = 0; i < n; i++) 
            sqDiff += (a[i] - m) * (a[i] - m);
         
        return (float)sqDiff / n;
    }
     
    float std(float []arr, int n) {
        return (float)Math.Sqrt(variance(arr, n));
    }
    float[] normalize(float []a) {
        int n=a.Length;
        float m = mean(a,n);
        float s = std(a,n);
        for(int i=0;i<n;i++){
            a[i]=(a[i] - m) / s;
        }
        return a;
    }
}