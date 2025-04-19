using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMode : MonoBehaviour
{
    public static TestMode instance;

    public GameObject testCanvas;

    public Toggle trajectoryToggle;
    public Toggle slowTimeToggle;
    public Toggle wingsToggle;
    public Toggle sidesToggle;
    public Toggle floorToggle;
    public Toggle pinksPlusToggle;
    public Toggle orangesToggle;
    public Toggle stickyToggle;

    void Start()
    {
        instance = this;
    }

    public void UpdateTrajectory()
    {
        if (trajectoryToggle.isOn) PickUpsManager.instance.ActivateTrajectory(false);
        else PickUpsManager.instance.DeactivateTrajectoryInstantly();
    }
    public void UpdateSlowTime()
    {
        if (slowTimeToggle.isOn) PickUpsManager.instance.ActivateSlowTime(false);
        else PickUpsManager.instance.DeactivateSlowTimeInstantly();
        Time.timeScale = 0;
    }
    public void UpdateWings()
    {
        if (wingsToggle.isOn) PickUpsManager.instance.ActivateWings(false);
        else PickUpsManager.instance.DeactivateWingsInstantly();
    }
    public void UpdateSides()
    {
        if (sidesToggle.isOn) PickUpsManager.instance.ActivateSidesObject(false);
        else PickUpsManager.instance.sidesObject.SetActive(false);
    }
    public void UpdateFloor()
    {
        if (floorToggle.isOn) PickUpsManager.instance.ActivateFloorObject(false);
        else PickUpsManager.instance.floorObject.SetActive(false);
    }
    public void UpdatePinksPlus()
    {
        if (pinksPlusToggle.isOn) PickUpsManager.instance.ActivateBouncierPinks(false);
        else PickUpsManager.instance.DeactivateBouncierPinksInstantly();
    }

    public void UpdateOrange()
    {
        if (orangesToggle.isOn) PickUpsManager.instance.ActivateOranges(false);
        else PickUpsManager.instance.DeactivateOrangesInstantly();
    }

    public void UpdateSticky()
    {
        if (stickyToggle.isOn) PickUpsManager.instance.ActivateSticky(false);
        else PickUpsManager.instance.DeactivateStickyInstantly();
    }
}
