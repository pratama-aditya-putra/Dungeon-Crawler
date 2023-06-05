using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostumCursor : MonoBehaviour
{
    Vector2 Worldpos2D;
    private void Awake()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
        Worldpos2D = new Vector2(Worldpos.x, Worldpos.y);
        transform.position = Input.mousePosition;
    }
    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }
}
