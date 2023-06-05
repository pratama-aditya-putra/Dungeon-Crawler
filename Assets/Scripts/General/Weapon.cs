using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Collidable
{
    public bool isAlive = true;
    //Damage structure
    public bool continousAttack = false;


    public int[] damagePoint = { 1, 2, 3, 4, 5, 6, 7};
    public float[] pushForce = { 2.0f, 2.4f, 2.8f, 3.0f, 3.2f, 3.5f, 3.7f };

    //Weapon level
    public int weaponLevel = 0;
    private SpriteRenderer spriteRenderer;

    //Swing mechanic
    private Animator anim;
    private float cooldown = 0.5f;
    private float lastSwing;

    //Sound
    public static bool slash = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start(); 
        anim = GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        if (continousAttack)
        {
            if (Time.time - lastSwing > cooldown * 2)
            {
                lastSwing = Time.time;
                Swing();
                slash = true;
            }
            else slash = false;
        }
        else if (Input.GetKey(KeyCode.Mouse0) && isAlive)
        {
            if (Time.time - lastSwing > cooldown)
            {
                lastSwing = Time.time;
                Swing();
                slash = true;
            }
            else slash = false;
        }
    }

    public void SetSlashFalse()
    {
        slash = false;
    }

    protected override void OnCollide(Collider2D coll)
    {
        if (coll.name == "Collision")
            Physics2D.IgnoreCollision(boxCollider2D,coll,true);
        if(coll.tag == "Fighter")
        {
            if (coll.name == "Player")
                return;

            //Create a new damage object and then send it to the collided object
            Damage dmg = new Damage() {
                damageAmount = damagePoint[weaponLevel],
                origin = transform.position,
                pushForce = pushForce[weaponLevel]
            };

            coll.SendMessage("ReceiveDamage", dmg);
        }
    }

    private void Swing()
    {
        anim.SetTrigger("Swing");
    }

    public void UpgradeWeapon()
    {
        weaponLevel++;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];

        //Change stats

    }

    public void SetWeaponLevel(int level)
    {
        weaponLevel = level;
        spriteRenderer.sprite = GameManager.instance.weaponSprites[weaponLevel];
    }
}
