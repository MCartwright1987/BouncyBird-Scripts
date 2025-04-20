using UnityEngine;

public class YellowPads : MonoBehaviour
{
    [SerializeField] GameObject[] Pads;

    public void ToggleActivatePads(bool activate)
    {
        foreach (GameObject p in Pads)
        {
            SpriteRenderer spriteRenderer;
            spriteRenderer = p.GetComponent<SpriteRenderer>();
            Color currentColor = spriteRenderer.color;

            if (activate)
            {
                currentColor.a = 1; 
                p.GetComponent<BoxCollider2D>().enabled = true;
                spriteRenderer.sortingOrder = 18;
            }
            else 
            {
                // Change this value to set different transparency levels
                currentColor.a = 0f; 
                p.GetComponent<BoxCollider2D>().enabled = false;
                spriteRenderer.sortingOrder = -3;
            }
            // Apply the modified color back to the SpriteRenderer
            spriteRenderer.color = currentColor;
        }      
    }
}
