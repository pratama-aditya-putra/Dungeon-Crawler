using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NyiBlorong : Enemy
{
    public Canvas bossHUD;
    public string setBossName;
    public Text bossName;
    public RectTransform bossHpBar;
    private bool isAlive = true;

    public GameObject portal;
    public GameObject block;

    protected override void Start()
    {
        base.Start();
        bossHpBar.localScale = Vector3.one;
        if (PlayerPrefs.GetString("DeadEnemies").Contains(gameObject.name))
        {
            Destroy(gameObject); bossHpBar.localScale = Vector3.one;
            bossHUD.gameObject.SetActive(false);
            isAlive = false;
            portal.SetActive(true);
            block.SetActive(false);
        }
    }
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
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
        block.SetActive(false);
    }
}
