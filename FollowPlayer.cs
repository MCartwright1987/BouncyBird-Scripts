using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] bool copySpriteFlip;
    [SerializeField] bool followPlayer;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        if (copySpriteFlip) spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (followPlayer)
        {
            Vector2 playerPosition = Player.Instance.transform.position;

            transform.position = new Vector2(playerPosition.x, playerPosition.y);
        }
        
        if (copySpriteFlip)
        {
            if (Player.Instance.spriteRenderer.flipX == true) spriteRenderer.flipX = false;
            else spriteRenderer.flipX = true;
        }
    }
}
