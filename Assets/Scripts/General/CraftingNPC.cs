using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingNPC : Collidable
{
    private bool buttonTriggerShow;
    private bool buttonTriggerHide;
    public Animator craftingAnim;
    public CraftingManager managerCrafting;
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player" && buttonTriggerShow)
        {
            managerCrafting.UpdateInventory();
            craftingAnim.SetTrigger("Show");
        }
        if (coll.tag == "Fighter" && coll.name == "Player" && buttonTriggerHide)
        {
            Debug.Log("Hide");
            craftingAnim.SetTrigger("Hide");
        }
    }
    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space))
            buttonTriggerShow = true;
        else if (Input.GetKeyDown(KeyCode.Escape))
            buttonTriggerHide = true;
        else
        {
            buttonTriggerShow = false;
            buttonTriggerHide = false;
        }
        /*if (DialogueManager.isEnd == true)
            isTalked = true;
        Talked();*/
    }
}
