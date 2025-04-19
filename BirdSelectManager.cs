using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class PlayerSelectMenu : MonoBehaviour
{
    public static PlayerSelectMenu instance;

    public RuntimeAnimatorController[] animatorControllers;
    [SerializeField] Image BiggerPlayerIcon;

    [SerializeField] Image birdCage;
    [SerializeField] SpriteRenderer[] StickySprite;
    //[SerializeField] Transform stickyPlayerVisualTransform;
    [SerializeField] Transform goggles;
    [SerializeField] GameObject[] toUnlockTxt;
    [SerializeField] Button startButton;


    public int playerSelectIndex = 0;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        //get last selected bird
        playerSelectIndex = PlayerPrefs.GetInt("playerNumber");
        SwitchPlayerImage(playerSelectIndex);
    }

    public void ResetUnlocks()
    {
        SaveSystem.SetBool("player1Unlocked", false);
        SaveSystem.SetBool("player2Unlocked", false);
        SaveSystem.SetBool("player3Unlocked", false);
        SaveSystem.SetBool("player4Unlocked", false);
        SaveSystem.SetBool("player5Unlocked", false);
        SaveSystem.SetBool("player6Unlocked", false);
        SaveSystem.SetBool("player7Unlocked", false);

        SaveSystem.SaveToDisk();
    }

    public void UnlocksAllUnlocks()
    {
        SaveSystem.SetBool("player1Unlocked", true);
        SaveSystem.SetBool("player2Unlocked", true);
        SaveSystem.SetBool("player3Unlocked", true);
        SaveSystem.SetBool("player4Unlocked", true);
        SaveSystem.SetBool("player5Unlocked", true);
        SaveSystem.SetBool("player6Unlocked", true);
        SaveSystem.SetBool("player7Unlocked", true);

        SaveSystem.SaveToDisk();
    }

    public void GetPlayerIndexAndSwitchPlayer(bool right)
    {
        if (right)
        {
            if (playerSelectIndex < animatorControllers.Length - 1) playerSelectIndex++;
            else playerSelectIndex = 0;
        }
        else
        {
            if (playerSelectIndex > 0) playerSelectIndex--;
            else playerSelectIndex = animatorControllers.Length - 1;
        }

        SwitchPlayerImage(playerSelectIndex);
    }

    public void SwitchPlayerImage(int index)
    {
        //swap the animator and sprite
        Player.Instance.animator.runtimeAnimatorController = animatorControllers[index];
        SizeAndFlipBirdSprite();
        SetAnimatorSpeed();

        birdCage.enabled = true;

        foreach (GameObject txt in toUnlockTxt)
        {
            txt.SetActive(false);
        }

        switch (index)
        {
            case 0:
                UnlockPlayer();
                break;

            case 1:
                if (SaveSystem.GetBool("player1Unlocked") == false)
                {
                    toUnlockTxt[0].SetActive(true);
                }
                else UnlockPlayer();
            break;

            case 2:
                if (SaveSystem.GetBool("player2Unlocked") == false)
                {
                    toUnlockTxt[1].SetActive(true);
                }
                else UnlockPlayer();
            break;

            case 3:
                if (SaveSystem.GetBool("player3Unlocked") == false)
                {
                    toUnlockTxt[2].SetActive(true);
                }
                else UnlockPlayer();
            break;

            case 4:
                if (SaveSystem.GetBool("player4Unlocked") == false)
                {
                    toUnlockTxt[3].SetActive(true);
                }
                else UnlockPlayer();
            break;

            case 5:
                if (SaveSystem.GetBool("player5Unlocked") == false)
                {
                    toUnlockTxt[4].SetActive(true);
                }
                else UnlockPlayer();
            break;

            case 6:
                if (SaveSystem.GetBool("player6Unlocked") == false)
                {
                    toUnlockTxt[4].SetActive(true);
                }
                else UnlockPlayer();
                break;

            case 7:
                if (SaveSystem.GetBool("player7Unlocked") == false)
                {
                    toUnlockTxt[4].SetActive(true);
                }
                else UnlockPlayer();
                break;

            case 8:
                if (SaveSystem.GetBool("player8Unlocked") == false)
                {
                    toUnlockTxt[5].SetActive(true);
                }
                else UnlockPlayer();
                break;

            case 9:
                if (SaveSystem.GetBool("player9Unlocked") == false)
                {
                    toUnlockTxt[6].SetActive(true);
                }
                else UnlockPlayer();
                break;

            case 10:
                if (SaveSystem.GetBool("player10Unlocked") == false)
                {
                    toUnlockTxt[7].SetActive(true);
                }
                else UnlockPlayer();
                break;

            default:
                UnlockPlayer();
                break;
        }

        if (birdCage.enabled) GameManager.instance.startBtn.GetComponent<Button>().interactable = false;
    }

    void UnlockPlayer()
    {
        birdCage.enabled = false;
        startButton.interactable = true;
    }

    private void SizeAndFlipBirdSprite()
    {
        if (playerSelectIndex < 5 || playerSelectIndex >= 9) Player.Instance.transform.localScale = new Vector2(0.2f, 0.2f);
        else if (playerSelectIndex == 6 || playerSelectIndex == 8) Player.Instance.transform.localScale = new Vector2(-0.17f, 0.2f);
        else Player.Instance.transform.localScale = new Vector2(-0.2f, 0.2f);
    }

    private void SetAnimatorSpeed()
    {
        if (playerSelectIndex == 2 || playerSelectIndex == 9)
        {
            Player.Instance.animator.speed = 0.8f;
        }
        else
        {
            Player.Instance.animator.speed = 1;
        }
    }

    public void UpdateSpritesAndCollidersOnPlay()
    {
        Vector2 offset;

        //set animator speed on bird script
        Player.Instance.defaultAnimatorSpeed = Player.Instance.animator.speed;
       
        //move the circle collider  to fit different style players
        if (playerSelectIndex == 0 || playerSelectIndex == 4) // default
        {
            offset = new Vector2(0.19f, -0.21f);
        }
        else if (playerSelectIndex == 1 || playerSelectIndex == 10) // fat
        {
            offset = new Vector2(0.05f, -0.1f);
        }
        else if (playerSelectIndex == 2 || playerSelectIndex == 9) //thin
        {
            offset = new Vector2(0.1f, -0.65f);
        }
        else if (playerSelectIndex == 3) //pairShaped
        {
            offset = new Vector2(0.1f, -0.18f);
        }
        else if (playerSelectIndex == 5 || playerSelectIndex == 7) //flair
        {
            offset = new Vector2(0f, -0.1f);
        }
        else if (playerSelectIndex == 6 || playerSelectIndex == 8) //bloated
        {
            offset = new Vector2(-0.1f, -0.21f);
        }

        else offset = Vector2.zero;

        //set goggles size
        if (playerSelectIndex < 5 || playerSelectIndex >= 9)
        {
            goggles.localPosition = offset;
            goggles.localScale = new Vector2(1, 1);
        }
        else if (playerSelectIndex == 5 || playerSelectIndex == 7)
        {
            goggles.localPosition = new Vector2(-0.21f, -0.02f);
            goggles.localScale = new Vector2(-1, 1);
        }
        else
        {
            goggles.localPosition = new Vector2(0.14f, -0.02f);
            goggles.localScale = new Vector2(-1, 1f);
            goggles.transform.GetChild(0).localScale = new Vector2(0.2f, 0.28f);
            goggles.transform.GetChild(0).transform.localPosition = new Vector2(0.12f, 0.75f);
        }

        Player.Instance.moonglow.SetParent(Player.Instance.transform); //set after scale change
        Player.Instance.moonglow.GetChild(0).localPosition = offset;

        Player.Instance.sticky.transform.SetParent(Player.Instance.transform);
        if (playerSelectIndex != 6) Player.Instance.sticky.transform.GetChild(0).localPosition = offset;
        Player.Instance.GetComponent<CircleCollider2D>().offset = offset;


        //save the selection
        PlayerPrefs.SetInt("playerNumber", playerSelectIndex);

        // change the sticky sprites to the same colour as the bird
        Color spriteColor = new Color(1,0, 0.2627451f); //red

        switch (playerSelectIndex)
        {
            case 1:
                spriteColor = new Color(0.9921569f,1, 0.4117647f); //yellow
                break;
            case 2:
                spriteColor = new Color(0.04705882f, 0.8745098f, 0.3607843f); //Green
                break;
            case 3:
                spriteColor = new Color(0.7490196f, 0.4235294f, 1); //purple
                break;
            case 4:
                spriteColor = new Color(0.2901961f, 0.8196079f, 1); //lightBlue
                break;
            case 5:
                spriteColor = new Color(0.8117f, 0.18f, 0.3529412f); //dark red
                break;
            case 6:
                spriteColor = new Color(0.9960784f, 0.5803922f, 0.7294118f); //Lightpink
                break;
            case 7:
                spriteColor = new Color(0.5254902f, 0.8392157f, 0.372549f); //light Green
                break;
            case 8:
                spriteColor = new Color(0.372549f, 0.3411765f, 0.3098039f); //Black
                break;
            case 9:
                spriteColor = new Color(1, 0.9882f, 0.9803f); //White
                break;
            case 10:
                spriteColor = new Color(0.9960784f, 0.5803922f, 0.7294118f); //Lightpink
                break;
        }

        foreach (var spriteRenderer in StickySprite)
        {
            spriteRenderer.color = spriteColor;
        }
    }
}
