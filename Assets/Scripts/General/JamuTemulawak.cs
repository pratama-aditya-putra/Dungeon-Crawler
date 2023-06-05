using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JamuTemulawak : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        JakaController controller = other.GetComponent<JakaController>();

        if (controller != null)
        {
            if(controller.health < controller.maxHealth)
            {
                controller.ChangeHealth(2);
                Destroy(gameObject);
            }
        }
    }
}
