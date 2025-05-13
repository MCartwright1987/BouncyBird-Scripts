using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FlipTransformWhenSpriteFlips : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    Transform birdTransform;

    GameObject flipedSprite;
    GameObject nonFlipedSprite;

    void Start()
    {
        birdTransform = transform.root.transform;
        spriteRenderer = birdTransform.GetComponent<SpriteRenderer>();

        flipedSprite = transform.GetChild(0).gameObject;
        nonFlipedSprite = transform.GetChild(1).gameObject;
    }
    void Update()
    {
        if (birdTransform.localScale.x < 0)
        {
            flipedSprite.SetActive(spriteRenderer.flipX);
            nonFlipedSprite.SetActive(!spriteRenderer.flipX);
        }
        else
        {
            flipedSprite.SetActive(!spriteRenderer.flipX);
            nonFlipedSprite.SetActive(spriteRenderer.flipX);
        }
    }
}
