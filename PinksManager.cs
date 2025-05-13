using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinksManager : MonoBehaviour
{
    public static PinksManager instance;

    GameObject lastPink;

    public GameObject[] pinks;

    public bool movingDown = false;

    public GameObject highestPink;
    public GameObject lowestPink;


    private void Start()
    {
        instance = this;
    }

    public void StartFailsafe()
    {
        InvokeRepeating(nameof(CheckForPinkFail), 20f, 1.5f);
    }

    void CheckForPinkFail()
    {
        bool anyActive = false;
        for (int i = 0; i < pinks.Length; i++)
        {
            if (pinks[i].activeSelf)
            {
                anyActive = true;
                break;
            }
        }

        if (!anyActive)
        {
            Debug.LogWarning("Failsafe triggered: all pinks inactive.");
            SpawnPink();
        }
    }

    public void SwitchDirectionDown(bool down)
    {
        if (down)
        {
            for (int i = 0; i < pinks.Length; i++)
            {
                pinks[i].GetComponent<Pinks>().moveDownwards = true;
                //pinks[i].transform.position = new Vector2(transform.position.x, GameManager.instance.spawningObjectsAnchor.position.y + 11);
                pinks[i].GetComponent<Pinks>().triggeredNextPink = false;
            }

            movingDown = true;
        }
        else
        {
            for (int i = 0; i < pinks.Length; i++)
            {
                pinks[i].GetComponent<Pinks>().moveDownwards = false;
                pinks[i].GetComponent<Pinks>().triggeredNextPink = false;
            }

            movingDown = false;
        }
    }

    // public void SpawnPink()
    // {
    //     int obj = Random.Range(0, pinks.Length);
    //     int counter = 0;
    //
    //     // Re-roll if pad is already active or the two middle pads try to spawn consecutively
    //     while (pinks[obj].activeSelf ||
    //           (obj == 3 && lastPink == pinks[4]) ||
    //           (obj == 4 && lastPink == pinks[3]))
    //     {
    //         obj = Random.Range(0, pinks.Length);
    //         counter++;
    //
    //         // Select the highest pink pad or lowest if stage 2 if all are active
    //         if (counter == pinks.Length)
    //         {
    //             if (!movingDown)
    //             {
    //                 float maxY = float.MinValue;
    //                 for (int i = 0; i < pinks.Length; i++)
    //                 {
    //                     if (pinks[i].transform.position.y > maxY)
    //                     {
    //                         maxY = pinks[i].transform.position.y;
    //                         obj = i;
    //                     }
    //                 }
    //             }
    //             else 
    //             {
    //                 float minY = float.MaxValue;
    //                 for (int i = 0; i < pinks.Length; i++)
    //                 {
    //                     if (pinks[i].transform.position.y < minY)
    //                     {
    //                         minY = pinks[i].transform.position.y;
    //                         obj = i;
    //                     }
    //                 }
    //             }
    //             
    //             break; // Exit the loop since we've found the highest or lowest pink
    //         }
    //     }
    //
    //     lastPink = pinks[obj];
    //
    //     if (movingDown)
    //     {
    //         pinks[obj].transform.position = new Vector2(pinks[obj].transform.position.x, GameManager.instance.spawningObjectsAnchor.position.y + 11);
    //         highestPink = pinks[obj]; // if moving downwards the highest pink is last spawned
    //
    //         AssignLowestPink();
    //
    //         if (lowestPink == null) lowestPink = highestPink; //the very first pink doesn't assign both
    //     }
    //     else
    //     {
    //         pinks[obj].transform.position = new Vector2(pinks[obj].transform.position.x, GameManager.instance.spawningObjectsAnchor.position.y - 9);
    //         lowestPink = pinks[obj];
    //
    //         AssignHighestPink();
    //
    //         if (highestPink == null) highestPink = lowestPink;
    //     }
    //
    //     pinks[obj].SetActive(true);
    // }

    public void SpawnPink()
    {
        int obj = -1;

        // Try to find a valid pink index
        List<int> validIndices = new List<int>();
        for (int i = 0; i < pinks.Length; i++)
        {
            bool middlePadBlocked =
             (i == 3 && lastPink == pinks[4]) ||
             (i == 4 && lastPink == pinks[3]);

            if (!pinks[i].activeSelf && !middlePadBlocked)
            {
                validIndices.Add(i);
            }
            else if (!pinks[i].activeSelf)
            {
                // Fallback: allow temporarily breaking the rule if it's the only one left
                obj = i;
            }
        }

        // If none are valid, fallback to highest or lowest pink
        if (validIndices.Count == 0)
        {
            if (!movingDown)
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
        }
        else
        {
            int randomIndex = Random.Range(0, validIndices.Count);
            obj = validIndices[randomIndex];
        }

        lastPink = pinks[obj];

        if (movingDown)
        {
            pinks[obj].transform.position = new Vector2(pinks[obj].transform.position.x, GameManager.instance.spawningObjectsAnchor.position.y + 11);
            highestPink = pinks[obj];

            AssignLowestPink();

            if (lowestPink == null) lowestPink = highestPink;
        }
        else
        {
            pinks[obj].transform.position = new Vector2(pinks[obj].transform.position.x, GameManager.instance.spawningObjectsAnchor.position.y - 9);
            lowestPink = pinks[obj];

            AssignHighestPink();

            if (highestPink == null) highestPink = lowestPink;
        }

        pinks[obj].SetActive(true);
    }


    public void AssignLowestPink()
    {
        float lowestPinkPosition = 100;

        for (int i = 0; i < pinks.Length; i++)
        {
            if (pinks[i].activeSelf && pinks[i].transform.position.y < lowestPinkPosition)
            {
                lowestPinkPosition = pinks[i].transform.position.y;
                lowestPink = pinks[i];
            }
        }
    }

    public void AssignHighestPink()
    {
        float highestPinkPosition = -100;

        for (int i = 0; i < pinks.Length; i++)
        {

            if (pinks[i].activeSelf && pinks[i].transform.position.y > highestPinkPosition)
            {
                highestPinkPosition = pinks[i].transform.position.y;
                highestPink = pinks[i];
            }
        }
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
