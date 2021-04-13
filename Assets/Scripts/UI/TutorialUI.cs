using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public List<string> tutorialNames   = new List<string>();
    public List<string> tutorialTexts   = new List<string>();
    public List<Sprite> tutorialAvatars = new List<Sprite>();

    public Button nextButton;
    public Image  image;
    public Text   nameText;
    public Text   contentText;

    private int _curStage = 0;

    public int CurStage
    {
        get => _curStage;
        set
        {
            if(value < tutorialAvatars.Count)
            {
                _curStage = value;
                SetStage(CurStage);
            }
            else
            {
                Quit();
            }
        }
    }

    private void Start()
    {
        SetStage(CurStage);
    }

    public void SetStage(int index)
    {
        nameText.text = tutorialNames[index];
        image.sprite = tutorialAvatars[index];
        contentText.text    = tutorialTexts[index];
    }

    public void OnNextClick()
    {
        CurStage++;
    }

    public void Quit()
    {
        this.gameObject.SetActive(false);
    }
}
