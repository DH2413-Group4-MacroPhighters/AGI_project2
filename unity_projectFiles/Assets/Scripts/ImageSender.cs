
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ImageSender : MonoBehaviour
{
    public static IEnumerator SendImageToServer(string sAdd, Camera c)
    {
        string fileDestination = Application.dataPath + "/Images/map.png";
        ImgCapture(c, fileDestination);
        //form.AddBinaryData("ímg", File.ReadAllBytes(Application.dataPath + "/Images/map.png")); 
        List < IMultipartFormSection > formData = new List < IMultipartFormSection > ();
        byte [] arr = File.ReadAllBytes (fileDestination);
        formData.Add ( new MultipartFormFileSection ( "map.png", arr ));
    
        UnityWebRequest request = UnityWebRequest.Post ( sAdd, formData );
        yield return request.SendWebRequest ();
    
        if ( request.result != UnityWebRequest.Result.Success ) {
            Debug.LogError ( $"<color=red>Upload error: {request.error}</color>" );
        } else {
            Debug.Log ( "<color=cyan>Upload successful</color>" );
        }
        
    }

    private static void ImgCapture(Camera c, string fileName)
    {
        // this code has been fetch from here, https://answers.unity.com/questions/22954/how-to-save-a-picture-take-screenshot-from-a-camer.html 
        int resWidth = 1024;
        int resHeight = 1024;
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        c.targetTexture = rt;
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        c.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        c.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = fileName;
        File.WriteAllBytes(filename, bytes);
        Debug.Log($"Took screenshot to: {filename}");
    }


}