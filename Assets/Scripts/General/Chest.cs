using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : Colletible
{
    public Sprite emptyChest;
    public int pesoAmount = 10;

    protected override void Start()
    {
        base.Start();
        if (PlayerPrefs.GetString("CollectedItems").Contains(gameObject.name))
        {
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            collected = true;
        }
    }

    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            GameManager.instance.collectedItems += gameObject.name;
            GetComponent<SpriteRenderer>().sprite = emptyChest;
            GameManager.instance.peso += pesoAmount;
            GameManager.instance.ShowText("+" + pesoAmount + " Rupiah!",25,Color.yellow,transform.position,Vector3.up * 50,0.8f);
        }
    }
}
