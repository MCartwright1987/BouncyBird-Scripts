using UnityEngine;

public class UnlockBirdTriggers : MonoBehaviour
{
    public void EndAnimation()
    {
        UnlockBirdAnimation.instance.EndAnimation();
    }

    public void EnableText()
    {
        UnlockBirdAnimation.instance.EnableText();
    }

    public void DisableText()
    {
        UnlockBirdAnimation.instance.DisableText();
    }
}
