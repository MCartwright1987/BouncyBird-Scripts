using System.Collections;
using UnityEngine;

public class Climber : MonoBehaviour
{
    public void StartClimbing()
    {
        if (gameObject.activeInHierarchy == true) StartCoroutine(ClimbUpMountain());
        GetComponent<Animator>().enabled = true;
    }

    IEnumerator ClimbUpMountain()
    {
        while (transform.localPosition.y < 10.88f)
        {
            transform.Translate(Vector3.up * Time.deltaTime * 0.05f); //0.07

            yield return null;
        }

        GetComponent<Animator>().enabled = false;
    }
}
