using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inputs : MonoBehaviour
{
    public static Inputs instance;
    private float initalGrabPosition;
    private float initalPanelPosition;

    public bool screenGrabed;
    public Transform moveablePanel;

    float difference;

    [SerializeField] Slider sensitivitySlider;

    private void Start()
    {
        instance = this;
        // set the toggle to the last saved slider value and update the float
        if (PlayerPrefs.GetFloat("sensitivitySliderValue") != 0)
        {
            sensitivitySlider.value = PlayerPrefs.GetFloat("sensitivitySliderValue");
        }
    }

    public void OnGrabScreen() // left mouse or touch
    {
        initalGrabPosition = Input.mousePosition.x; // simulates Input.GetTouch(0).position.x on mobile

        initalPanelPosition = moveablePanel.position.x;
        screenGrabed = true;
        StartCoroutine(DragMoveablePanel());
    }
    public void OnScreenLetGo()
    {
        screenGrabed = false;
    }

    public void OnSpaceBar()
    {
        GameManager.instance.TogglePauseMovingObjects();
    }

    public void SaveSensitivityValue()
    {
        PlayerPrefs.SetFloat("sensitivitySliderValue", sensitivitySlider.value);
    }

    IEnumerator DragMoveablePanel()
    {
        while (screenGrabed)
        {
            difference = (initalGrabPosition - Input.mousePosition.x) / (75 / sensitivitySlider.value) ; // was /75

            moveablePanel.position = new Vector2(initalPanelPosition - difference, moveablePanel.position.y);
            yield return null;
        }
    }
}
