using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : Collidable
{
    public GameObject door;
    public Sprite doorClosed;
    protected override void OnCollide(Collider2D coll)
    {
        if(coll.tag == "Fighter")
        {
            if(coll.name == "Player")
            {
                door.layer = LayerMask.GetMask("Default");
                door.GetComponent<BoxCollider2D>().enabled = true;
                door.GetComponent<SpriteRenderer>().sprite = doorClosed;
                Destroy(gameObject);
            }
        }
    }
}
