/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

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
    private List<GameObject> gameObjectListBar;

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        labelTemplateX = graphContainer.Find("labelTemplateX").GetComponent<RectTransform>();
        labelTemplateY = graphContainer.Find("labelTemplateY").GetComponent<RectTransform>();
        dashTemplateX = graphContainer.Find("dashTemplateX").GetComponent<RectTransform>();
        dashTemplateY = graphContainer.Find("dashTemplateY").GetComponent<RectTransform>();

        gameObjectList = new List<GameObject>();
        gameObjectListBar = new List<GameObject>();

        List<int> valueList = new List<int>() { 5, 98, 56, 45, 30, 22, 17, 15, 13, 17, 25, 37, 40, 36, 33 };
        List<int> energySourcesList = new List<int>() { 1, 2 , 1};
        ShowGraph(valueList, (int _i) => "Day " + (_i + 1) + "\n Hr " + ((_i + 1) % 24), (float _f) => "CO2: " + Mathf.RoundToInt(_f));
        CreateBar(energySourcesList, new Vector2(0f, -60f), 816f);

        FunctionPeriodic.Create(() => {
            valueList.Clear();
            for (int i = 0; i < 15; i++) {
                valueList.Add(UnityEngine.Random.Range(0, 500));
            }
            ShowGraph(valueList, (int _i) => "Day " + (_i + 1), (float _f) => "CO2 " + Mathf.RoundToInt(_f));
        }, 1f);

        FunctionPeriodic.Create(() => {
            int sources = energySourcesList.Count;
            energySourcesList.Clear();
            for (int i = 0; i < sources; i++) {
                energySourcesList.Add(UnityEngine.Random.Range(0, 500));
            }
            CreateBar(energySourcesList, new Vector2(0f, -60f), 816f );
        }, 1f);
        


    }

    private void ShowGraph(List<int> valueList, Func<int, string> getAxisLabelX = null, Func<float, string> getAxisLabelY = null) {
        if (getAxisLabelX == null) {
            getAxisLabelX = delegate (int _i) { return _i.ToString(); };
        }
        if (getAxisLabelY == null) {
            getAxisLabelY = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }

        foreach (GameObject gameObject in gameObjectList) {
            Destroy(gameObject);
        }
        gameObjectList.Clear();

        float graphHeight = graphContainer.sizeDelta.y;

        float yMaximum = valueList[0];
        float yMinimum = valueList[0];

        foreach (int value in valueList) {
            if (value > yMaximum) {
                yMaximum = value;
            }
            if (value < yMinimum) {
                yMinimum = value;
            }
        }

        yMaximum = yMaximum + ((yMaximum - yMinimum) * 0.2f);
        yMinimum = yMinimum - ((yMaximum - yMinimum) * 0.2f);

        float xSize = 50f;

        GameObject lastCircleGameObject = null;

        

        for (int i = 0; i < valueList.Count; i++) {
            float xPosition = xSize + i * xSize;
            float yPosition = ((valueList[i] - yMinimum) / (yMaximum - yMinimum)) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
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
        }

        int separatorCount = 10;
        for (int i = 0; i <= separatorCount; i++) {
            RectTransform labelY = Instantiate(labelTemplateY);
            labelY.SetParent(graphContainer, false);
            labelY.gameObject.SetActive(true);
            float normalizedValue = i * 1f / separatorCount;
            labelY.anchoredPosition = new Vector2(-7f, (normalizedValue * graphHeight) +12f);
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
        rectTransform.sizeDelta = new Vector2(11, 11);
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
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));
        return gameObject;
    }

    private void CreateBar(List<int> energySourceList, Vector2 graphPosition, float barWidth) {
        float maxWidth = barWidth;
        float previousWidth = 0;
        int sum = energySourceList.Sum();
        //int sum = energySourceList[0] + energySourceList[1];
        bool renewable = true;

        List<string> sources = new List<string>() { "Fossil", "Renewable", "Nuclear" };
        

        foreach (GameObject gameObject in gameObjectListBar)
        {
            Destroy(gameObject);
        }
        gameObjectListBar.Clear();


        for (int i = 0; i < energySourceList.Count; i++) {
            if ( previousWidth == 0) {
                GameObject gameObject = new GameObject("bar", typeof(Image));
                gameObject.transform.SetParent(graphContainer, false);
                gameObject.GetComponent<Image>().color = new Color32(188, 108, 219, 255);
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2(-15f , graphPosition.y);
                rectTransform.sizeDelta = new Vector2(barWidth * (energySourceList[i] * 1f / sum), 20f);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 1f);
                //previousWidth = barWidth * (energySourceList[i] * sum);
                previousWidth = rectTransform.sizeDelta.x;
                gameObjectListBar.Add(gameObject);

                RectTransform labelX = Instantiate(labelTemplateX);
                labelX.SetParent(graphContainer, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(-15f, graphPosition.y + 15f);
                labelX.GetComponent<Text>().text = sources[i];
                gameObjectList.Add(labelX.gameObject);

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
                rectTransform.sizeDelta = new Vector2(barWidth * (energySourceList[i] * 1f / sum), 20f);
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.pivot = new Vector2(0, 1f);
                //previousWidth = barWidth * (energySourceList[i] / sum);

                RectTransform labelX = Instantiate(labelTemplateX);
                labelX.SetParent(graphContainer, false);
                labelX.gameObject.SetActive(true);
                labelX.anchoredPosition = new Vector2(previousWidth - 13f, graphPosition.y + 15f);
                labelX.GetComponent<Text>().text = sources[i];
                gameObjectList.Add(labelX.gameObject);

                previousWidth = previousWidth + 2f + rectTransform.sizeDelta.x;
                gameObjectListBar.Add(gameObject);
                renewable = false;

               
            }

                

        }


    }

}
