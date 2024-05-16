using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
// uses testing.onnx!
public class Fly : MonoBehaviour
{
    public float flySpeed = 3f; // Adjust this to control the speed of flying
    public OVRCameraRig cameraRig;
    public float height = 4f;

    public Terrain midland1;
    public Terrain midland2;
    public Terrain midland3;
    public Terrain midland4;
    public GameObject faraway;
    public NNModel modelSource;

    int planeLength = 200;
    int initialOffset = -20;
    int sideOffset = -50;
    int cycle = 0;
    private Model model;
    private IWorker worker;
    // https://developer.oculus.com/documentation/unity/unity-passthrough-tutorial-passthrough-window/
    void Start()
    {
        model = ModelLoader.Load(modelSource);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.ComputePrecompiled, model);
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

    void Update()
    {
        // Get input from the thumbstick
        Vector2 thumbstickInput = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        // Calculate the movement direction based on the thumbstick input
        Vector3 moveDirection = new Vector3(thumbstickInput.x, 0f, thumbstickInput.y);
        moveDirection = transform.TransformDirection(moveDirection); // Transform relative to the camera's orientation
        
        // Move the camera position
        // Quaternion rotate = cameraRig.centerEyeAnchor.rotation;
        // rotate *= Quaternion.Euler(0, 90, 0);
        // transform.position += flySpeed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x, height, transform.position.z + thumbstickInput.x * flySpeed * Time.deltaTime);
    
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
        test();
    }
    void test(){
        float[] raw = new float[100];
        raw[0]=2;
        //normalize!
        var inputTensor = new Tensor(1, 1, 100, 1, raw);
        worker.Execute(inputTensor);

        var output = worker.PeekOutput();
        int pred = maxIndex(output)+1;
        Debug.Log(output[0]+" "+output[1]+" "+output[2]+" "+output[3]);
        Debug.Log("!"+pred);
        
        inputTensor.Dispose();
        output.Dispose();
    }
    void OnDestroy(){
        worker.Dispose();
    }
}