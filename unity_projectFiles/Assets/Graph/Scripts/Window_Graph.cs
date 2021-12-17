

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class Window_Graph : MonoBehaviour {

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;
    private RectTransform labelTemplateX;
    private RectTransform labelTemplateY;
    private RectTransform dashTemplateX;
    private RectTransform dashTemplateY;
    private List<GameObject> gameObjectList;
    private List<GameObject> gameObjectListBarchart;
    private List<int> energySourcesList;
    private List<int> emissionsList;

    private void Awake() {

        //find references to objects in scene
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();
        RenderSettings.fog = true;
        //stores copy of gameobjects created
        gameObjectList = new List<GameObject>();
        gameObjectListBarchart = new List<GameObject>();

        RenderSettings.fogDensity = 0.001f;

        List<int> emissionsList = new List<int>() { 0 };
        energySourcesList = new List<int>() { 1, 1, 1 };

        //ShowGraph(valueList, (int _i) => "Day " + (_i + 1) + "\n Hr " + ((_i + 1) % 24), (float _f) => "CO2: " + Mathf.RoundToInt(_f));
        CreateBar(energySourcesList, new Vector2(0f, -36f), 310f);

        

        FunctionPeriodic.Create(() => {            

            int emissions = calcEmissions();
            emissionsList.Add(emissions);
            ShowGraph(emissionsList, (int _i) => "D " + (_i + 1), (float _f) => "CO2 " + Mathf.RoundToInt(_f));
            
        }, 10f);

        FunctionPeriodic.Create(() => {
            //int sources = energySourcesList.Count;
            //energySourcesList.Clear();
            //for (int i = 0; i < sources; i++) {
            //    energySourcesList.Add(UnityEngine.Random.Range(0, 500));
            //}
            int fossil = GameObject.FindGameObjectsWithTag("coalPlant").Length;
            float coalPlantIncrease = 0.0007f;
            float totalFog = fossil * coalPlantIncrease;
            if (fossil > 0)
                
            {
                
                
                

                // And enable fog
                RenderSettings.fog = true;

                // Set the fog amount 
                RenderSettings.fogDensity = totalFog;
            
            }
            updateBar();
            CreateBar(energySourcesList, new Vector2(0f, -36f), 310f);
            /*
            int sumEnergySourcesList = energySourcesList[0] + energySourcesList[1] + energySourcesList[2];
            Debug.Log(energySourcesList[0]);
            float fogController = energySourcesList[0] * 1f / sumEnergySourcesList;
            Debug.Log(fogController);
            */
            //RenderSettings.fogDensity = 0.1f * (energySourcesList[0] / sumEnergySourcesList * 1f) ;
            
        }, 1f);
        


    }


    public int calcEmissions()
    {
        int fossil = GameObject.FindGameObjectsWithTag("coalPlant").Length;
        int wind = GameObject.FindGameObjectsWithTag("windPlant").Length;
        int solar = GameObject.FindGameObjectsWithTag("solarPlant").Length;
        int nuclear = GameObject.FindGameObjectsWithTag("nuclearPlant").Length;
        int emissions = fossil * 10 + wind * 1 + solar * 1 + nuclear * 2;
        return emissions;
        
    }

    // Detects gameobjects and adds them to the energy bar
    public void updateBar()
    {
        int fossil = GameObject.FindGameObjectsWithTag("coalPlant").Length;
        int renewable = GameObject.FindGameObjectsWithTag("windPlant").Length + GameObject.FindGameObjectsWithTag("solarPlant").Length;
        int nuclear = GameObject.FindGameObjectsWithTag("nuclearPlant").Length;
        List<int> objectList = new List<int>() {fossil, renewable, nuclear };
        
        int i = 0;

        for (i = 0; i < energySourcesList.Count(); i++)

            if ( objectList[i] > energySourcesList[i])
            {
                energySourcesList[i] += objectList[i] - energySourcesList[i]; 
            }
    }
    
    //Function that creates graph and inserts circle markers 
    private void ShowGraph(List<int> valueList, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null) {
        if (getAxisLabelX == null) {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null) {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }
        
        foreach (GameObject gameObject in gameObjectList) { // to avoid creating doubles of gameobject 
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        GameObject lastCircleGameObject = null;
        float graphHeight = graphContainer.sizeDelta.y;
        float xSize = 15f; // space between two values on x axis
        
        
        float yMaximum = valueList[0];
        float yMinimum = valueList[0];
        int maxVisibleValueAmount = 19; // decides number of shown points in graph

        for (int i = Mathf.Max(valueList.Count() - maxVisibleValueAmount, 0) ; i < valueList.Count; i++) {
            int value = valueList[i];
            if (value > yMaximum) {
                yMaximum = value;
            }
            if (value < yMinimum) {
                yMinimum = value;
            }
        }

        float yDifference = yMaximum - yMinimum;
        if (yDifference <= 0)
        {
            yDifference = 5f;
        }

        yMaximum = yMaximum + ((yDifference) * 0.2f); // margins for y axis 
                                                              //yMinimum = yMinimum - ((yMaximum - yMinimum) * 0.2f); // margins for y axis
       
        int xIndex = 0;
        
        for (int i = Mathf.Max(valueList.Count() - maxVisibleValueAmount, 0) ; i < valueList.Count; i++) {
            float xPosition = xSize + xIndex * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject circleGameObject = null;
            if (valueList[i] > 0)
            {
                circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            }

            gameObjectList.Add(circleGameObject);
            if (lastCircleGameObject != null) {
                GameObject dotConnectionGameObject = CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
                gameObjectList.Add(dotConnectionGameObject);
            }
            lastCircleGameObject = circleGameObject;

            RectTransform labelX = Instantiate(labelTemplateX);
            labelX.SetParent(graphContainer, false);
            labelX.gameObject.SetActive(true);
            labelX.anchoredPosition = new Vector2(xPosition, -12f);
            labelX.GetComponent<Text>().text = getAxisLabelX(i);
            gameObjectList.Add(labelX.gameObject);

            RectTransform dashX = Instantiate(dashTemplateX);
            dashX.SetParent(graphContainer, false);
            dashX.gameObject.SetActive(true);
            dashX.anchoredPosition = new Vector2(xPosition, -3f);
            gameObjectList.Add(dashX.gameObject);
            xIndex++;
        }

        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++) {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-20f, (normalizedValue * graphHeight) + 5f);
            labelY.GetComponent<Text>().text = getAxisLabelY(yMinimum + (normalizedValue * (yMaximum - yMinimum)));
            gameObjectList.Add(labelY.gameObject);

            RectTransform dashY = Instantiate(dashTemplateY);
            dashY.SetParent(graphContainer, false);
            dashY.gameObject.SetActive(true);
            dashY.anchoredPosition = new Vector2(-4f, normalizedValue * graphHeight);
            gameObjectList.Add(dashY.gameObject);
            
        }
        
    }

    private GameObject CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(5, 5);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1, 1, 1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 1f); // thickness of line between points
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
        return gameObject;
    }

    private void CreateBar(List<int> energySourceList, Vector2 graphPosition, float barWidth) {
        float maxWidth = barWidth;
        float previousWidth = 0;
        int sum = energySourceList.Sum();
        bool renewable = true;

        if (energySourceList[0] / sum > 0.5)
        {
            
        }

        List<string> sources = new List<string>() { "Fossil", "Renewable", "Nuclear" };

        foreach (GameObject gameObject in gameObjectListBarchart)
        {
            Destroy(gameObject);
        }
        gameObjectListBarchart.Clear();


        for (int i = 0; i < energySourceList.Count; i++) {
            if ( previousWidth == 0) {
                GameObject gameObject = new GameObject("bar", typeof(Image));
                gameObject.transform.SetParent(graphContainer, false);
                gameObject.GetComponent<Image>().color = new Color32(188, 108, 219, 255);
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(-15f , graphPosition.y);
                rectTransform.sizeDelta = new Vector2(barWidth * 0.1f, 10f);

                if (energySourceList[i] > 0)
                {
                    rectTransform.sizeDelta = new Vector2(barWidth * (energySourceList[i] * 1f / sum), 10f);
                }
                
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 1f);
                //previousWidth = barWidth * (energySourceList[i] * sum);
                previousWidth = rectTransform.sizeDelta.x;
                gameObjectListBarchart.Add(gameObject);

                RectTransform labelX = Instantiate(labelTemplateX);
                labelX.SetParent(graphContainer, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(-15f, graphPosition.y + 8f);
                labelX.GetComponent<Text>().fontSize = 7;
                if (rectTransform.sizeDelta.x < 20f)
                {
                    labelX.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
                }
                    
                
                labelX.GetComponent<Text>().text = sources[i];
                gameObjectListBarchart.Add(labelX.gameObject);

            }
            else {
                GameObject gameObject = new GameObject("bar", typeof(Image));
                gameObject.transform.SetParent(graphContainer, false);
                if (renewable == true) {
                    gameObject.GetComponent<Image>().color = new Color32(118, 219, 108, 255);
                renewable = false;
                        }

                else if (renewable == false)
                {
                    gameObject.GetComponent<Image>().color = new Color32(206, 255, 108, 255);
                }
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                    //rectTransform.anchoredPosition = new Vector2(graphContainer.sizeDelta.x / 2f, graphPosition.y);
                rectTransform.anchoredPosition = new Vector2(previousWidth - 13f, graphPosition.y);
                rectTransform.sizeDelta = new Vector2(barWidth * 0.1f, 10f);
                if (energySourceList[i] > 0) ;
                {
                    rectTransform.sizeDelta = new Vector2(barWidth * (energySourceList[i] * 1f / sum), 10f);
                }
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 1f);
                //previousWidth = barWidth * (energySourceList[i] / sum);

                RectTransform labelX = Instantiate(labelTemplateX);
                labelX.SetParent(graphContainer, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(previousWidth - 13f, graphPosition.y + 8f );
                labelX.GetComponent<Text>().fontSize = 7;
                labelX.GetComponent<Text>().text = sources[i];
                if (rectTransform.sizeDelta.x < 20f)
                {
                    labelX.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Wrap;
                }
                gameObjectListBarchart.Add(labelX.gameObject);

                previousWidth = previousWidth + 2f + rectTransform.sizeDelta.x;
                gameObjectListBarchart.Add(gameObject);
                renewable = false;

               
            }

                

        }


    }

}
