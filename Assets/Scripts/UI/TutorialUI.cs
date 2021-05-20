using System.Collections.Generic;
using UI.Tutorial;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public TutorialClipUI tutorialClipUI;
    public List<string>   tutorialNames = new List<string>();

    [TextArea(5,10)] public List<string> tutorialTexts = new List<string>();

    public List<Sprite> tutorialAvatars = new List<Sprite>();

    public Button nextButton;
    public Button previousButton;
    public Image  image;
    public Text   nameText;
    public TextUI contentText;

    private int _curStage;

    public int CurStage
    {
        get => _curStage;
        set
        {
            if (value < tutorialAvatars.Count && value >= 0)
            {
                _curStage = value;
                SetStage(CurStage);
            }
            else if (value >= tutorialAvatars.Count)
            {
                Quit();
            }
        }
    }

    private void Start()
    {
        SetStage(CurStage);
        nextButton.onClick.AddListener(OnNextClick);
        previousButton.onClick.AddListener(OnPreviousButtonClick);
        Time.timeScale = 0;
    }

    public void SetStage(int index)
    {
        nameText.text    = tutorialNames[index] + ":";
        image.sprite     = tutorialAvatars[index];
        contentText.text = tutorialTexts[index];
    }

    public void OnNextClick()
    {
        CurStage++;
    }

    public void OnPreviousButtonClick()
    {
        CurStage--;
    }

    public void Quit()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        tutorialClipUI.FinishTutorialClip();
    }
}