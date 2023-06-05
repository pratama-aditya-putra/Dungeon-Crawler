using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTrigger : Collidable
{
    public GameObject block;
    public GameObject buttonOn;
    public GameObject buttonOff;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            block.SetActive(false);
            buttonOff.SetActive(false);
            buttonOn.SetActive(true);
        }
    }
}
