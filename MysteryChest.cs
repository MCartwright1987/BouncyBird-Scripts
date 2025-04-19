using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using UnityEngine;
using UnityEngine.UI;

public class MysteryChest : MonoBehaviour
{
    Animator jiggleAnimator;
    Animator ChestAnimator;
    GameObject ChestTimer;

    // Start is called before the first frame update
    void Start()
    {
        jiggleAnimator = transform.GetChild(0).GetComponent<Animator>();
        ChestAnimator = transform.GetChild(0).transform.GetChild(0).GetComponent<Animator>();

        ChestTimer = transform.GetChild(1).gameObject;

        //ChestReadyToBeOpened();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChestReadyToBeOpened()
    {
        GetComponent<Button>().interactable = true;
        jiggleAnimator.enabled = true;

        StartCoroutine(FlashTimer());

    }

    public void ResetChest()
    {
        GetComponent<Button>().interactable = false;
        StopCoroutine(FlashTimer());
    }

    public void ActivateChest()
    {
        GameManager.instance.blockBttonsImage.SetActive(true);

        jiggleAnimator.enabled = false;
        transform.rotation = Quaternion.identity;
        ChestAnimator.enabled = true;


        int number = Random.Range(1, 101); // number can only go upto 100

        //4% bird unlock
        //5% 3 skip ad tokens
        //11% 2 skip ad tokens
        //80% 1 skip ad token
    }

    IEnumerator FlashTimer()
    {
        while (true)
        {
            ChestTimer.SetActive(false);
            yield return new WaitForSeconds(0.4f);
            ChestTimer.SetActive(true);
            yield return new WaitForSeconds(0.4f);
        }
    }
}
