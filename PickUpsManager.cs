using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using System;

public class PickUpsManager : MonoBehaviour
{
    public static PickUpsManager instance;

    public GameObject[] pickUp;

    public int lastPickUpNumber;

    [SerializeField] Image slowTimeIcon;
    [SerializeField] Image wingsIcon;
    [SerializeField] Image biggerPlayerIcon;
    [SerializeField] SpriteRenderer haloSprite;
    [SerializeField] SpriteRenderer gogglesSprite;

    public GameObject sidesObject;
    public GameObject floorObject;
    [SerializeField] Transform[] yellowPads;

    public bool TimeSlowed;
    public bool wingsActive;
    public bool bouncierPinksActive;
    public bool biggerPlayerActive;
    public bool haloActive;
    public bool trajectoryActive;
    public bool orangesActive;
    public bool stickyActive;

    [SerializeField] PhysicsMaterial2D pinksBounce1;
    [SerializeField] PhysicsMaterial2D pinksBounce2;
    [SerializeField] PhysicsMaterial2D pinksBounce3;
    [SerializeField] PhysicsMaterial2D orangeBounce;
    [SerializeField] PhysicsMaterial2D yellowBounce;

    [SerializeField] Coroutine deactivateFloorObjectCoroutine;
    [SerializeField] Coroutine deactivateSidesObjectCoroutine;
    [SerializeField] Coroutine deactivateOrangesCoroutine;
    [SerializeField] Coroutine deactivateBouncierPinksCoroutine;
    [SerializeField] Coroutine deactivateTrajectoryCoroutine;
    [SerializeField] Coroutine deactivateSlowTimeCoroutine;

    private int pads1pad1;
    private int pads2pad1;
    private int pads3pad1;

    private int pads1pad2;
    private int pads2pad2;
    private int pads3pad2;

    Color orange = new Color(0.94f, 0.6f, 0.13f);

    public bool orangePadsAssigned = false;
    public bool orangesUpgraded = false;

    public bool spawnedFirstPickUp = false;

    public int mysteryPickUpNumber; //so mystery follows same rules as spawning

    public int[] possiblePickUpNumbers = Enumerable.Range(0, 11).ToArray();

    public bool isPaused = false;

    private int pickupTimerSeconds = 35;
    private int pickupTimerExtraBonusSeconds = 10;

    //for oranges
    int offsetLevel2 = 0;
    int noFlatYellows = 2;

    void Start()
    {
        instance = this;

        spawnedFirstPickUp = false;
        mysteryPickUpNumber = UnityEngine.Random.Range(0, pickUp.Length-1);
    }

    private bool IsPickUpExcluded(int pickUpNumber)
    {
        // Exclude less effective pickups for level 0
        if (GameManager.instance.score < 1)
        {
            if (pickUpNumber >= 3 && pickUpNumber <= 5) return true;
            if (pickUpNumber == 9 || pickUpNumber == 7 || pickUpNumber == 10) return true;
        }
        else
        {
            // Check if it's the same as the last one spawned
            if (pickUpNumber == lastPickUpNumber) return true;
        }

        //ensure pickup is not the same as the active mystery pickup
        if (pickUp[10].activeSelf && pickUpNumber == mysteryPickUpNumber) return true;

        // Check if the pickup is already active
        if (pickUp[pickUpNumber].activeSelf) return true;

        // Check specific pickup exclusions
        if (pickUpNumber == 1 && Player.Instance.jetPack.activeSelf) return true;
        if (pickUpNumber == 2 && bouncierPinksActive) return true;
        if (pickUpNumber == 3 && floorObject.activeSelf) return true;
        if (pickUpNumber == 4 && sidesObject.activeSelf) return true;
        if (pickUpNumber == 5 && TimeSlowed) return true;
        if (pickUpNumber == 6 && wingsActive) return true;
        if (pickUpNumber == 7 && trajectoryActive) return true;
        if (pickUpNumber == 8 && orangesActive) return true;
        if (pickUpNumber == 9 && stickyActive) return true;

        return false;
    }

    public void SpawnPickUp()
    {
        float pickUpXposition = UnityEngine.Random.Range(-4, 4f);

        if (!spawnedFirstPickUp)
        {
            // Ensure the first pickup isn't too close to the center
            while (pickUpXposition > -1.2 && pickUpXposition < 1.2)
            {
                pickUpXposition = UnityEngine. Random.Range(-4, 4f);
            }
        }

        int pickUpNumber = 0;
        int counter = 0;
        bool mysteryPicked = false;

        possiblePickUpNumbers = possiblePickUpNumbers.OrderBy(x => UnityEngine.Random.value).ToArray(); // Shuffle the numbers

        while (true)
        {
            if (counter >= possiblePickUpNumbers.Length)
            {
                //Find the active pickup with the lowest y position
                float lowestY = float.MaxValue; // Start with the maximum possible float value
                int lowestPickUpNumber = -1; // To hold the pickup number of the lowest y position

                for (int i = 0; i < pickUp.Length; i++)
                {
                    if (pickUp[i].activeSelf) // Check if the pickup is active
                    {
                        float currentY = pickUp[i].transform.position.y; // Get the y position
                        if (currentY < lowestY) // If this pickup is lower than the current lowest
                        {
                            lowestY = currentY; // Update the lowest y
                            lowestPickUpNumber = i; // Update the corresponding pickup number
                        }
                    }
                }

                // If we found an active pickup with the lowest y position, set it
                if (lowestPickUpNumber != -1)
                {
                    Debug.Log("Cycled through all possible pickups, using pickup number: " + lowestPickUpNumber);
                    pickUpNumber = lowestPickUpNumber; // Set the pickup number to the lowest active pickup
                    break; // Exit the loop as we found our pickup
                }
            }

            pickUpNumber = possiblePickUpNumbers[counter];

            if (pickUpNumber == 10 && GameManager.instance.score >= 1 && !pickUp[10].activeSelf && lastPickUpNumber != 10)
            {
                mysteryPicked = true; // Special case for mystery pickup
                break;                                           
            }
            else if (!IsPickUpExcluded(pickUpNumber))
            {
                break; // We found a valid pickup
            }

            counter++;           
        }

        if (mysteryPicked)
        {
            lastPickUpNumber = 10;
            counter = 0;

            while (true)
            {
                // if looped through all then mystery = jetpack
                if (counter >= possiblePickUpNumbers.Length)
                {
                    pickUpNumber = 1;
                    break;
                }

                pickUpNumber = possiblePickUpNumbers[counter];

                if (!IsPickUpExcluded(pickUpNumber))
                {
                    break; // We found a valid pickup
                }

                counter++;
            }

            mysteryPickUpNumber = pickUpNumber;
            pickUp[10].SetActive(true);
            pickUp[10].transform.position = new Vector2(pickUpXposition, GameManager.instance.spawningObjectsAnchor.position.y + 12);
        }
        else
        {
            lastPickUpNumber = pickUpNumber;
            pickUp[pickUpNumber].SetActive(true);
            pickUp[pickUpNumber].transform.position = new Vector2(pickUpXposition, GameManager.instance.spawningObjectsAnchor.position.y + 12);
        }

        spawnedFirstPickUp = true;
    }

    public void ActivateOranges(bool upgraded)
    {
        Audio.instance.oranges.Play();

        if (GameManager.levelNumber == 3)
        {
            offsetLevel2 = 6;
            noFlatYellows = 2;
        }
        else if (GameManager.levelNumber == 2) 
        {
            offsetLevel2 = 3;
            noFlatYellows = 1; //allows full range of yellows for level 2
        }
        else
        {
            offsetLevel2 = 0;
            noFlatYellows = 2;
        }

        if (deactivateOrangesCoroutine != null)
        {
            StopCoroutine(deactivateOrangesCoroutine);
            deactivateOrangesCoroutine = null;
        }

        if (orangePadsAssigned == false)  //keep the same oranges if renewed before deactivating
        {
            orangePadsAssigned = true;

            pads1pad1 = UnityEngine.Random.Range(noFlatYellows, yellowPads[0 + offsetLevel2].childCount);
            pads2pad1 = UnityEngine.Random.Range(1, yellowPads[1 + offsetLevel2].childCount);
            pads3pad1 = UnityEngine.Random.Range(noFlatYellows, yellowPads[2 + offsetLevel2].childCount);
        
            pads1pad2 = UnityEngine.Random.Range(noFlatYellows, yellowPads[0 + offsetLevel2].childCount);
            pads2pad2 = UnityEngine.Random.Range(1, yellowPads[1 + offsetLevel2].childCount);
            pads3pad2 = UnityEngine.Random.Range(noFlatYellows, yellowPads[2 + offsetLevel2].childCount);
            
            while (pads1pad2 == pads1pad1) pads1pad2 = UnityEngine.Random.Range(noFlatYellows, yellowPads[0 + offsetLevel2].childCount);
            while (pads2pad2 == pads2pad1) pads2pad2 = UnityEngine.Random.Range(1, yellowPads[1 + offsetLevel2].childCount);
            while (pads3pad2 == pads3pad1) pads3pad2 = UnityEngine.Random.Range(noFlatYellows, yellowPads[2 + offsetLevel2].childCount);          
        }      

        // Set color and bounce material for first set of pads
        SetPadColorAndBounce(yellowPads[0 + offsetLevel2].GetChild(pads1pad1), new Color(1, 0.6392157f, 0), orangeBounce);
        SetPadColorAndBounce(yellowPads[1 + offsetLevel2].GetChild(pads2pad1), new Color(1, 0.6392157f, 0), orangeBounce);
        SetPadColorAndBounce(yellowPads[2 + offsetLevel2].GetChild(pads3pad1), new Color(1, 0.6392157f, 0), orangeBounce);

        // If upgraded, also set for the second set of pads
        if (upgraded || orangesUpgraded) // keeps upgrade if renewing oranges
        {
            orangesUpgraded = true;

            SetPadColorAndBounce(yellowPads[0 + offsetLevel2].GetChild(pads1pad2), new Color(1, 0.6392157f, 0), orangeBounce);
            SetPadColorAndBounce(yellowPads[1 + offsetLevel2].GetChild(pads2pad2), new Color(1, 0.6392157f, 0), orangeBounce);
            SetPadColorAndBounce(yellowPads[2 + offsetLevel2].GetChild(pads3pad2), new Color(1, 0.6392157f, 0), orangeBounce);
        }

        orangesActive = true;

        deactivateOrangesCoroutine = StartCoroutine(DeactivateOranges(upgraded));
    }

    IEnumerator DeactivateOranges(bool upgraded)
    {
        float elapsedTime = 0;
        float interval = 0.2f;

        while (elapsedTime < pickupTimerSeconds - 4) //26
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (elapsedTime < pickupTimerSeconds) //30
        {
            if (isPaused) break;

            // First set all to the intermediate color (fade effect)
            SetPadColorAndBounce(yellowPads[0 + offsetLevel2].GetChild(pads1pad1), new Color(1, 0.9254903f, 0.1529412f), orangeBounce);
            SetPadColorAndBounce(yellowPads[1 + offsetLevel2].GetChild(pads2pad1), new Color(1, 0.9254903f, 0.1529412f), orangeBounce);
            SetPadColorAndBounce(yellowPads[2 + offsetLevel2].GetChild(pads3pad1), new Color(1, 0.9254903f, 0.1529412f), orangeBounce);

            if (upgraded)
            {
                SetPadColorAndBounce(yellowPads[0 + offsetLevel2].GetChild(pads1pad2), new Color(1, 0.9254903f, 0.1529412f), orangeBounce);
                SetPadColorAndBounce(yellowPads[1 + offsetLevel2].GetChild(pads2pad2), new Color(1, 0.9254903f, 0.1529412f), orangeBounce);
                SetPadColorAndBounce(yellowPads[2 + offsetLevel2].GetChild(pads3pad2), new Color(1, 0.9254903f, 0.1529412f), orangeBounce);
            }

            yield return new WaitForSeconds(interval);

            // Set all back to orange
            SetPadColorAndBounce(yellowPads[0 + offsetLevel2].GetChild(pads1pad1), new Color(1, 0.6392157f, 0), orangeBounce);
            SetPadColorAndBounce(yellowPads[1 + offsetLevel2].GetChild(pads2pad1), new Color(1, 0.6392157f, 0), orangeBounce);
            SetPadColorAndBounce(yellowPads[2 + offsetLevel2].GetChild(pads3pad1), new Color(1, 0.6392157f, 0), orangeBounce);

            if (upgraded)
            {
                SetPadColorAndBounce(yellowPads[0 + offsetLevel2].GetChild(pads1pad2), new Color(1, 0.6392157f, 0), orangeBounce);
                SetPadColorAndBounce(yellowPads[1 + offsetLevel2].GetChild(pads2pad2), new Color(1, 0.6392157f, 0), orangeBounce);
                SetPadColorAndBounce(yellowPads[2 + offsetLevel2].GetChild(pads3pad2), new Color(1, 0.6392157f, 0), orangeBounce);
            }

            yield return new WaitForSeconds(interval);

            elapsedTime += interval * 2; // Increment elapsed time

            interval -= 0.01f; // Decrease interval to speed up
          
        }
        DeactivateOrangesInstantly();

    }
    private void SetPadColorAndBounce(Transform pad, Color color, PhysicsMaterial2D bounceMaterial)
    {
        SpriteRenderer renderer = pad.GetComponent<SpriteRenderer>();
        BoxCollider2D collider = pad.GetComponent<BoxCollider2D>();

        renderer.color = new Color(color.r, color.g, color.b, renderer.color.a);  // Keep alpha value
        collider.sharedMaterial = bounceMaterial;
    }

    public void DeactivateOrangesInstantly()
    {
        orangesActive = false;

        // Reset all pads' color and bounce material
        SetPadColorAndBounce(yellowPads[0 + offsetLevel2].GetChild(pads1pad1), new Color(1, 1, 0), yellowBounce);
        SetPadColorAndBounce(yellowPads[1 + offsetLevel2].GetChild(pads2pad1), new Color(1, 1, 0), yellowBounce);
        SetPadColorAndBounce(yellowPads[2 + offsetLevel2].GetChild(pads3pad1), new Color(1, 1, 0), yellowBounce);

        SetPadColorAndBounce(yellowPads[0 + offsetLevel2].GetChild(pads1pad2), new Color(1, 1, 0), yellowBounce);
        SetPadColorAndBounce(yellowPads[1 + offsetLevel2].GetChild(pads2pad2), new Color(1, 1, 0), yellowBounce);
        SetPadColorAndBounce(yellowPads[2 + offsetLevel2].GetChild(pads3pad2), new Color(1, 1, 0), yellowBounce);

        orangePadsAssigned = false;
        orangesUpgraded = false;
    }

    public void ActivateBouncierPinks(bool upgraded)
    {
        Audio.instance.pinks.Play();

        if (deactivateBouncierPinksCoroutine != null) //stops the coroutine if already started
        {
            StopCoroutine(deactivateBouncierPinksCoroutine);
            deactivateBouncierPinksCoroutine = null;
        }

        Vector2 colliderSize = new Vector2(1.45f, 0.53f);
        PhysicsMaterial2D upgradebounce = pinksBounce2;
        int upgradeSize = 1;

        if (upgraded)
        {
            upgradebounce = pinksBounce3;
            colliderSize = new Vector2(1.7f, 0.53f);
            upgradeSize = 2;
        }

        foreach (var pink in PinksManager.instance.pinks)
        {
            pink.GetComponent<BoxCollider2D>().sharedMaterial = upgradebounce;

            pink.transform.GetChild(0).gameObject.SetActive(true);
            pink.GetComponent<BoxCollider2D>().size = colliderSize;
            pink.transform.GetChild(upgradeSize).gameObject.SetActive(true);
        }

        bouncierPinksActive = true;

        deactivateBouncierPinksCoroutine = StartCoroutine(DeactivateBouncierPinks(upgraded));
    }

    IEnumerator DeactivateBouncierPinks(bool upgraded)
    {
        float elapsedTime = 0;
        float interval = 0.2f;

        int upgradeSize = 1;
        if (upgraded) upgradeSize = 2;

        while (elapsedTime < pickupTimerSeconds -4)
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (elapsedTime < pickupTimerSeconds)
        {
            if (isPaused) break;

            elapsedTime += Time.deltaTime;
                 
            foreach (var pink in PinksManager.instance.pinks)
            {
                pink.transform.GetChild(upgradeSize).gameObject.SetActive(false);
            }
           
            yield return new WaitForSeconds(interval);

            foreach (var pink in PinksManager.instance.pinks)
            {
                pink.transform.GetChild(upgradeSize).gameObject.SetActive(true);
            }
            
            yield return new WaitForSeconds(interval);
        
            elapsedTime += interval * 2; // Increment the elapsed time
        
            interval = interval - 0.01f;
        }
        DeactivateBouncierPinksInstantly();
    }

    public void DeactivateBouncierPinksInstantly()
    {
        bouncierPinksActive = false;

        foreach (var pink in PinksManager.instance.pinks)
        {
            pink.GetComponent<BoxCollider2D>().sharedMaterial = pinksBounce1;
            pink.transform.GetChild(0).gameObject.SetActive(false);
            pink.GetComponent<BoxCollider2D>().size = new Vector2(1.22f,0.53f);
            pink.transform.GetChild(1).gameObject.SetActive(false);
            pink.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    public void ActivateSidesObject(bool upgraded)
    {
        Audio.instance.walls.Play();

        if (deactivateSidesObjectCoroutine != null)
        {
            StopCoroutine(deactivateSidesObjectCoroutine);
            deactivateFloorObjectCoroutine = null;
        }

        sidesObject.SetActive(true);
        sidesObject.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        sidesObject.transform.GetChild(1).GetComponent<SpriteRenderer>().enabled = true;

        if (upgraded) deactivateSidesObjectCoroutine = StartCoroutine(DeactivateSidesObject(pickupTimerSeconds + pickupTimerExtraBonusSeconds));
        else deactivateSidesObjectCoroutine = StartCoroutine(DeactivateSidesObject(pickupTimerSeconds));
    }

    IEnumerator DeactivateSidesObject(int timeSpan)
    {
        SpriteRenderer spriteRenderer1 = sidesObject.transform.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer spriteRenderer2 = sidesObject.transform.GetChild(1).GetComponent<SpriteRenderer>();

        float elapsedTime = 0.0f;
        float interval = 0.2f;

        while (elapsedTime < timeSpan -4)
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (elapsedTime < timeSpan)
        {
            if (isPaused) break; 

            elapsedTime += Time.deltaTime;

            spriteRenderer1.enabled = false;
            spriteRenderer2.enabled = false;
            yield return new WaitForSeconds(interval);
            spriteRenderer1.enabled = true;
            spriteRenderer2.enabled = true;
            yield return new WaitForSeconds(interval);

            elapsedTime += interval * 2; // Increment the elapsed time

            interval = interval - 0.01f;
        }
        sidesObject.SetActive(false);
    }

    public void ActivateFloorObject(bool upgraded)
    {
        Audio.instance.walls.Play();

        if (deactivateFloorObjectCoroutine != null)
        {
            StopCoroutine(deactivateFloorObjectCoroutine);
            deactivateFloorObjectCoroutine = null;
        }

        if (floorObject.activeSelf == true)
        {
            floorObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        floorObject.SetActive(true);

        if (upgraded) deactivateFloorObjectCoroutine = StartCoroutine(DeactivateFloorObject(pickupTimerSeconds + pickupTimerExtraBonusSeconds));
        else deactivateFloorObjectCoroutine = StartCoroutine(DeactivateFloorObject(pickupTimerSeconds));
    }

    IEnumerator DeactivateFloorObject(int timeSpan)
    {
        SpriteRenderer spriteRenderer = floorObject.GetComponent<SpriteRenderer>();

        float elapsedTime = 0.0f;
        float interval = 0.2f;

        while (elapsedTime < timeSpan - 4)
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (elapsedTime < timeSpan)
        {
            if (isPaused) break;

            elapsedTime += Time.deltaTime;

            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(interval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(interval);

            elapsedTime += interval * 2; // Increment the elapsed time

            interval = interval - 0.01f;
        }
        spriteRenderer.enabled = true;

        floorObject.SetActive(false);
    }

    public void ActivateSticky(bool upgraded)
    {
        Audio.instance.stickyPickUp.Play();

        if (stickyActive == true) StopCoroutine("DeactivateSticky");

        if (upgraded == true)
        {
            Player.Instance.delayBeforeStickyBounce = 0.3f;
            Player.Instance.sticky.transform.localScale = new Vector2(1.2f, 1.2f);
        }

        stickyActive = true;
        Player.Instance.sticky.SetActive(true);

        StartCoroutine("DeactivateSticky");
    }

    IEnumerator DeactivateSticky()
    {
        SpriteRenderer spriteRenderer = Player.Instance.sticky.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();

        float elapsedTime = 0;
        float interval = 0.2f;

        while (elapsedTime < pickupTimerSeconds -4)
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (elapsedTime < pickupTimerSeconds)
        {
            if (isPaused) break;

            elapsedTime += Time.deltaTime;

            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(interval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(interval);

            elapsedTime += interval * 2;

            interval = interval - 0.01f;
        }
        DeactivateStickyInstantly();
    }

    public void DeactivateStickyInstantly()
    {
        stickyActive = false;
        Player.Instance.sticky.SetActive(false);
        Player.Instance.delayBeforeStickyBounce = 0.2f;
        Player.Instance.transform.GetChild(0).transform.localScale = Vector2.one;
    }

    public void ActivateWings(bool upgraded)
    {
        Audio.instance.choir.Play();

        if (wingsActive == true) StopCoroutine("DeactivateWings");

        if (upgraded)
        {
            Player.Instance.wingsBoostPower = 5;
            Player.Instance.Wings.transform.GetChild(0).localScale = new Vector2(1.2f, 1.2f);
            Player.Instance.Wings.transform.GetChild(1).localScale = new Vector2(1.2f, 1.2f);
            Player.Instance.Wings.transform.GetChild(2).localScale = new Vector2(1.2f, 1.2f);
        }

        wingsActive = true;
        haloSprite.enabled = true;

        Player.Instance.halo.SetActive(true);
        Player.Instance.Wings.SetActive(true);

        StartCoroutine("DeactivateWings");
    }

    IEnumerator DeactivateWings()
    {
        float elapsedTime = 0;
        float interval = 0.2f;

        while (elapsedTime < pickupTimerSeconds - 4)
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (elapsedTime < pickupTimerSeconds)
        {
            if (isPaused) break;

            elapsedTime += Time.deltaTime;

            haloSprite.enabled = false;
            yield return new WaitForSeconds(interval);
            haloSprite.enabled = true;
            yield return new WaitForSeconds(interval);

            elapsedTime += interval * 2; // Increment the elapsed time

            interval = interval - 0.01f;
        }
        DeactivateWingsInstantly();
    }

    public void DeactivateWingsInstantly()
    {
        Player.Instance.wingsBoostPower = 4;

        wingsActive = false;
        Player.Instance.Wings.SetActive(false);
        Player.Instance.halo.SetActive(false);

        Player.Instance.Wings.transform.GetChild(0).localScale = Vector2.one;
        Player.Instance.Wings.transform.GetChild(1).localScale = Vector2.one;
        Player.Instance.Wings.transform.GetChild(2).localScale = Vector2.one;
    }

    public void ActivateTrajectory(bool upgraded)
    {
        Audio.instance.goggles.Play();

        if (deactivateTrajectoryCoroutine != null) //stops the coroutine if already started
        {
            StopCoroutine(deactivateTrajectoryCoroutine);
            deactivateTrajectoryCoroutine = null;
        }

        gogglesSprite.enabled = true;
        trajectoryActive = true;
        Player.Instance.goggles.SetActive(true);

        if (upgraded) deactivateTrajectoryCoroutine = StartCoroutine(DeactivateTrajectory(pickupTimerSeconds + pickupTimerExtraBonusSeconds));
        else deactivateTrajectoryCoroutine = StartCoroutine(DeactivateTrajectory(pickupTimerSeconds));
    }

    IEnumerator DeactivateTrajectory(int timeSpan)
    {
        float elapsedTime = 0;
        float interval = 0.2f;

        while (elapsedTime < timeSpan - 4)
        {
            if (isPaused) 
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }     

        while (elapsedTime < timeSpan)
        {
            if (isPaused) break;

            gogglesSprite.enabled = false;
            yield return new WaitForSeconds(interval);
            gogglesSprite.enabled = true;
            yield return new WaitForSeconds(interval);

            elapsedTime += interval * 2; // Increment the elapsed time

            interval = interval - 0.01f;
        }
        DeactivateTrajectoryInstantly();
    }

    public void DeactivateTrajectoryInstantly()
    {
        trajectoryActive = false;
        Player.Instance.goggles.SetActive(false);
    }

    public void ActivateSlowTime(bool upgraded)
    {
        Audio.instance.slowTime.Play();

        if (deactivateSlowTimeCoroutine != null) //stops the coroutine if already started
        {
            StopCoroutine(deactivateSlowTimeCoroutine);
            deactivateTrajectoryCoroutine = null;
        }

        TimeSlowed = true;
        GameManager.instance.sessionTimeText.color = new Color(1f, 0.9529412f, 0.4627451f);
        Time.timeScale = 0.7f;

        if (upgraded) deactivateSlowTimeCoroutine = StartCoroutine(DeactivateTimeSlowed((pickupTimerSeconds * 0.7f) + (pickupTimerExtraBonusSeconds * 0.7f)));//28
        else deactivateSlowTimeCoroutine = StartCoroutine(DeactivateTimeSlowed(pickupTimerSeconds * 0.7f));//21

    }

    IEnumerator DeactivateTimeSlowed(float timeSpan)
    {
        float elapsedTime = 0;
        float interval = 0.14f; // 70% of 0.2

        while (elapsedTime < timeSpan - (4 * 0.7f))
        {
            if (isPaused)
            {
                yield return null;
                continue;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        while (elapsedTime < timeSpan)
        {
            if (isPaused) break;

            elapsedTime += Time.deltaTime;

            GameManager.instance.sessionTimeText.color = Color.white;
            yield return new WaitForSeconds(interval);
            GameManager.instance.sessionTimeText.color = new Color(1f, 0.9529412f, 0.4627451f);
            yield return new WaitForSeconds(interval);

            elapsedTime += interval * 2; // Increment the elapsed time

            interval = interval - 0.007f;
        }
        DeactivateSlowTimeInstantly();
    }

    public void DeactivateSlowTimeInstantly()
    {
        GameManager.instance.sessionTimeText.color = Color.white;
        TimeSlowed = false;
        //Time.timeScale = 1;
        StartCoroutine(LerpTimeScale(0.7f, 1f, 2f));
    }

    IEnumerator LerpTimeScale(float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time to ensure smooth transition even if timeScale is low
            Time.timeScale = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        Time.timeScale = to; // Ensure exact value at the end
    }

public void TogglePausePickUps()
    {
        if (pickUp[0].GetComponent<PickUps>().enabled == true)
        {
            isPaused = true;
            foreach (GameObject p in pickUp)
            {
                p.GetComponent<Animator>().speed = 0;
                p.GetComponent<PickUps>().enabled = false;
            }
        }
        else
        {
            isPaused = false;
            foreach (GameObject p in pickUp)
            {
                p.GetComponent<Animator>().speed = 1;
                p.GetComponent<PickUps>().enabled = true;
            }
        }
    }
}    

