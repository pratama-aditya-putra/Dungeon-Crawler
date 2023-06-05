using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMenu : MonoBehaviour
{
    //Text fields
    public Text levelText, hitpointText, pesosText, upgradeCost, xpText, manapointText;

    //Logic field
    private int currentCharacterSelection = 0;
    public Image characterSelectionSPrite;
    public Image weaponSprite;
    public RectTransform xpBar;
    public List<Item> inventoryItem;
    public List<Text> inventoryItemAmount;
    private Item currentItem;
    public Image costumCursor;
    public Slot potionSlots;
    public Text potionAmount;
    public Button pauseButton;

    private void Start()
    {
        UpdateMenu();
        GameManager.instance.inventory.potion = potionSlots.item;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (currentItem != null)
            {
                costumCursor.gameObject.SetActive(false);
                if(currentItem.itemId > 2000)
                {
                    SetPotion(currentItem, true);
                    UpdateMenu();
                }
            }
            currentItem = null;
        }
    }

    //True only if the item came from inventory to potion slot
    //False if item came from other object such as game manager
    public void SetPotion(Item myItem, bool Remov)
    {
        Slot nearestSlot = null;
        nearestSlot = potionSlots;
        nearestSlot.item.gameObject.SetActive(true);
        nearestSlot.item.GetComponent<Image>().sprite = GameManager.instance.inventory.GetItemIcon(myItem.itemId);
        if (nearestSlot.item.itemId != 0)
        {
            if (nearestSlot.item.itemId == myItem.itemId)
                nearestSlot.item.amount += myItem.amount;
        }
        else if (nearestSlot.item.itemId == 0)
        {
            nearestSlot.item.itemId = myItem.itemId;
            nearestSlot.item.itemName = myItem.itemName;
            nearestSlot.item.amount = myItem.amount;
        }
        potionAmount.text = nearestSlot.item.amount.ToString();

        if(Remov)
            GameManager.instance.inventory.RemoveItem(nearestSlot.item, myItem.amount);
    }

    public void OnClickSlot(Slot slot)
    {
        if (slot.item.itemId == 0)
            return;
        GameManager.instance.inventory.AddItem(new Item { itemId = slot.item.itemId, itemName = slot.item.itemName, amount = slot.item.amount });
        slot.item.itemId = 0;
        slot.item.itemName = "";
        slot.item.amount = 0;
        slot.item.gameObject.SetActive(false);
        UpdateMenu();
    }

    public void OnMouseDownItem(Item item)
    {
        if (currentItem == null)
        {
            currentItem = item;
            costumCursor.gameObject.SetActive(true);
            costumCursor.sprite = currentItem.GetComponent<Image>().sprite;
        }
    }

    //Weapon Upgrade 
    public void OnClickUpgrade()
    {
        if(GameManager.instance.TryUpdateWeapon())
        {
            UpdateMenu();
        }
    }

    //Update character information
    public void UpdateMenu()
    {
        //Item mechanic
        for (int i = 0; i < 18; i++)
        {
            inventoryItem[i].gameObject.SetActive(false);
        }
        //Debug.Log(GameManager.instance.inventory.GetItemList().Count);
        for (int i = 0; i < GameManager.instance.inventory.GetItemList().Count; i++)
        {
            inventoryItem[i].itemId = GameManager.instance.inventory.GetItemList()[i].itemId;
            inventoryItem[i].itemName = GameManager.instance.inventory.GetItemList()[i].itemName;
            inventoryItem[i].amount = GameManager.instance.inventory.GetItemList()[i].amount;
            inventoryItemAmount[i].text = inventoryItem[i].amount.ToString();
            inventoryItem[i].GetComponent<Image>().sprite = GameManager.instance.inventory.GetItemIcon(inventoryItem[i].itemId);
            inventoryItem[i].gameObject.SetActive(true);
        }

        /*if(GameManager.instance.potion != null)
        {
            if(GameManager.instance.potion.itemId != 0)
                potionSlots.item.amount = GameManager.instance.potion.amount;
        }
        else
        {
            potionSlots.item.itemId = 0;
            potionSlots.item.amount = 0;
            potionSlots.item.itemName = "";
            potionSlots.gameObject.SetActive(false);
        }*/

        //Weapon
        weaponSprite.sprite = GameManager.instance.weaponSprites[GameManager.instance.weapon.weaponLevel];
        if (GameManager.instance.weapon.weaponLevel == GameManager.instance.weaponPrices.Count)
            upgradeCost.text = "MAX";
        else
            upgradeCost.text = GameManager.instance.weaponPrices[GameManager.instance.weapon.weaponLevel].ToString();

        //Meta
        levelText.text = GameManager.instance.GetCurrentLevel().ToString();
        hitpointText.text = GameManager.instance.player.hitPoints.ToString() + " / " + GameManager.instance.player.maxHitpoints.ToString();
        manapointText.text = Mathf.Floor(GameManager.instance.player.Mana).ToString() + " / " + GameManager.instance.player.maxMana.ToString();
        pesosText.text = GameManager.instance.peso.ToString();

        //Xp bar
        int currLevel = GameManager.instance.GetCurrentLevel();
        if (currLevel == GameManager.instance.expTable.Count)
        {
            xpText.text = GameManager.instance.experience.ToString() + " Total experience points!"; // Display total xp
            xpBar.localScale = Vector3.one;
        }
        else
        {   
            int prevLevelXp = GameManager.instance.GetXptoLevel(currLevel - 1);
            int currLevelXp = GameManager.instance.GetXptoLevel(currLevel);

            int diff = currLevelXp - prevLevelXp;
            int currXpIntoLevel = GameManager.instance.experience - prevLevelXp;

            float completionRatio = (float)currXpIntoLevel / (float)diff;
            xpBar.localScale = new Vector3(completionRatio, 1, 1);
            xpText.text = currXpIntoLevel + " / " + diff;
        }
    }
}
