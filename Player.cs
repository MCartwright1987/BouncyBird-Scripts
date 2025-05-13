using HeathenEngineering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public Trajectory trajectoryScript;

    public Rigidbody2D rigidBody;

    [SerializeField] Transform offScreenIndicator;

    [SerializeField] int maxSpeed;

    public SpriteRenderer spriteRenderer;

    float previousVelocityY;
    float wingsCooldownTimer;
    public GameObject Wings;
    public GameObject halo;
    public GameObject jetPack;
    public GameObject mallet;
    public GameObject goggles;
    public GameObject sticky;

    bool jetPackActive;

    public int wingsBoostPower = 4;

    public bool colliding;

    public Animator animator;

    public int pinksHit = 0;
    public int powerUpsHit;

    private Vector2 savedVelocity;
    private GameObject lastStuckToObject;
    float addedForceToBounce;
    float bouncePosition;
    public float delayBeforeStickyBounce;

    public float boostForce;

    private bool deathCalled = false;
    public float defaultAnimatorSpeed = 1;

    public Transform moonglow;

    private void Start()
    {
        if (GameManager.gameStarted == false)
        {
            InvokeRepeating("CheckPlayerStillInGame", 3f, 3f);
        }

        Instance = this;

        animator = GetComponent<Animator>();
        

        rigidBody = GetComponent<Rigidbody2D>();
        previousVelocityY = rigidBody.velocity.y;

        spriteRenderer = GetComponent<SpriteRenderer>();

        deathCalled = false;
    }

    private void Update()
    {

        //keeps it within the walls, especially if sticky is active, avoids bugging through wall.
        if (PickUpsManager.instance.sidesObject.activeSelf)
        {
            // Get the current position
            Vector3 currentPosition = transform.position;

            // Clamp the x value
            currentPosition.x = Mathf.Clamp(currentPosition.x, -4.75f, 4.75f);

            // Assign the clamped position back to the transform
            transform.position = currentPosition;
        }


        //limitSpeed was 23

        if (rigidBody.velocity.y > 23) rigidBody.velocity = new Vector2(rigidBody.velocity.x, 23);
        if (rigidBody.velocity.y < -23) rigidBody.velocity = new Vector2(rigidBody.velocity.x, -23);

        //if (rigidBody.velocity.y > 10) rigidBody.velocity = new Vector2(rigidBody.velocity.x, 10);

        if (rigidBody.velocity.x > 6) rigidBody.velocity = new Vector2(7, rigidBody.velocity.y);
        if (rigidBody.velocity.x < -6) rigidBody.velocity = new Vector2(-7, rigidBody.velocity.y);

        if (jetPackActive)
        {
            if (rigidBody.velocity.y < 7) DetatchJetPack();
        }
        
        if (PickUpsManager.instance.wingsActive == true) ActivateWings();

        // make bird face the way it is moving
        if (colliding == false)
        {
            if (rigidBody.velocity.x > -0.01f) spriteRenderer.flipX = false;
            else spriteRenderer.flipX = true;

           //if (rigidBody.velocity.x > -0.01f) transform.rotation = Quaternion.identity;
           //else transform.eulerAngles = new Vector3()
        }
        
        // make bird slowly move to being upright if it is not colliding
        if (!colliding)
        {
            Vector3 currentEulerAngles = transform.eulerAngles;
            float zAngle = currentEulerAngles.z;

            if (zAngle > 180)
            {
                zAngle -= 360;
            }

            if (zAngle < -4 || zAngle > 4)
            {
                float zValue = Mathf.LerpAngle(zAngle, 0, Time.deltaTime * 1.5f);
                transform.eulerAngles = new Vector3(0, 0, zValue);
            }
        }
    }

    public void SwitchMoonGlowColor()
    {
        Color color;

        if (GameManager.levelNumber == 2) color = new Color(0.7803922f, 0.6235294f, 0.3058824f, 0.3176471f);
        else color = new Color(0.7686275f, 0.8509805f, 0.8039216f, 0.3176471f);

        moonglow.GetChild(0).GetComponent<SpriteRenderer>().color = color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Death")
        {            
            if (deathCalled == false)
            {
                deathCalled = true;
                GameManager.instance.Death();       
            }
        }

        if (other.tag == "AdvanceLevel")
        {
            GameManager.instance.MoveCameraSmoothly();
            //Limit speed so bird can only move up 3 levels maximum
            if (rigidBody.velocity.y > 16.5) rigidBody.velocity = new Vector2(rigidBody.velocity.x, 16.5f);     
        }

        if (other.tag == "PowerUp") powerUpsHit++;

        if (other.tag == "Coin") other.gameObject.SetActive(false);
    }

    public void Respawn()
    {
        transform.position = new Vector2(0, GameManager.instance.spawningObjectsAnchor.transform.position.y + 5.5f);
        transform.rotation = Quaternion.identity;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0;
        deathCalled = false;
        rigidBody.simulated = false;
        animator.speed = 1f;

        Wings.transform.GetChild(0).gameObject.SetActive(false);
        Wings.transform.GetChild(1).gameObject.SetActive(false);
        Wings.transform.GetChild(2).gameObject.SetActive(false);
    }

    public void DetatchJetPack()
    {
        jetPack.GetComponent<FollowPlayer>().enabled = false;

        Rigidbody2D jetpackRigidbody = jetPack.GetComponent<Rigidbody2D>();

        jetpackRigidbody.velocity = rigidBody.velocity;

        // Apply a leftward force (adjust the value as needed)
        Vector2 leftForce = new Vector3(-0.5f, 0); // Change -5f to control the force
        jetpackRigidbody.AddForce(leftForce, ForceMode2D.Impulse);


        // Apply a slight torque to tilt the jetpack
        jetpackRigidbody.AddTorque(0.1f, ForceMode2D.Impulse);

        jetPackActive = false;

        jetPack.transform.GetChild(0).gameObject.SetActive(false);
        jetPack.transform.GetChild(1).gameObject.SetActive(true);

        Invoke("ResetJetPack", 2.5f);
    }

    public void ResetJetPack()
    {
        jetPack.transform.rotation = Quaternion.identity;
        jetPack.SetActive(false);
        jetPack.GetComponent<FollowPlayer>().enabled = true;

        jetPack.transform.GetChild(1).gameObject.SetActive(false);
        jetPack.transform.GetChild(0).gameObject.SetActive(true);
    }

    public void CheckPlayerStillInGame()
    {
        if (transform.position.x > 8.5 || transform.position.x < -8.5 || transform.position.y < -9)
        {
            if (deathCalled == false)
            {
                deathCalled = true;
                GameManager.instance.Death();              
            }
        }   
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (PickUpsManager.instance.wingsActive) wingsCooldownTimer = 0.1f;
        colliding = true;
        if (collision.gameObject.tag == "Pink") pinksHit++;

        if (PickUpsManager.instance.stickyActive )
        {
            lastStuckToObject = collision.gameObject;
        
            savedVelocity = rigidBody.velocity;
            rigidBody.constraints = RigidbodyConstraints2D.FreezePosition;

            transform.parent = collision.transform;

            Invoke("UnFreezePosition", delayBeforeStickyBounce);

            animator.speed = 0;
        }

        if (jetPackActive) DetatchJetPack();

        if (PickUpsManager.instance.stickyActive) Audio.instance.sticky.Play();
        else Audio.instance.playerBounce.Play();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        rigidBody.angularVelocity = 0;
        if (PickUpsManager.instance.stickyActive == false) Invoke("TurnCollidingBoolOff", 0.1f);


        //add force if bird bouncing too low
        bouncePosition = transform.position.y - GameManager.instance.spawningObjectsAnchor.position.y;

        addedForceToBounce = (bouncePosition * -1) / 15;
        if (addedForceToBounce < 0) addedForceToBounce = 0;

        //if bird bounces on the lower pads or floor its bounce will be boosted if it is not 
        //reaching the original height so that it can use all the pads

        if (bouncePosition < 2 && bouncePosition + rigidBody.velocity.y < 7)
        {        
            rigidBody.AddForce(Vector3.up * (0.55f + addedForceToBounce), ForceMode2D.Impulse);
            animator.speed = defaultAnimatorSpeed *1.85f;
        }
        else
        {
            animator.speed = defaultAnimatorSpeed;
        }

        if (PickUpsManager.instance.stickyActive == false)
        {
            if (PickUpsManager.instance.trajectoryActive) trajectoryScript.DrawTrajectory();
        }
    }

    private void TurnCollidingBoolOff()=> colliding = false;

    private void UnFreezePosition()
    {
        rigidBody.constraints = RigidbodyConstraints2D.None;

        transform.parent = null;

        rigidBody.velocity = savedVelocity;

        if (bouncePosition < 2 && bouncePosition + rigidBody.velocity.y < 7)
        {
            rigidBody.AddForce(Vector3.up * (0.7f + (addedForceToBounce)), ForceMode2D.Impulse);
            animator.speed = 2f;
        }
        else
        {
            animator.speed = 1f;
        }

        if (PickUpsManager.instance.trajectoryActive) trajectoryScript.DrawTrajectory();

        colliding = false;
    }
    void ActivateWings()
    {
        //if bird is moving down and on the last frame it was higher
        if (rigidBody.velocity.y < 0 && previousVelocityY > 0)
        {
            // if bird is not being boosted down
            if (rigidBody.velocity.y > -5)
            {
                if (wingsCooldownTimer <= 0)
                {
                    StartCoroutine("ActivateWingsBoostAndAnimatioin");
                    wingsCooldownTimer = 0.7f;
                }
            }           
        }
        wingsCooldownTimer -= Time.deltaTime;

        previousVelocityY = rigidBody.velocity.y;
    }

    IEnumerator ActivateWingsBoostAndAnimatioin()
    {
        // if the player is moving too fast or too close to the edge boost it inwards enough to slow it
        // down most of the way plus a small set amount so if traveling slowly it will change its direction going towrds the middle

        float sideWaysForce = 0;

        //if Going too fast or position is nearly off screen
        if (rigidBody.velocity.x < -4 || transform.position.x < -5) 
        {
            sideWaysForce = (rigidBody.velocity.x * -1) / 1.5f + 0.5f;
            Wings.transform.rotation = Quaternion.Euler(0,0,-10);
        }
        else if (rigidBody.velocity.x > 4 || transform.position.x > 5) 
        {
            sideWaysForce = ((rigidBody.velocity.x) / 1.5f + 0.5f) * -1;
            Wings.transform.rotation = Quaternion.Euler(0, 0, 10f);
        }

        // if near the edge and moving towards it too fast.
        else if (rigidBody.velocity.x < -1.5 && transform.position.x < -2.5)
        {
            sideWaysForce = (rigidBody.velocity.x * -1) / 1.5f + 0.5f;
            Wings.transform.rotation = Quaternion.Euler(0, 0, -10);
        }
        else if (rigidBody.velocity.x > 1.5 && transform.position.x > 2.5)
        {
            sideWaysForce = ((rigidBody.velocity.x) / 1.5f + 0.5f) * -1;
            Wings.transform.rotation = Quaternion.Euler(0, 0, 10f);
        }



        Wings.transform.GetChild(0).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.05f);
        Wings.transform.GetChild(0).gameObject.SetActive(false);

        rigidBody.AddForce(Vector3.up * wingsBoostPower, ForceMode2D.Impulse);
        rigidBody.AddForce(Vector3.right * sideWaysForce, ForceMode2D.Impulse);
        Audio.instance.wings.Play();

        if (PickUpsManager.instance.trajectoryActive) trajectoryScript.DrawTrajectory();

        Wings.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Wings.transform.GetChild(1).gameObject.SetActive(false);
        
        
        Wings.transform.GetChild(2).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        Wings.transform.GetChild(2).gameObject.SetActive(false);
        
        Wings.transform.rotation = Quaternion.Euler(0, 0, 0); // reset the rotation
    }

    public void Boost(float force)
    {
        if (PickUpsManager.instance.stickyActive)
        {
            CancelInvoke("UnFreezePosition");

            rigidBody.constraints = RigidbodyConstraints2D.None;

            transform.parent = null;
            colliding = false;
        }

        //doesn't add any force if the birds speed is already greater than the boost
        if (force > 0 && force < rigidBody.velocity.y) force = rigidBody.velocity.y;
        else if (force < 0 && force > rigidBody.velocity.y) force = rigidBody.velocity.y;

        rigidBody.constraints = RigidbodyConstraints2D.None;
        rigidBody.velocity = Vector3.zero;
        rigidBody.angularVelocity = 0;

        if (force > 0) 
        {
            CancelInvoke("ResetJetPack");//edge case if activated while already active
            ResetJetPack();

            if (force > 11.6)
            {
                jetPack.transform.GetChild(0).transform.localScale = new Vector2(1.2f, 1.2f);
                jetPack.transform.GetChild(1).transform.localScale = new Vector2(1.2f, 1.2f);
            }
            else
            {
                jetPack.transform.GetChild(0).transform.localScale = Vector2.one;
                jetPack.transform.GetChild(1).transform.localScale = Vector2.one;
            }

            Audio.instance.jetPack.Play();
            transform.rotation = Quaternion.identity;
            jetPackActive = true;
            jetPack.SetActive(true);

            rigidBody.AddForce(Vector3.up * force, ForceMode2D.Impulse);

            if (PickUpsManager.instance.trajectoryActive) trajectoryScript.DrawTrajectory();
        }
        else
        {
            DeactivateMallet(); //edge case if activated while already active
            boostForce = force;

            if (boostForce < -11.6) //make mallet bigger if upgraded.
            {
                mallet.transform.GetChild(0).GetChild(0).transform.localScale = new Vector2(1.2f, 1.2f);
            }
            else mallet.transform.GetChild(0).GetChild(0).transform.localScale = Vector2.one;

            mallet.SetActive(true);
            Invoke("ActivateMalletForce", 0.25f);
        }
    }

    public void ActivateMalletForce()
    {
        Audio.instance.mallet.Play();
        rigidBody.AddForce(Vector3.up * boostForce, ForceMode2D.Impulse);
        Invoke("DeactivateMallet", 0.3f);
        mallet.GetComponent<FollowPlayer>().enabled = false;
    }

    public void DeactivateMallet()
    {
        mallet.SetActive(false);
        mallet.GetComponent<FollowPlayer>().enabled = true;
    }  
}
