using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Mover
{
    //Experience
    public int xpValue = 1;

    //Logic
    public float triggerRange = 0.3f;
    public float chaseLength = 1.0f;
    public float attackRange = 0.3f;

    public bool chasing;
    public bool attacking;
    private bool collidingWithPlayer;
    private Transform  playerTransform;
    protected Vector3 startingPosition;
    public Animator animator = null;

    //Hitbox
    private ContactFilter2D filter;
    private BoxCollider2D hitBox;
    private Collider2D[] hits = new Collider2D[10];

    protected override void Start()
    {
        base.Start();
        //uncomment this line
        //playerTransform = GameManager.instance.player.transform;
        startingPosition = transform.position;
        hitBox = transform.GetChild(0).GetComponent<BoxCollider2D>();
        if (gameObject.GetComponent<Animator>() != null)
            animator = gameObject.GetComponent<Animator>();
        if (PlayerPrefs.GetString("DeadEnemies").Contains(gameObject.name))
            Destroy(gameObject);
    }

    protected virtual void FixedUpdate()
    {
        //Checking if position of player is inside the range
        if (Vector3.Distance(GameManager.instance.player.transform.position, startingPosition) < chaseLength)
        {
            if (Vector3.Distance(GameManager.instance.player.transform.position, startingPosition) < triggerRange)
                chasing = true;

            if (chasing == true)
            {
                if (!collidingWithPlayer)
                {
                    UpdateMotor((GameManager.instance.player.transform.position - transform.position).normalized);
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
        GameManager.instance.ShowText("+" + xpValue +  " xp",30,Color.magenta,transform.position,Vector3.up * 10, 0.5f);
    }
}
