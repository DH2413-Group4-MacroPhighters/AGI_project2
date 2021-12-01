using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using WebSocketSharp;
using UnityEngine.UI;

public class SocketServer : MonoBehaviour
{
    WebSocket ws;
    public GameObject tree;
    private GameObject objectToPlace;
    public GameObject scenePart;
    public Camera mapCamera;

    private STATE state;

    private long placeX;

    private long placeZ;

    private string clientName;

    private bool placeFlag = false;
        private void Start()
        {
            CONFIG config = readCONFIG();
            ws = new WebSocket("ws://" + config.HostIP + ":" + config.PortWs);
            ws.Connect();
            ws.Send("clear");
            ws.OnMessage += (sender, e) =>
            {
                string message = e.Data;
                Debug.Log("Message Received from "+((WebSocket)sender).Url+", Data : "+e.Data);
                switch(message)
                {
                    case "clear": break;
                    case "new-client": break;
                    default: 
                    {
                        PLACEMENT new_placement = readDATA(e.Data); 
                        handleData(new_placement);
                        break;
                    }
                }
            };

            CaptureAndSendMap(config, mapCamera);
        }
        private void Update()
        {
            if (placeFlag) {
                PlaceObject();
            }
            if(ws == null)
            {
                return;
            }if (Input.GetKeyDown(KeyCode.Space))
            {
                ws.Send("Hello from  UNITY MY LITTLE FRIEND");
            }  
        }

        //gets coordinates, object type and sets placeFlag to true.
        private void handleData(PLACEMENT new_placement) {
            switch(new_placement.type)
            {
                case "tree": 
                    objectToPlace = tree;
                    break;
                default: throw new ArgumentException("ObjectType does not exist");
            }
            placeX = new_placement.x;
            placeZ = new_placement.y;
            clientName = new_placement.clientName;
            placeFlag = true;
        }

        //called if placeFlag is true. Places tree in correct spot in the world.
        private void PlaceObject()
        {
            Debug.Log("place object");
            placeFlag = false;
            Vector3 position = new Vector3(-placeX, 18.1f, -placeZ);
            Quaternion rotation = scenePart.transform.rotation;
            GameObject new_object = Instantiate(objectToPlace, position, rotation);
            new_object.transform.Find("clientName").gameObject.GetComponent<TextMesh>().text = "Added by " + clientName;
        }

        private PLACEMENT readDATA(string json) {
            Debug.Log("reading");
            state = JsonConvert.DeserializeObject<STATE>(json);
            Debug.Log(state);
            string newID = state.newID;
            Debug.Log(newID);
            Dictionary<string,PLACEMENT> objects = state.objects;
            Debug.Log("tesst");
            PLACEMENT new_placement = objects[newID];
            return new_placement;
        }

        private CONFIG readCONFIG() {
            string json = "{}";
            using (StreamReader r = new StreamReader("../config.json"))
            {
                json = r.ReadToEnd();
            }
            CONFIG config = JsonConvert.DeserializeObject<CONFIG>(json);
            return config;
        }

        private void CaptureAndSendMap(CONFIG config, Camera c)
        {
            StartCoroutine(ImageSender.SendImageToServer("http://" + config.HostIP + ":" + config.PortServer+"/mapPost", c));
            c.enabled = false;
        }
}

public class CONFIG
{
    public int PortServer;
    public int PortWs;
    public string LocalIP;
    public string HostIP;
}

public class STATE
{
    public string newID;

    public Dictionary<string,PLACEMENT> objects;
}

public class PLACEMENT
{
    public string type;
    public long x;
    public long y;
    public string clientName;
}


