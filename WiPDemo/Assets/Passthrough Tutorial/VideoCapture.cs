using System.Collections;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
// Attach to the camera that you want to record. The recording will start on enable. Disable to stop recording, enable to resume recording.
// If you use this code, consider limiting the recording length but using a queue, or automatically disabling the component. You don't want an out-of-memory error.
[RequireComponent(typeof(Camera))]
public class VideoCapture : MonoBehaviour
{
    // We could create the render texture at runtime, but this gives you more control over the capture quality
    public RenderTexture videoRenderTexture;
    public TextureFormat saveFormat = TextureFormat.RGB24;
 
    private Camera _camera;
    private Camera recordingCamera;
 
    private List<Texture2D> imageSequence;
 
    private void Start()
    {
        _camera = GetComponent<Camera>();
        InitRecordingCamera();
        imageSequence = new List<Texture2D>();
    }
 
    // Use fixed update to keep a constant framerate for playback.
    private void FixedUpdate()
    {
        imageSequence.Add(RenderTextureToTexture2D(videoRenderTexture));
    }
 
    private void InitRecordingCamera()
    {
        if (recordingCamera != null)
            Destroy(recordingCamera.gameObject);
 
        // Create our own camera so that the fov, culling mask, etc. match the attached camera.
        GameObject go = new GameObject("Recording Camera [" + _camera.name + "]");
        go.transform.parent = transform;
        recordingCamera = go.AddComponent<Camera>();
        recordingCamera.CopyFrom(_camera);
        recordingCamera.targetTexture = videoRenderTexture;
    }
 
    private Texture2D RenderTextureToTexture2D(RenderTexture renderTexture)
    {
        Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height, saveFormat, false);
 
        RenderTexture.active = renderTexture;
        tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        tex.Apply();
        return tex;
    }
 
    public Texture2D[] GetImageSequence()
    {
        return imageSequence.ToArray();
    }
    void OnApplicationQuit()
    {
        var outputPath = Path.Combine(Application.persistentDataPath, "RecordedFrames");
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }
        var arr=GetImageSequence();
        for(int frameCount=0;frameCount<arr.Length;frameCount++){
            string frameFile = Path.Combine(outputPath, $"frame{frameCount:D04}.png");
            byte[] bytes = arr[frameCount].EncodeToPNG();
            File.WriteAllBytes(frameFile, bytes);
        }
    }
}
 
// // An example for playing back the recording. Attach to a world space canvas with a RawImage UI component.
// [RequireComponent(typeof(RawImage))]
// public class ImageSequenceViewer : MonoBehaviour
// {
//     private RawImage _image;
 
//     private int index = -1;
//     private Texture2D[] sequence;
 
//     private void Start()
//     {
//         _image = GetComponent<RawImage>();
//     }
 
//     private void FixedUpdate()
//     {
//         if (index > -1 || index < sequence.Length)
//         {
//             _image.texture = sequence[index];
//             index++;
//         }
//     }
 
//     public void PlayImageSequence(Texture2D[] sequence)
//     {
//         this.sequence = sequence;
//         index = 0;
//     }
// }