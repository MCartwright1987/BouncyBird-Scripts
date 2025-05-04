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
                currentColor.a = 0.75f;
            }
            else if (currentColor.a == 0.75f)
            {
                currentColor.a = 0.5f;
            }
            else if (currentColor.a == 0.5f)
            {
                currentColor.a = 0.25f;
            }
            else if (currentColor.a == 0.25f)
            {
                currentColor.a = 0;
                GetComponent<BoxCollider2D>().enabled = false;
            }

            spriteRenderer.color = currentColor;
        }
    }
 
   
}
