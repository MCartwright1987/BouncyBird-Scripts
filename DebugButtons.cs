using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButtons : MonoBehaviour
{
    [SerializeField] GameObject button1;
    [SerializeField] GameObject button2;
    [SerializeField] GameObject button3;
    [SerializeField] GameObject button4;

    private void Start()
    {
        button1.SetActive(true);
    }

    public void Button1()
    {
        button1.SetActive(false);
        button2.SetActive(true);

        StartCoroutine(SetButtonsOneActive());
    }

    public void Button2()
    {
        button2.SetActive(false);
        button3.SetActive(true);
    }

    public void Button3()
    {
        button3.SetActive(false);
        button4.SetActive(true);
    }

    public void Button4()
    {
        if (!button1.activeSelf)
        {
            CanvasManager.instance.ToggleDebugCanvas(true);
        }       
    }

    IEnumerator SetButtonsOneActive()
    {
        yield return new WaitForSeconds(2);

        button1.SetActive(true);
        button2.SetActive(false);
        button3.SetActive(false);
        button4 .SetActive(false);
    }


}
