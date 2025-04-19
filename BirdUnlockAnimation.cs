using UnityEngine;

public class BirdUnlockAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

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
