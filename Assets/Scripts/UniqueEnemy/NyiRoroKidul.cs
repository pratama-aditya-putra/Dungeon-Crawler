using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NyiRoroKidul : Enemy
{
    public GameObject projectilePrefab;
    public GameObject center;
    BoxCollider2D projectileCollider;
    private float lastLaunch;
    private float launchCooldown = 2.5f;

    public Canvas bossHUD;
    public string setBossName;
    public Text bossName;
    public RectTransform bossHpBar;
    private bool isAlive = true;
    private float playerDistance;
    private Animator animator;
    public float tempx;
    public float tempy;

    public GameObject portal;

    protected override void Start()
    {
        base.Start();
        animator = gameObject.GetComponent<Animator>();
        if (PlayerPrefs.GetString("DeadEnemies").Contains(gameObject.name))
        {
            bossHpBar.localScale = Vector3.one;
            bossHUD.gameObject.SetActive(false);
            isAlive = false;
            portal.SetActive(true);
        }
    }

    private void Update()
    {
        if (chasing == true)
        {
            if (Time.time - lastLaunch > launchCooldown)
            {
                lastLaunch = Time.time;
                Launch();
            }
        }
        center.transform.position = transform.position + new Vector3(tempx, -tempy, 0);
    }


    void Launch()
    {
        animator.SetTrigger("Launch");
        Vector2 direction;

        direction = (transform.position - GameManager.instance.player.transform.position) * -1;
        direction.Normalize();

        GameObject projectileObject = Instantiate(projectilePrefab, center.transform.position + new Vector3(direction.x, direction.y, 0) * 0.3f, Quaternion.identity);
        projectileObject.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("weapon");
        projectileCollider = projectileObject.GetComponent<BoxCollider2D>();
        projectileCollider.size = new Vector2(0.2155424f, 0.08285652f);
        projectileCollider.offset = new Vector2(0.005057238f, 0.00119327f);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.transform.Rotate(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
        projectile.Launch(direction * 4, 20);
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        playerDistance = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);
        animator.SetFloat("Speed", moveDelta.magnitude);
        if (playerDistance < attackRange)
            animator.SetBool("Attack", true);
        else
            animator.SetBool("Attack", false);
        if (chasing == true)
        {
            bossHUD.gameObject.SetActive(true);
            bossName.text = setBossName;
        }
        else
        {
            bossHUD.gameObject.SetActive(false);
        }
    }

    protected override void ReceiveDamage(Damage dmg)
    {
        if (!isAlive)
            return;

        base.ReceiveDamage(dmg);
        float ratio = (float)hitPoints / (float)maxHitpoints;
        bossHpBar.localScale = new Vector3(ratio, 1, 1);
    }

    protected override void Death()
    {
        base.Death();
        bossHpBar.localScale = Vector3.one;
        bossHUD.gameObject.SetActive(false);
        isAlive = false;
        portal.SetActive(true);
        //block.SetActive(false);
    }
}
