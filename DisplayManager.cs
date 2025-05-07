using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    public static DisplayManager instance;

    [SerializeField] RectTransform canvasRectTransform;

    [SerializeField] Transform mountainsSectionOfBackGround;
    [SerializeField] Transform mountainsSectionOfBackGround2;
    [SerializeField] Transform mountainsSectionOfBackGround3;

    [SerializeField] RectTransform stageSelectTransform;
    [SerializeField] Transform stageSelectCanvas;

    [SerializeField] RectTransform birdSelectTransform;
    
    void Start()
    {
        instance = this;
    }

    public void SetScreenSize() // from 20x9 and wider
    {
        RectTransform titleSpace = CanvasManager.instance.startScreenCanvas.transform.GetChild(0).GetComponent<RectTransform>();

        RectTransform thumbSpace = CanvasManager.instance.inGameCanvas.transform.GetChild(0).GetComponent<RectTransform>();
        RectTransform thumbArrowLeft = thumbSpace.GetChild(2).GetComponent<RectTransform>();
        RectTransform thumbArrowRight = thumbSpace.GetChild(1).GetComponent<RectTransform>();
        RectTransform pauseButton = GameManager.instance.pauseBtn.GetComponent<RectTransform>();
        RectTransform bestScore = GameManager.instance.bestScoreTxt.transform.parent.GetComponent<RectTransform>();
        RectTransform score = GameManager.instance.scoreTxt.transform.parent.GetComponent<RectTransform>();

        Transform UnlockBirdAnimationAnchor = GameManager.instance.mainCamTransform.GetChild(0).transform;
        Transform unlockBirdText = UnlockBirdAnimation.instance.UnlockBirdText.transform;

        // if screen width larger than default
        if (canvasRectTransform.rect.width > 276)
        {
            float screenWidthDifferenceFromDefault = Mathf.Min(canvasRectTransform.rect.width - 278, 99);

            float newCameraSizeReduction = screenWidthDifferenceFromDefault * 0.035f;

            //zoom and move the camera
            Camera.main.orthographicSize = 12 - newCameraSizeReduction;

            //move camera vertical position
            float newCameraYpositionAdition = screenWidthDifferenceFromDefault * 0.019f;
            GameManager.instance.mainCamTransform.position = new Vector3(0, newCameraYpositionAdition, -10);

            // move the pause button
            pauseButton.anchoredPosition = new Vector2(pauseButton.anchoredPosition.x + (screenWidthDifferenceFromDefault / 2), pauseButton.anchoredPosition.y);

            //move the scores text
            score.localPosition = new Vector2(107.5f + (screenWidthDifferenceFromDefault / 2f), score.localPosition.y);
            bestScore.localPosition = new Vector2(-107.5f - (screenWidthDifferenceFromDefault / 2f), bestScore.localPosition.y);

            //scale and move the thumbspace
            thumbSpace.localPosition = new Vector2(0, screenWidthDifferenceFromDefault * -1);
            thumbArrowLeft.localPosition = new Vector2(-84.5f - screenWidthDifferenceFromDefault, -252.5f + screenWidthDifferenceFromDefault * 0.5f);
            thumbArrowRight.localPosition = new Vector2(84.5f + screenWidthDifferenceFromDefault, -252.5f + screenWidthDifferenceFromDefault * 0.5f);

            // remove thumbspace arrows if screen width is too wide
            if (canvasRectTransform.rect.width > 342)
            {
                thumbArrowLeft.gameObject.SetActive(false);
                thumbArrowRight.gameObject.SetActive(false);
            }
            else //scale the arrows to suit the thumbspace
            {
                thumbArrowLeft.gameObject.SetActive(true);
                thumbArrowRight.gameObject.SetActive(true);

                float newArrowScale = 0.5f - (screenWidthDifferenceFromDefault * 0.003f);

                thumbArrowRight.localScale = new Vector2(newArrowScale, newArrowScale);
                thumbArrowLeft.localScale = new Vector2(newArrowScale, newArrowScale);
            }

        

            UnlockBirdAnimationAnchor.localPosition = new Vector2(0, screenWidthDifferenceFromDefault * -0.022f);
            unlockBirdText.localPosition = new Vector2(0, unlockBirdText.localPosition.y + (screenWidthDifferenceFromDefault * 0.2f));

            GameManager.instance.newHighScoreText.transform.localPosition = new Vector2(0, GameManager.instance.newHighScoreText.transform.localPosition.y + (screenWidthDifferenceFromDefault * 0.2f));
            
            //move mountains background
            mountainsSectionOfBackGround.localPosition = new Vector2(0, 2.8f - (screenWidthDifferenceFromDefault * 0.011f));
            mountainsSectionOfBackGround2.localPosition = new Vector2(3.13f, -6.27f - (screenWidthDifferenceFromDefault * 0.011f));
            mountainsSectionOfBackGround3.localPosition = new Vector2(-2.77f, -0.47f + (screenWidthDifferenceFromDefault * 0.0045f));

            stageSelectCanvas.localPosition = new Vector2(0, -106.7f - (screenWidthDifferenceFromDefault * 0.25f));

            //move floor object
            PickUpsManager.instance.floorObject.transform.localPosition = new Vector3(0, -7.95f - (screenWidthDifferenceFromDefault * 0.005f), 11);

            titleSpace.transform.localPosition = new Vector3(0, 0 - (screenWidthDifferenceFromDefault * 0.5f), 0);
            titleSpace.transform.GetChild(1).transform.localPosition = new Vector3(0, -252.5f - (screenWidthDifferenceFromDefault * -0.25f), 0);

            //GameManager.instance.redButtons.transform.localPosition = new Vector3(0, 0 - (screenWidthDifferenceFromDefault * 0.2f), 0);
            GameManager.instance.redButtons.transform.localPosition = new Vector3(0, 0 - (screenWidthDifferenceFromDefault * 0.35f), 0);

            GameManager.instance.redButtons.transform.localScale = new Vector2(1 + (screenWidthDifferenceFromDefault * 0.0016f), 1 + (screenWidthDifferenceFromDefault * 0.0016f));
            GameManager.instance.releaseBirdButton.transform.localScale = new Vector2(1 + (screenWidthDifferenceFromDefault * 0.0016f), 1 + (screenWidthDifferenceFromDefault * 0.0016f));

            birdSelectTransform.anchoredPosition = new Vector2(0, 0 - (screenWidthDifferenceFromDefault * 0.2f));
            stageSelectTransform.anchoredPosition = new Vector2(0, -106.7f - (screenWidthDifferenceFromDefault * 0.5f));

            birdSelectTransform.localScale = new Vector2(1 + (screenWidthDifferenceFromDefault * 0.001f), 1 + (screenWidthDifferenceFromDefault * 0.001f));
            stageSelectTransform.localScale = new Vector2(1 + (screenWidthDifferenceFromDefault * 0.001f), 1 + (screenWidthDifferenceFromDefault * 0.001f));

            foreach (var rectTransform in CanvasManager.instance.StartScreenCanvases)
            {
                rectTransform.localScale = new Vector2(2.57f + (screenWidthDifferenceFromDefault * 0.0026f), 2.57f + (screenWidthDifferenceFromDefault * 0.0026f));
            }

        }
    }
}
