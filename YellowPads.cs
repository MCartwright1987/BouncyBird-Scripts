using UnityEngine;

public class YellowPads : MonoBehaviour
{
    [SerializeField] GameObject[] Pads;

    public void ToggleActivatePads(bool activate)
    {
        foreach (var p in Pads)
        {
            SpriteRenderer spriteRenderer;
            spriteRenderer = p.GetComponent<SpriteRenderer>();
            Color currentColor = spriteRenderer.color;

            // Set the transparency (alpha value) of the color to 0.5 (50% transparent)
            if (activate)
            {
                currentColor.a = 1; // Change this value to set different transparency levels
                p.GetComponent<BoxCollider2D>().enabled = true;
                spriteRenderer.sortingOrder = 18;
            }
            else 
            {
                currentColor.a = 0f; // was 0.13
                p.GetComponent<BoxCollider2D>().enabled = false;
                spriteRenderer.sortingOrder = -3;
            }
            // Apply the modified color back to the SpriteRenderer
            spriteRenderer.color = currentColor;
        }      
    }
}
