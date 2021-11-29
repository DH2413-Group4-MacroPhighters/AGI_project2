using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Serialization;
using WebSocketSharp;

public class SocketServer : MonoBehaviour
{
    WebSocket ws;
    public GameObject tree;
    public GameObject scenePart;
    public Camera mapCamera;

    private long placeX;

    private long placeZ;

    private bool placeFlag = false;
        private void Start()
        {
            CONFIG config = readCONFIG();
            ws = new WebSocket("ws://" + config.HostIP + ":" + config.PortWs);
            ws.Connect();
            ws.OnMessage += (sender, e) =>
            {
                Debug.Log("Message Received from "+((WebSocket)sender).Url+", Data : "+e.Data);
                DATA data = readDATA(e.Data);
                handleData(data);
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

        private void handleData(DATA data) {
            if (data.type == "touchstart") {
                placeX = data.x;
                placeZ = data.y;
                placeFlag = true;
            }
        }

        private void PlaceObject()
        {
            Debug.Log("place object");
            Vector3 position = scenePart.transform.Find("Middle").transform.position;
            position = position + new Vector3(-placeX/10, 0, placeZ/10);
            Quaternion rotation = scenePart.transform.rotation;
            Instantiate(tree, position, rotation);
            placeFlag = false;
        }

        private DATA readDATA(string json) {
            DATA data = JsonConvert.DeserializeObject<DATA>(json);
            return data;
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

public class DATA
{
    public string type;
    public long x;
    public long y;
    public int touchID;
}


