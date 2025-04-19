using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class PickUps : MonoBehaviour
{
    public bool upgraded;
    float boostPower = 11.5f;

    [SerializeField] SpriteRenderer pickUpBackGroundSpriteRenderer;
    Color originalColour = new Color(0.7421383f, 0.3995131f, 0.291721f);
    Color flashColour = new Color(1, 0.8f, 0.66f);

    private void OnEnable()
    {
        //give chance to make power up better
        int number = Random.Range(1,11); // number can only go upto 10
        if (number == 4) upgraded = true;
    
        //upgraded = true;
    
        if (upgraded)
        {
            StartCoroutine(FlashColour());
            boostPower = 13.5f;
        }
        
    }

    void Update()
    {
        // make pickup move slowly down the screen
        transform.Translate(Vector2.down * Time.deltaTime * 1f, Space.World);

        // Deactivate when off the screen
        if (transform.localPosition.y < GameManager.instance.spawningObjectsAnchor.position.y - 11f) // 8.8
        {
            DeactivatePickUp();
        }
    }

    IEnumerator FlashColour()
    {
        while (true) // construct creates an infinite loop.
        {
            pickUpBackGroundSpriteRenderer.color = flashColour;
            yield return new WaitForSeconds(0.2f);
            pickUpBackGroundSpriteRenderer.color = originalColour;
            yield return new WaitForSeconds(0.4f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Audio.instance.pickUp.Play();

            CallMethodByName(gameObject.name);

            DeactivatePickUp();

            //dont show explode animation if pickup is below bottom banner
            if (transform.localPosition.y > GameManager.instance.spawningObjectsAnchor.position.y - 7f)
            {
                GameManager.instance.crateExplosionParticleSystem.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -1);
                GameManager.instance.crateExplosionParticleSystem.GetComponent<ParticleSystem>().Play();
            }           
        }
    }

    private void DeactivatePickUp()
    {
        StopCoroutine(FlashColour());
        pickUpBackGroundSpriteRenderer.color = originalColour;
        upgraded = false;
        gameObject.SetActive(false);
    }

    private void CallMethodByName(string objectName)
    {
        switch (objectName)
        {
            case "PickUp_Sides":
                Sides();
                break;

            case "PickUp_SlowTime":
                SlowTime();
                break;

            case "PickUp_BouncierPinks":
                BouncierPads();
                break;

            case "PickUp_Wings":
                Wings();
                break;

            case "PickUp_BoostUp":
                BoostUp();
                break;

            case "PickUp_BoostDown":
                BoostDown();              
                break;

            case "PickUp_Floor":
                Floor();
                break;

            case "PickUp_Sticky":
                Sticky();
                break;

            case "PickUp_Trajectory":
                Trajectory();
                break;

            case "PickUp_Oranges":
                Oranges();
                break;

            case "PickUp_Mystery":

                switch (PickUpsManager.instance.mysteryPickUpNumber)
                {
                    case 0:
                        BoostDown();
                        break;

                    case 1:
                        BoostUp();
                        break;

                    case 2:
                        BouncierPads();
                        break;

                    case 3:
                        Floor();
                        break;

                    case 4:
                        Sides();
                        break;
                    
                    case 5:
                        SlowTime();
                        break;  

                    case 6:
                        Wings();
                        break;
                    case 7:
                        Trajectory();
                        break;

                    case 8:
                        Oranges();
                        break;

                    case 9:
                        Sticky();
                        break;
                }

                break;

            default:
                Debug.LogWarning("Method '" + objectName + "' not found!");
                break;
        }
    }


    public void Oranges()
    {
        PickUpsManager.instance.ActivateOranges(upgraded);
    }
    public void Sides()=> PickUpsManager.instance.ActivateSidesObject(upgraded);
    public void SlowTime()=> PickUpsManager.instance.ActivateSlowTime(upgraded);    
    public void BouncierPads() => PickUpsManager.instance.ActivateBouncierPinks(upgraded);
    public void Wings() => PickUpsManager.instance.ActivateWings(upgraded);
    public void BoostUp()
    {
        Player.Instance.Boost(boostPower + 0.5f);
        boostPower = 11.5f;
    }
    public void BoostDown()
    {
        Player.Instance.Boost(-boostPower + 1);
        boostPower = 11.5f;
    }

    public void Floor() => PickUpsManager.instance.ActivateFloorObject(upgraded);

    public void Trajectory() => PickUpsManager.instance.ActivateTrajectory(upgraded);

    public void Sticky() => PickUpsManager.instance.ActivateSticky(upgraded);
}
