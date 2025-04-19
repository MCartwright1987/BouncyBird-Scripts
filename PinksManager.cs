using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinksManager : MonoBehaviour
{
    public static PinksManager instance;

    GameObject lastPink;

    public GameObject[] pinks;


    private void Start()
    {
        instance = this;
    }
    public void SpawnPink()
    {
        int obj = Random.Range(0, pinks.Length);
        int counter = 0;

        // Re-roll if pad is already active or the two middle pads try to spawn consecutively
        while (pinks[obj].activeSelf ||
              (obj == 3 && lastPink == pinks[4]) ||
              (obj == 4 && lastPink == pinks[3]))
        {
            obj = Random.Range(0, pinks.Length);
            counter++;



            // Select the highest pink pad or lowest if stage 2 if all are active
            if (counter == pinks.Length)
            {
                if (GameManager.levelNumber == 1)
                {
                    float maxY = float.MinValue;
                    for (int i = 0; i < pinks.Length; i++)
                    {
                        if (pinks[i].transform.position.y > maxY)
                        {
                            maxY = pinks[i].transform.position.y;
                            obj = i;
                        }
                    }
                }
                else 
                {
                    float minY = float.MaxValue;
                    for (int i = 0; i < pinks.Length; i++)
                    {
                        if (pinks[i].transform.position.y < minY)
                        {
                            minY = pinks[i].transform.position.y;
                            obj = i;
                        }
                    }
                }
                
                break; // Exit the loop since we've found the highest or lowest pink
            }
        }

        lastPink = pinks[obj];

        if (GameManager.levelNumber == 2)
        {
            pinks[obj].transform.position = new Vector2(pinks[obj].transform.position.x, GameManager.instance.spawningObjectsAnchor.position.y + 11);
        }
        else
        {
            pinks[obj].transform.position = new Vector2(pinks[obj].transform.position.x, GameManager.instance.spawningObjectsAnchor.position.y - 9);
        }

        pinks[obj].SetActive(true);
    }

    public void TogglePausePinks()
    {
        if (pinks[0].GetComponent<Pinks>().speed > 0)
        {
            foreach (GameObject p in pinks)
            {
                p.GetComponent<Pinks>().speed = 0;
            }
        }
        else
        {
            foreach (GameObject p in pinks)
            {
                p.GetComponent<Pinks>().speed = 1.04f;
            }
        }
    }
}
