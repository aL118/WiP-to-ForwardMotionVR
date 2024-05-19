using System.Collections;
using System.IO;
using UnityEngine;

public class FrameCapture : MonoBehaviour
{
    public int videoWidth = 1280;
    public int videoHeight = 720;
    public int frameRate = 15;

    private bool isRecording = false;
    private string outputPath;
    private int frameCount = 0;
    private Camera captureCamera;

    void Start()
    {
        // Find the center eye camera within the OVRCameraRig
        OVRCameraRig cameraRig = FindObjectOfType<OVRCameraRig>();
        if (cameraRig != null)
        {
            captureCamera = cameraRig.centerEyeAnchor.GetComponent<Camera>();
        }

        if (captureCamera == null)
        {
            Debug.LogError("No camera found for capturing frames!");
            return;
        }

        StartRecording();
    }

    void StartRecording()
    {
        isRecording = true;
        frameCount = 0;
        outputPath = Path.Combine(Application.persistentDataPath, "RecordedFrames");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        Time.captureFramerate = frameRate;
    }

    void StopRecording()
    {
        isRecording = false;
        Time.captureFramerate = 0;
        Debug.Log("Recording stopped. Frames saved to: " + outputPath);
    }

    void Update()
    {
        if (isRecording)
        {
            StartCoroutine(CaptureFrame());
        }
    }

    IEnumerator CaptureFrame()
    {
        yield return new WaitForEndOfFrame();

        // Create a RenderTexture
        RenderTexture rt = new RenderTexture(videoWidth, videoHeight, 24);
        captureCamera.targetTexture = rt; // Assign the RenderTexture to the camera

        // Render the camera's view to the RenderTexture
        captureCamera.Render();

        // Read the RenderTexture into a Texture2D
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(videoWidth, videoHeight, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, videoWidth, videoHeight), 0, 0);
        screenShot.Apply();
        RenderTexture.active = null; // Don't forget to set it back to null
        captureCamera.targetTexture = null; // Unassign the RenderTexture

        // Save the screenshot to file
        byte[] bytes = screenShot.EncodeToPNG();
        Destroy(screenShot);

        string frameFile = Path.Combine(outputPath, $"frame{frameCount:D04}.png");
        File.WriteAllBytes(frameFile, bytes);
        frameCount++;

        // Clean up
        Destroy(rt);
    }

    void OnApplicationQuit()
    {
        if (isRecording)
        {
            StopRecording();
        }
    }
}
