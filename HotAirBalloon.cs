using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;

public class HotAirBalloon : MonoBehaviour
{
    public static HotAirBalloon Instance;

    public float speed = 0f;
    public int levelReached = 0;

    SpriteRenderer spriteRenderer;

    [SerializeField] Sprite[] balloon;

    void Start()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SwitchSprites()
    {
        spriteRenderer.sprite = balloon[GameManager.levelNumber-1];
    }

    void Update()
    {
        transform.position += Vector3.up * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "AdvanceLevel") levelReached++;
    }
}
