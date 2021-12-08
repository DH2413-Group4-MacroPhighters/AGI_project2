using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class barchart : MonoBehaviour
{

    private RectTransform bar;
    private RectTransform renewableBar;
    private RectTransform graphContainer;

    // Start is called before the first frame update
    void Awake()
    {
        bar = transform.Find("barchart").GetComponent<RectTransform>();
        renewableBar = bar.Find("renewable").GetComponent<RectTransform>();
        graphContainer= transform.Find("graphContainer").GetComponent<RectTransform>();
        /*
        float graphWidth = graphContainer.sizeDelta.x;


        RectTransform renewableBar = Instantiate(renewableBar);
        renewableBar.SetParent(barchart, false);
        renewableBar.gameObject.SetActive(true);
        //renewableBar.anchoredPosition = new Vector2(10f, -7f);
        renewableBar.sizeDelta = new Vector2(graphWidth, 100);

        //renewableBar.GetComponent<Text>().text = getAxisLabelX(i);
        //gameObjectList.Add(labelX.gameObject);
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // change size of renewable bar to half size of graphContainer
    // Change size of other bar to other half of graphcontainer
    // make function that normalizes sizes of bar so always same width
    // but can have 
}
