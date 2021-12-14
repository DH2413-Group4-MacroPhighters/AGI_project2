using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WebSocketSharp;
using System.Text.RegularExpressions;

public class SocketServer : MonoBehaviour
{
    WebSocket ws;
    public GameObject tree;
    public GameObject bush;
    public GameObject rock;
    public GameObject pineTree;
    public GameObject coal;
    public GameObject nuclear;
    public GameObject solar;
    public GameObject windTurbine;
    private GameObject objectToPlace;
    public GameObject scenePart;
    public Camera mapCamera;

    private STATE state;

    private Dictionary<string, PLACEMENT> placement_dict = new Dictionary<string, PLACEMENT>();

    [SerializeField]
    private Dictionary<string, GameObject> placed_objects = new Dictionary<string, GameObject>();

    private long placeX;

    private long placeZ;

    private string clientName;

    private string removeID = "";

    private bool removeFlag = false;

    private bool placeFlag = false;
        private void Start()
        {
            CONFIG config = ReadCONFIG();
            ws = new WebSocket("ws://" + config.HostIP + ":" + config.PortWs);
            ws.Connect();
            ws.Send("clear ");
            ws.OnMessage += (sender, e) =>
            {
                string message = e.Data;
                Debug.Log("Message Received from "+((WebSocket)sender).Url+", Data : "+e.Data);
                switch(message)
                {
                    case "clear ": break;
                    case "new-client ": break;
                    case var str when new Regex("^remove.*$").IsMatch(str):
                    {
                        var str_array = str.Split(' ');
                        removeID = str_array[1];
                        removeFlag = true;
                        break;
                    }
                    default: 
                    {
                        ReadDATAaddPlacement(e.Data);
                        placeFlag = true;
                        break;
                    }
                }
            };

            CaptureAndSendMap(config, mapCamera);
        }
        private void Update()
        {
            if (placeFlag) {
                PlaceObjects();
            }
            if (removeFlag) {
                Remove(removeID);
            }
            if(ws == null)
            {
                return;
            }
        }

        //called if placeFlag is true. Places tree in correct spot in the world.
        private void PlaceObjects()
        {
            foreach (KeyValuePair<string, PLACEMENT> kvp in placement_dict) {
                string ID = kvp.Key;
                PLACEMENT placement = kvp.Value;
                placeFlag = false;
                GameObject objectToPlace;
                switch(placement.type)
                {
                    case "tree": 
                        objectToPlace = tree;
                        break;
                    case "bush": 
                        objectToPlace = bush;
                        break;
                    case "rock": 
                        objectToPlace = rock;
                        break;
                    case "pineTree": 
                        objectToPlace = pineTree;
                        break;
                    case "coal": 
                        objectToPlace = coal;
                        break;
                    case "nuclear": 
                        objectToPlace = nuclear;
                        break;
                    case "solar": 
                        objectToPlace = solar;
                        break;
                    case "wind": 
                        objectToPlace = windTurbine;
                        break;
                    default: throw new ArgumentException("ObjectType does not exist");
                }
                float height = 75.1f;
                if (placement.y > 35) {
                    height = 109f;
                }
                if (placement.y < -13) {
                    height = 59.1f;
                }
                Debug.Log("place object");
                Vector3 position = new Vector3(-placement.x, height, -placement.y);
                Quaternion rotation = scenePart.transform.rotation;
                GameObject new_object = Instantiate(objectToPlace, position, rotation);
                new_object.transform.Find("clientName").gameObject.GetComponent<TextMesh>().text = "Added by " + placement.clientName;
                new_object.name = ID;
                placed_objects.Add(ID, new_object);
            }
            placement_dict.Clear();
        }

        private void ReadDATAaddPlacement(string json) {
            state = JsonConvert.DeserializeObject<STATE>(json);
            string newID = state.newID;
            Dictionary<string,PLACEMENT> objects = state.objects;
            PLACEMENT new_placement = objects[newID];
            placement_dict.Add(newID,new_placement);
        }

        private CONFIG ReadCONFIG() {
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
            GameObject[] clouds = GameObject.FindGameObjectsWithTag("cloud"); 
            foreach (GameObject cloud in clouds)
            {
                cloud.SetActive(false);
                        
            }
            StartCoroutine(ImageSender.SendImageToServer("http://" + config.HostIP + ":" + config.PortServer+"/mapPost", c));
            c.enabled = false;
            foreach (GameObject cloud in clouds)
            {
                cloud.SetActive(true);
                        
            }
        }

        private void Remove(string id) {
            removeID = "";
            removeFlag = false;
            GameObject objToRemove = placed_objects[id];
            placed_objects.Remove(id);
            Destroy(objToRemove);
            Debug.Log("Destroyed: " + id);
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


