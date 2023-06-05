using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingManager : MonoBehaviour
{
    private Item currentItem;
    public Image costumCursor;

    public Slot[] craftingSlots;

    public List<Item> itemLists;
    public string[] recipes;
    public Item[] recipesResults;
    public Slot resultSlot;
    public List<Item> inventoryItem;
    public List<Text> inventoryItemAmount;
    public List<Text> craftingItemAmount;
    public Text craftingResultAmount;

    private void Start()
    {
        UpdateInventory();
    }

    public void UpdateInventory()
    {
        //Item mechanic
        for (int i = 0; i < 18; i++)
        {
            inventoryItem[i].gameObject.SetActive(false);
            //inventoryItem[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < GameManager.instance.inventory.GetItemList().Count; i++)
        {
            inventoryItem[i].itemId = GameManager.instance.inventory.GetItemList()[i].itemId;
            inventoryItem[i].itemName = GameManager.instance.inventory.GetItemList()[i].itemName;
            inventoryItem[i].amount = GameManager.instance.inventory.GetItemList()[i].amount;
            inventoryItemAmount[i].text = inventoryItem[i].amount.ToString();
            inventoryItem[i].GetComponent<Image>().sprite = GameManager.instance.inventory.GetItemIcon(inventoryItem[i].itemId);
            inventoryItem[i].gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        int amountItem;
        if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            if(currentItem != null)
            {
                costumCursor.gameObject.SetActive(false);
                Slot nearestSlot = null;
                float shortestDistance = float.MaxValue;

                foreach(Slot slot in craftingSlots)
                {
                    float dist = Vector2.Distance(Input.mousePosition, slot.transform.position);

                    if(dist < shortestDistance)
                    {
                        shortestDistance = dist;
                        nearestSlot = slot;
                    }
                }
                if (nearestSlot.item.itemId != 0)
                {
                    OnClickSlot(nearestSlot);
                }
                nearestSlot.gameObject.SetActive(true);
                nearestSlot.GetComponent<Image>().sprite = currentItem.GetComponent<Image>().sprite;
                nearestSlot.item.itemName = currentItem.itemName;
                nearestSlot.item.itemId = currentItem.itemId;
                amountItem = currentItem.amount;
                if (Input.GetMouseButtonUp(1))
                    if(amountItem > 1)
                        amountItem /= 2;
                nearestSlot.item.amount = amountItem;
                Debug.Log(nearestSlot.item);
                itemLists[nearestSlot.index] = nearestSlot.item;
                craftingItemAmount[nearestSlot.index].text = nearestSlot.item.amount.ToString();
                CheckForCreatedRecipes();
                currentItem = null;
                //GameManager.instance.inventory.RemoveItem(nearestSlot.item);
                GameManager.instance.inventory.RemoveItem(nearestSlot.item, amountItem);

                UpdateInventory();
            }
        }
    }

    public void CheckForCreatedRecipes()
    {
        resultSlot.gameObject.SetActive(false);
        resultSlot.item = null;

        for(int i = 0; i < 4; i++)
        {
            itemLists[i] = craftingSlots[i].item;
        }

        string currentRecipeString = "";
        foreach(Item item in itemLists)
        {
            if (item != null)
                currentRecipeString += item.itemName;
            else
                currentRecipeString += "null";
        }

        for(int i=0;i < recipes.Length; i++)
        {
            if (recipes[i] == currentRecipeString)
            {
                resultSlot.gameObject.SetActive(true);
                resultSlot.GetComponent<Image>().sprite = recipesResults[i].GetComponent<Image>().sprite;
                resultSlot.item = recipesResults[i];
            }
        }

        Debug.Log(currentRecipeString);
    }

    public void OnClickResult(Slot slot)
    {
        GameManager.instance.inventory.AddItem(slot.item);
        slot.item = null;
        for(int i = 0; i < 4; i++)
        {
            if (craftingSlots[i].item.amount > 1)
            {
                craftingSlots[i].item.amount--;
                craftingItemAmount[i].text = craftingSlots[i].item.amount.ToString();
            }
            else
            {
                itemLists[i] = null;
                craftingSlots[i].item.itemId = 0;
                craftingSlots[i].item.itemName = "";
                craftingSlots[i].item.amount = 0;
                craftingSlots[i].gameObject.SetActive(false);
            }
        }
        slot.gameObject.SetActive(false);
        UpdateInventory();
    }

    public void OnClickSlot(Slot slot)
    {
        if (slot.item.itemId == 0)
            return;
        GameManager.instance.inventory.AddItem(new Item { itemId = slot.item.itemId, itemName = slot.item.itemName, amount = slot.item.amount });
        slot.item.itemId = 0;
        slot.item.itemName = "";
        slot.item.amount = 0;
        itemLists[slot.index] = null;
        slot.gameObject.SetActive(false);
        UpdateInventory();
        CheckForCreatedRecipes();
    }

    public void OnMouseDownItem(Item item)
    {
        if(currentItem == null)
        {
            currentItem = item;
            costumCursor.gameObject.SetActive(true);
            costumCursor.sprite = currentItem.GetComponent<Image>().sprite;
        }
    }
}
