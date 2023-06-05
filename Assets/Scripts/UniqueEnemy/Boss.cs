using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Boss : Enemy
{
    public float[] fireballSpeed = { 2.5f, -2.5f };
    public float distance = 0.4f;
    public Transform[] fireball;
    public GameObject door;
    public Sprite doorOpened;
    public Canvas bossHUD;
    public string setBossName;
    public Text bossName;
    public RectTransform bossHpBar;
    private bool isAlive = true;

    protected override void Start()
    {
        base.Start();
        bossHpBar.localScale = Vector3.one;
        if (PlayerPrefs.GetString("DeadEnemies").Contains(gameObject.name))
        {
            Destroy(gameObject);
            bossHpBar.localScale = Vector3.one;
            bossHUD.gameObject.SetActive(false);
            isAlive = false;
            door.layer = LayerMask.GetMask("Default");
            door.GetComponent<BoxCollider2D>().enabled = false;
            door.GetComponent<SpriteRenderer>().sprite = doorOpened;
        }
    }

    private void Update()
    {
        for(int i = 0; i < fireball.Length; i++)
        {
            fireball[i].position = transform.position + new Vector3(-Mathf.Cos(Time.time * fireballSpeed[i]) * distance, Mathf.Sin(Time.time * fireballSpeed[i]) * distance, 0);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(chasing == true)
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
        door.layer = LayerMask.GetMask("Default");
        door.GetComponent<BoxCollider2D>().enabled = false;
        door.GetComponent<SpriteRenderer>().sprite = doorOpened;
    }
}
