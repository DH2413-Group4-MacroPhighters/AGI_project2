using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WebSocketSharp;

public class SocketServer : MonoBehaviour
{
    WebSocket ws;
        private void Start()
        {
            CONFIG config = readCONFIG();
            ws = new WebSocket("ws://" + config.HostIP + ":" + config.PortWs);
            ws.Connect();
            ws.OnMessage += (sender, e) =>
            {
                Debug.Log("Message Received from "+((WebSocket)sender).Url+", Data : "+e.Data);
            };
        }
        private void Update()
        {
            if(ws == null)
            {
                return;
            }if (Input.GetKeyDown(KeyCode.Space))
            {
                ws.Send("Hello from  UNITY MY LITTLE FRIEND");
            }  
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
}

public class CONFIG
{
    public int PortServer;
    public int PortWs;
    public string LocalIP;
    public string HostIP;
}
