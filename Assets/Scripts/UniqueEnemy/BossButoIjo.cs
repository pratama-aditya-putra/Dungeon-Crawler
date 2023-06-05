using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossButoIjo : Enemy
{
    public float distance = 0.4f;
    //public GameObject door;
    //public Sprite doorOpened;
    public Canvas bossHUD;
    public string setBossName;
    public Text bossName;
    public RectTransform bossHpBar;
    private bool isAlive = true;
    public Animator animator;
    public GameObject portal;
    public GameObject peti;
    public GameObject trap1;
    public GameObject itemReward;

    public float playerDistance;
    protected override void Start()
    {
        base.Start();
        bossHpBar.localScale = Vector3.one;
        if (PlayerPrefs.GetString("DeadEnemies").Contains(gameObject.name))
            Destroy(gameObject);
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        playerDistance = Vector3.Distance(GameManager.instance.player.transform.position, transform.position);
        animator.SetFloat("Speed", moveDelta.magnitude);
        if (playerDistance < attackRange)
        {
            animator.SetBool("Attack", true);
        }
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
        /*if (Mathf.Approximately(transform.position.x, startingPosition.x) && Mathf.Approximately(transform.position.y, startingPosition.y))
        {
            animator.SetTrigger("Idle");
            animator.ResetTrigger("Run");
            moveDelta = Vector3.zero;
        }
        else if (!Mathf.Approximately(moveDelta.x, 0.0f) || !Mathf.Approximately(moveDelta.y, 0.0f))
        {
            animator.SetTrigger("Run");
        }*/

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
        GameManager.instance.deadEnemies += gameObject.name + "|";
        portal.gameObject.SetActive(true);
        peti.gameObject.SetActive(true);
        if(trap1 != null)
            trap1.gameObject.SetActive(false);
        itemReward.gameObject.SetActive(true);
        bossHpBar.localScale = Vector3.one;
        bossHUD.gameObject.SetActive(false);
        isAlive = false;
        //door.layer = LayerMask.GetMask("Default");
        //door.GetComponent<BoxCollider2D>().enabled = false;
        //door.GetComponent<SpriteRenderer>().sprite = doorOpened;
    }
}
