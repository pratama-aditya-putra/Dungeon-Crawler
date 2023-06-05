using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Collidable
{
    Rigidbody2D rigidbody2D;

    public int damagePoint = 3;
    public float pushForce =10f;
    public string target;
    private float distance = 4.0f;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        distance = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);
        if (distance > 15.0f)
        {
            Destroy(gameObject);
        }
    }

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2D.AddForce(direction * force);
    }

    protected override void OnCollide(Collider2D coll)
    {
        /*if (coll.tag == "Hitbox")
        {
            coll.GetComponent<Rigidbody>().isKinematic = true;
            coll.GetComponent<Rigidbody>().detectCollisions = false;
        }*/
        if (coll.tag != "Hitbox" && coll.name != "weapon" && coll.name != "upper")
        {
            Destroy(gameObject);
        }

        if (coll.tag == "Fighter")
        {
            if (coll.name == target)
                return;

            //Create a new damage object and then send it to the collided object
            Damage dmg = new Damage()
            {
                damageAmount = damagePoint,
                origin = transform.position,
                pushForce = pushForce
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }

    }


    /*void OnCollisionEnter2D(Collision2D other)
    {
        MusuhController enemy = other.gameObject.GetComponent<MusuhController>();

        if (enemy != null)
        {
            enemy.ChangeHealth(-1);
        }

        Debug.Log("Projectile Collision with " + other.gameObject);
        Destroy(gameObject);
    }*/
}
