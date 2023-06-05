using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    //Public fields
    public int hitPoints = 10;
    public int maxHitpoints = 10;
    public float pushRecoverySpeed = 0.2f;

    //Immunity
    protected float immuneTime = 1f;
    protected float lastImmune;
    public SpriteRenderer objectRenderer;
    protected bool isImmune;
    protected float fadeSpeed = 1f;

    //Push
    protected Vector3 pushDirection;

    //All fighter can received damage & die

    protected virtual void Update()
    {

        /*if (Input.GetKeyDown(KeyCode.X))
        {
            if (Time.time - lastImmune > immuneTime)
            {
                lastImmune = Time.time;
                isImmune = true;
            }
            isImmune = false;
            Debug.Log("Hurt");
        }*/
        if (isImmune)
        {
            //gameObject.layer = LayerMask.NameToLayer("Immune");
            Color objectColor = objectRenderer.material.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime)*2;

            objectColor = new Color(objectColor.r, objectColor.b, objectColor.b, fadeAmount);
            objectRenderer.material.color = objectColor;

            if (objectColor.a <= 0)
            {
                objectColor = new Color(objectColor.r, objectColor.b, objectColor.b, 1f);
                objectRenderer.material.color = objectColor;
                isImmune = false;
            }

            //Debug.Log("Hurt");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Actor");
        }
    }

    protected virtual void ReceiveDamage(Damage dmg)
    {
        if (Time.time - lastImmune > immuneTime)
        {
            lastImmune = Time.time;
            hitPoints -= dmg.damageAmount;
            pushDirection = (transform.position - dmg.origin).normalized * dmg.pushForce;

            GameManager.instance.ShowText(dmg.damageAmount.ToString(), 25, Color.red, transform.position, Vector3.zero, 0.2f);

            if (hitPoints <= 0)
            {
                hitPoints = 0;
                Death();
            }
        }
    }

    protected virtual void Death()
    {

    }
}
