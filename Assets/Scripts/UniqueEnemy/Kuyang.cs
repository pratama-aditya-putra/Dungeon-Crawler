using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kuyang : Mover
{
    //Experience
    public int xpValue = 1;

    //Logic
    public float triggerRange = 0.3f;
    public float chaseLength = 1.0f;

    private bool chasing;
    private bool collidingWithPlayer;
    private Transform playerTransform;
    private Vector3 startingPosition;

    //Hitbox
    private ContactFilter2D filter;
    private BoxCollider2D hitBox;
    private Collider2D[] hits = new Collider2D[10];

    public GameObject portal;
    public GameObject ct1;
    public GameObject ct2;

    protected override void Start()
    {
        base.Start();
        playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        if (PlayerPrefs.GetString("DeadEnemies").Contains(gameObject.name))
        {
            Destroy(gameObject);
            portal.SetActive(true);
            ct1.SetActive(true);
            ct2.SetActive(true);
        }
    }

    private void FixedUpdate()
    {
        //Checking if position of player is inside the range
        if (Vector3.Distance(playerTransform.position, startingPosition) < chaseLength)
        {
            if (Vector3.Distance(playerTransform.position, startingPosition) < triggerRange)
                chasing = true;

            if (chasing == true)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((playerTransform.position - transform.position).normalized);
                }
            }
            else
            {
                UpdateMotor(startingPosition - transform.position);
            }
        }
        else
        {
            UpdateMotor(startingPosition - transform.position);
            chasing = false;
        }

        //Collison mechanic
        collidingWithPlayer = false;
        hitBox.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i] == null)
                continue;

            if (hits[i].tag == "Fighter" && hits[i].name == "Player")
            {
                collidingWithPlayer = true;
            }

            hits[i] = null;
        }
    }

    protected override void Death()
    {
        Destroy(gameObject);
        GameManager.instance.deadEnemies += gameObject.name + "|";
        GameManager.instance.experience += xpValue;
        GameManager.instance.ShowText("+" + xpValue + " xp", 30, Color.magenta, transform.position, Vector3.up * 10, 0.5f);
        portal.SetActive(true);
        ct1.SetActive(true);
        ct2.SetActive(true);
    }
}
