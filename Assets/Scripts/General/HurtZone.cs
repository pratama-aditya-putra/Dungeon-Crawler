using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtZone : Collidable
{
    /*void OnTriggerStay2D(Collider2D other)
    {
        JakaController controller = other.GetComponent<JakaController>();

        if (controller != null)
        {
            controller.ChangeHealth(-1);
        }
    }*/

    public GameObject chest;

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player")
        {
            chest.SetActive(true);
        }
    }
}
