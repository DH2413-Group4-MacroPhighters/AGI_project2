
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class ImageSender : MonoBehaviour
{
    public static IEnumerator SendImageToServer(string sAdd)
    {
        //form.AddBinaryData("ímg", File.ReadAllBytes(Application.dataPath + "/Images/map.png")); 
        List < IMultipartFormSection > formData = new List < IMultipartFormSection > ();
        byte [] arr = File.ReadAllBytes (Application.dataPath + "/Images/map.png");
        formData.Add ( new MultipartFormFileSection ( "map.png", arr ));
    
        UnityWebRequest request = UnityWebRequest.Post ( sAdd, formData );
        yield return request.SendWebRequest ();
    
        if ( request.result != UnityWebRequest.Result.Success ) {
            Debug.LogError ( $"<color=red>Upload error: {request.error}</color>" );
        } else {
            Debug.Log ( "<color=cyan>Upload successful</color>" );
        }
        
    }
    
    
}