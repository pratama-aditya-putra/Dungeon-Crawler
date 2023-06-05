using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Mover
{

    Animator animator;
    Vector2 lookDirection = new Vector2(1, 0);
    float horizontal;
    float vertical;

    public Weapon weapon;
    public GameObject projectilePrefab;
    BoxCollider2D projectileCollider;
    private float lastLaunch;
    private float launchCooldown = 0.8f;
    private Transform parent;
    public Rigidbody2D rigidbody2d;

    private bool isHurt;
    private float hurtDuration;

    //Mana mechanic
    public float maxMana = 12;
    public float Mana = 12;
    public float manaRecovery = 0.5f;
    public float manaRecoverySpeed = 1.5f;
    public float lastManaRecovery;
    public float fireballManaCost = 3.0f;
    public float fireballSpeed;


    private ContactFilter2D filter;
    private Rigidbody2D hitBox;
    private Collider2D[] hits = new Collider2D[10];


    public bool isAlive = true;

    //sound
    [SerializeField] private AudioSource fireSoundEffect;
    [SerializeField] private AudioSource slashSoundEffect;
    [SerializeField] private AudioSource hurtSoundEffect;

    protected override void Start()
    {
        base.Start();
        animator = GetComponent<Animator>();
        hitBox = GetComponent<Rigidbody2D>();
        weapon.isAlive = true;
    }

    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive)
            return;
        base.ReceiveDamage(dmg);
        GameManager.instance.OnHitPointChange();
        if(!isImmune)
            hurtSoundEffect.Play();
        isImmune = true;
    }

    protected override void Death()
    {
        weapon.isAlive = false;
        isAlive = false;
        //Time.timeScale = 0.1f;
        GameManager.instance.deathMenuAnim.SetTrigger("Show");
        //GameManager.instance.SaveState();
    }
    public void Respawn()
    {
        Heal(maxHitpoints);
        Mana = maxMana;
        GameManager.instance.OnManaPointChange();
        isAlive = true;
        lastImmune = Time.time;
        pushDirection = Vector3.zero;
    }

    protected override void Update()
    {
        base.Update();
        /*
        hitBox.OverlapCollider(filter, hits);
        if (isImmune)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null)
                    continue;

                if (hits[i].gameObject.layer == LayerMask.NameToLayer("Enemy") /* && hits[i].name == "Player")
                {
                    Physics.IgnoreCollision(hits[i].GetComponent<Rigidbody2D>().GetComponent<Collider>(), hitBox.GetComponent<Collider>());
                }

                hits[i] = null;
            }
        }
        else
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i] == null)
                    continue;

                if (hits[i].gameObject.layer == LayerMask.NameToLayer("Enemy") /* && hits[i].name == "Player")
                {
                    Physics.IgnoreCollision(hits[i].GetComponent<Rigidbody2D>().GetComponent<Collider>(), hitBox.GetComponent<Collider>(), false);
                }

                hits[i] = null;
            }
        }
        */
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector2 move = new Vector2(horizontal, vertical);

        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }

        animator.SetFloat("Move X", lookDirection.x);
        
        if(rigidbody2d.velocity.magnitude > 0.01)
        {
            animator.SetTrigger("Walk");
            animator.ResetTrigger("Idle");
        }
        else
        {
            animator.SetTrigger("Idle");
            animator.ResetTrigger("Walk");
        }


        if (Mana < maxMana)
        {
            if (Time.time - lastManaRecovery > manaRecoverySpeed)
            {
                lastManaRecovery = Time.time;
                Mana += manaRecovery;
                if (Mana > maxMana)
                    Mana = maxMana;
                GameManager.instance.OnManaPointChange();
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && Mana > fireballManaCost)
        {
            if (Time.time - lastLaunch > launchCooldown)
            {
                OnMagicSpell(fireballManaCost);
                lastLaunch = Time.time;
                Launch();
                fireSoundEffect.Play();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            GameManager.instance.UsePotion();
        }

        if (Weapon.slash == true)
        {
            slashSoundEffect.Play();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.menu.GetComponent<CharacterMenu>().pauseButton.onClick.Invoke();
        }
    }

    protected void OnMagicSpell(float cost)
    {
        if (Mana <= 0)
            return;
        Mana -= cost;
        GameManager.instance.OnManaPointChange();
    }

    private void FixedUpdate()
    {
        if (DialogueManager.isActive == true) return;
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        if (isAlive)
            UpdateMotor(new Vector3(x, y, 0));
    }

    public void OnLevelUp()
    {
        maxHitpoints += 4;
        Heal(maxHitpoints- hitPoints);
        maxMana += 3;
        Mana = maxMana;
        GameManager.instance.OnManaPointChange();
        manaRecovery += 0.1f;
        manaRecoverySpeed -= 0.02f;
    }
    public void SetLevel(int level)
    {
        for (int i = 0; i < level; i++)
            OnLevelUp();
    }

    public void Heal(int healingAmount)
    {
        if (hitPoints == maxHitpoints)
            return;
        hitPoints += healingAmount;
        if (hitPoints > maxHitpoints)
            hitPoints = maxHitpoints;
        GameManager.instance.ShowText("+" + healingAmount.ToString() + "hp", 25, Color.green, transform.position,Vector3.up * 20, 0.5f);
        GameManager.instance.OnHitPointChange();
    }

    void Launch()
    {
        Vector2 direction;

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Camera.main.nearClipPlane;
        Vector3 Worldpos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 Worldpos2D = new Vector2(Worldpos.x, Worldpos.y);

        direction = Worldpos - transform.position;
        direction.Normalize();


        GameObject projectileObject = Instantiate(projectilePrefab, transform.position + new Vector3(direction.x, direction.y, transform.position.z) * 1f, Quaternion.identity);
        projectileObject.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("weapon");
        projectileCollider = projectileObject.GetComponent<BoxCollider2D>();
        //projectileCollider.size = new Vector2(0.2155424f, 0.08285652f);
        //projectileCollider.offset = new Vector2(0.005057238f, 0.00119327f);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.transform.Rotate(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        projectile.Launch(direction * 4, fireballSpeed);
    }

}
