using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateAfterSoManyBounces : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Color currentColor;

    void Start()
    {      
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {

        currentColor = spriteRenderer.color;

        if (currentColor != null)
        {
            if (currentColor.a == 1)
            {
                currentColor.a = 0.8f;
            }
            else if (currentColor.a == 0.8f)
            {
                currentColor.a = 0.6f;
            }
            else if (currentColor.a == 0.6f)
            {
                currentColor.a = 0.4f;
            }
            else if (currentColor.a == 0.4f)
            {
                currentColor.a = 0.2f;
            }
            else if (currentColor.a == 0.2f)
            {
                currentColor.a = 0;
                GetComponent<BoxCollider2D>().enabled = false;
            }

            spriteRenderer.color = currentColor;
        }
    }
 
   
}
