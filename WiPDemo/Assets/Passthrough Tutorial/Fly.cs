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
    public GameObject hole;
    public Vector3 defaultScale = new Vector3(.1f,.1f,.2f);

    int planeLength = 200;
    int initialOffset = -20;
    int sideOffset = -50;
    int cycle = 0;
    private ArrayList pastWindow = new ArrayList();
    private Model model;
    private IWorker worker;

    // https://developer.oculus.com/documentation/unity/unity-passthrough-tutorial-passthrough-window/
    void Start()
    {
        model = ModelLoader.Load(modelSource);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
    }

    void Update()
    {
        float speed = flySpeed*(float)predictSpeed();
        messageText.SetText("Speed: "+speed);
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
        }
        faraway.transform.position = new Vector3(sideOffset,0,transform.position.z+2*planeLength+initialOffset);
    
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            if(hole.transform.localScale.x==1f){
                hole.transform.localScale=defaultScale;
            }else{
                hole.transform.localScale = new Vector3(1,1,1);
            }
        }
    }
    int predictSpeed(){
        var headPosition = head.transform.position;
        statText.SetText("Head.y: "+headPosition.y.ToString());
        int window=100;
        if(pastWindow.Count<window){
            pastWindow.Add(headPosition.y);
        }else{
            pastWindow.RemoveAt(0);
            pastWindow.Add(headPosition.y);
        }
        int speed=0;
        if(pastWindow.Count==window){      

            float[] raw = (float[])pastWindow.ToArray(typeof(float));
            normalize(raw);

            var inputTensor = new Tensor(1, 1, 100, 1, raw);
            worker.Execute(inputTensor);

            var output = worker.PeekOutput();
            int pred = maxIndex(output)+1;
            speed=pred;
            
            inputTensor.Dispose();
            output.Dispose();
        }
        return speed;
    }
    void OnDestroy(){
        worker.Dispose();
    }
    float[] maxmin(float []a, int n) {
        float max=a[0];
        float min=a[0];   
        for (int i = 0; i < n; i++){
            if(a[i]<min){
                min=a[i];
            }
            if(a[i]>max){
                max=a[i];
            }
        }
        return new float[]{max, min};
    }
    int maxIndex(Tensor t){
        float max=t[0];
        int idx=0;
        for(int i=1;i<4;i++){
            if(t[i]>max){
                max=t[i];
                idx=i;
            }
        }
        return idx;
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