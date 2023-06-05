using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamuBerasKencur : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        JakaController controller = other.GetComponent<JakaController>();

        if (controller != null)
        {
            if(controller.health < controller.maxHealth)
            {
                controller.ChangeHealth(1);
                Destroy(gameObject);
            }
        }
    }
}
