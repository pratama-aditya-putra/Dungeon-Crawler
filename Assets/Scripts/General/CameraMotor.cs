using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform lookAt;
    public float boundX = 5;
    public float boundY = 5;

    private void Start()
    {
        //transform.position = GameObject.Find("Player").transform.position;
        //transform.position = GameObject.Find("Player").transform.position;
        //Debug.Log(transform.position);
        //lookAt.position = lookAt.position - new Vector3(boundX / 4, boundY / -4, 0);
    }

    void LateUpdate()
    {
        transform.position = GameObject.Find("Player").transform.position + new Vector3(0,0,-10);
        //    Vector3 delta = Vector3.zero;

        //    //Checking the character position, is it inside camera view or not
        //    float deltaX = lookAt.position.x - transform.position.x;
        //    if (transform.position.x < lookAt.position.x)
        //    {
        //        delta.x = deltaX - boundX/2;
        //    }
        //    else
        //    {
        //        delta.x = deltaX + boundX/2;
        //    }

        //    //Checking the character position, is it inside camera view or not
        //    float deltaY = lookAt.position.y - transform.position.y;
        //    if (transform.position.y < lookAt.position.y)
        //    {
        //        delta.y = deltaY - boundY/2;
        //    }
        //    else
        //    {
        //        delta.y = deltaY + boundY/2;
        //    }

        //    transform.position += new Vector3(delta.x, delta.y, 0);
        }
}
