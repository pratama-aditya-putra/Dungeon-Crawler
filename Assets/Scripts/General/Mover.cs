using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : Fighter
{

    protected BoxCollider2D boxCollider;
    protected Rigidbody2D rb;
    protected RaycastHit2D hit;
    protected Vector3 moveDelta;
    public float ySpeed = 0.75f;
    public float xSpeed = 1;

    public float sizeX = 1.0f;
    public float sizeY = 1.0f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void UpdateMotor(Vector3 input)
    {
        //Reset Move Delta
        moveDelta = new Vector3(input.x * xSpeed, input.y * ySpeed,0);

        //Rotate sprite to direction
        if (moveDelta.x > 0)
            transform.localScale = new Vector3(sizeX, sizeY, 1);
        else if (moveDelta.x < 0)
            transform.localScale = new Vector3(-sizeX, sizeY, 1);

        //Add push vector
        moveDelta += pushDirection;

        //Reduce push force based from recovery speed
        pushDirection = Vector3.Lerp(pushDirection,Vector3.zero,pushRecoverySpeed);

        rb.velocity = moveDelta;

        //Making sure we can move in this direction, by casting a box there first, if the box return null then we can move 
        /*hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(0, moveDelta.y), Mathf.Abs(moveDelta.y * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {

            //Moving sprite
            transform.Translate(0, moveDelta.y * Time.deltaTime, 0);

        }
        else
            pushDirection = Vector3.zero;
        //Making sure we can move in this direction, by casting a box there first, if the box return null then we can move 
        hit = Physics2D.BoxCast(transform.position, boxCollider.size, 0, new Vector2(moveDelta.x, 0), Mathf.Abs(moveDelta.x * Time.deltaTime), LayerMask.GetMask("Actor", "Blocking"));
        if (hit.collider == null)
        {

            //Moving sprite
            transform.Translate(moveDelta.x * Time.deltaTime, 0, 0);

        }
        else
            pushDirection = Vector3.zero;*/
    }
}
