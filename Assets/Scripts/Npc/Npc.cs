using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : Collidable
{
    public DialogueTrigger trigger;
    public bool buttonTrigger;
    //public Block block;
    //public bool isTalked;
    protected override void OnCollide(Collider2D coll)
    {
        if (coll.tag == "Fighter" && coll.name == "Player" && buttonTrigger)
        {
            Debug.Log("Interacted!");
            trigger.StartDialogue();
        }
    }
    /*public void Talked()
    {
        if (isTalked == true)
            block.openBlock();
    }*/

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.Space) && DialogueManager.isActive == false)
            buttonTrigger = true;
        else
            buttonTrigger = false;
        /*if (DialogueManager.isEnd == true)
            isTalked = true;
        Talked();*/
    }
}