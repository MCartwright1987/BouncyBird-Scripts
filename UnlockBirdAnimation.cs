using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockBirdAnimation : MonoBehaviour
{
    public static UnlockBirdAnimation instance;

    [SerializeField] Animator birdLeftToRightAnimation;
    public Animator birdAnimator;
    public GameObject UnlockBirdText;
    public SpriteRenderer birdSpriteRenderer;
    [SerializeField] Transform birdTransform;

    // List of tuples to store numbers and bools
    public List<(int number, bool isTrue)> queuedBirdAndFlipSprite = new List<(int, bool)>();

    void Start()
    {
        instance = this;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void PlayAnimation(int birdNumber, bool flipedSprite)
    {    
        if (birdNumber == 6 || birdNumber == 8)
        {
           birdTransform.localScale = new Vector2(0.17f, 0.2f);
        }
        else birdTransform.localScale = new Vector2(0.2f, 0.2f);


        //if already playing animation then add the bird to the queue
        if (transform.GetChild(0).gameObject.activeSelf)
        {
            queuedBirdAndFlipSprite.Add((birdNumber, flipedSprite));
        }
        else
        {
            birdAnimator.runtimeAnimatorController =
            PlayerSelectMenu.instance.animatorControllers[birdNumber];
        
            birdSpriteRenderer.flipX = flipedSprite;
            transform.GetChild(0).gameObject.SetActive(true);
        }    
    }

    public void EndAnimation()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        //play the next queued bird if there is one.
        if (queuedBirdAndFlipSprite.Count > 0)
        {
            birdAnimator.runtimeAnimatorController = 
            PlayerSelectMenu.instance.animatorControllers[queuedBirdAndFlipSprite[0].number];

            PlayAnimation(queuedBirdAndFlipSprite[0].number, queuedBirdAndFlipSprite[0].isTrue);

            queuedBirdAndFlipSprite.RemoveAt(0);
        }       
    }

    public void EnableText()
    {
        Audio.instance.UnlockBird.Play();
        UnlockBirdText.SetActive(true);
    }
    public void DisableText() => UnlockBirdText.SetActive(false);
}
