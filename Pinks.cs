using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pinks : MonoBehaviour
{
    public float speed;
    public bool triggeredNextPink = false;

    public bool moveDownwards = false;
    public bool moveLeftToRight = false;


    //spawning object anchor point is same as camera position

    private void OnEnable()
    {
        if (GameManager.levelNumber == 2) 
        {
            moveDownwards = true;
            transform.position = new Vector2(transform.position.x, GameManager.instance.spawningObjectsAnchor.position.y + 11);
        }
    }

    void Update()
    {  
        //pinks stop spawning when moving down ones added to mix
        //problem pinks moving down can trigger pink going up too close to last one.


        if (moveDownwards) 
        {
            transform.Translate(Vector2.down * Time.deltaTime * speed, Space.World);

            if (triggeredNextPink == false)
            {
                if (transform.localPosition.y < GameManager.instance.spawningObjectsAnchor.position.y + 5)
                {
                    PinksManager.instance.SpawnPink();
                    triggeredNextPink = true;
                }
            }
            
            if (transform.localPosition.y < GameManager.instance.spawningObjectsAnchor.position.y - 11)
            {
                gameObject.SetActive(false);
                triggeredNextPink = false;
            }
        }
        else
        {
            transform.Translate(Vector2.up * Time.deltaTime * speed, Space.World);

            if (triggeredNextPink == false)
            {
                if (transform.localPosition.y > GameManager.instance.spawningObjectsAnchor.position.y - 3)
                {
                    PinksManager.instance.SpawnPink();
                    triggeredNextPink = true;
                }
            }

            if (transform.localPosition.y > GameManager.instance.spawningObjectsAnchor.position.y + 17)
            {
                gameObject.SetActive(false);
                triggeredNextPink = false;
            }
        }

       
    }

}
