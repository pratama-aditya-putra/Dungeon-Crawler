using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleItem : Colletible
{
    public Item item;

    protected override void Start()
    {
        base.Start();
        if (PlayerPrefs.GetString("CollectedItems").Contains(gameObject.name))
            Destroy(gameObject);
    }

    protected override void OnCollect()
    {
        if (!collected)
        {
            collected = true;
            item.amount = 1;
            GameManager.instance.collectedItems += gameObject.name;
            GameManager.instance.inventory.AddItem(item);
            GameManager.instance.ShowText("+ " + item.itemName, 25, Color.yellow, transform.position, Vector3.up * 30, 0.8f);
            Debug.Log(item.amount);
            Destroy(gameObject);
        }
    }
}
