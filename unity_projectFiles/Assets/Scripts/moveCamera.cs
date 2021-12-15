using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    public GameObject world;
    
    private bool forward = true;

    private bool back = true;

    private Vector3 randomExtra = new Vector3(0,0,0);

    private int spinSpeed = 3;

    private int speedUp = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var spotAbove = world.transform.position;
        spotAbove.y += 60f;
        transform.LookAt(spotAbove + randomExtra);
        transform.Translate(Vector3.right * spinSpeed * speedUp * Time.deltaTime);
        float dist = Vector3.Distance(spotAbove + randomExtra, transform.position);
        if (dist < 70) {
            forward = false;
        }
        if (!back && dist <= 170) {
            transform.Translate(Vector3.back * 10 * speedUp * Time.deltaTime);
        }
        if (dist >= 170 && !forward) {
            if (transform.position.z > -20) {
                spinSpeed = 3;
                forward = true;
                back = true;
                randomExtra = RandomVect(new Vector3(-90,-10,90), new Vector3(90,10,90), spotAbove);
            } else {
                spinSpeed = 8;
                forward = false;
                back = false;
                randomExtra = new Vector3(0,0,0);
            }
        }
        if (forward) {
            transform.Translate(Vector3.forward * 10 * speedUp * Time.deltaTime);
        } else if (back) {
            transform.Translate(Vector3.back * 10 * speedUp * Time.deltaTime);
        }
    }

    public Vector3 RandomVect(Vector3 min, Vector3 max, Vector3 spotAbove)
    {
        float x = UnityEngine.Random.Range(min.x, max.x);
        float z = UnityEngine.Random.Range(min.x, max.x);
        float xPoint = x + spotAbove.x;
        float zPoint = z + spotAbove.z;
        float yMin = 0;
        float yMax = 0;
        if (zPoint > 22) {
            if (xPoint > 6) {
                yMin = 10;
            } else {
                yMin = -5;
            }
        } else if (zPoint > -30) {
            if (xPoint > -20) {
                yMin = 20;
            } else {
                yMin = 10;
            }
        } else if (zPoint > -75) {
            if (xPoint > 63) {
                yMin = 26;
            } else if (xPoint > 12.5) {
                yMin = 59;
            } else {
                yMin = 10;
            }
        } else {
            if (xPoint > -24) {
                yMin = 45;
            } else {
                yMin = 10;
            }
        }
        yMax = yMin + 10;
        return new Vector3(x, UnityEngine.Random.Range(yMin, yMax), z);
    }

    private void OnCollisionEnter(Collision other) {
        forward = false;
    }
}
