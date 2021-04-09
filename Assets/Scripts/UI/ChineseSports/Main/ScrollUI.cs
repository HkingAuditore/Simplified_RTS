using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ScrollUI : MonoBehaviour
{
    public Animator  scrollAnimator;
    [Range(0,1)]
    public float     fillAmount;
    public                        FillBarUI bar;
    public                        string    sceneName;
    public                        Text      titleText;
    public                        Image     titleImage;
    [Header("Scroll UI")] private string    titleName;
    public                        Sprite    titleSprite;

    public string TitleName
    {
        get => titleName;
        set
        {
            titleName      = value;
            titleText.text = TitleName;
        }
    }

    public Sprite TitleSprite
    {
        get => titleSprite;
        set
        {
            titleSprite       = value;
            titleImage.sprite = TitleSprite;
        }
    }


    private void Start()
    {
        SetFillBar();
    }



#if UNITY_EDITOR
    private void Update()
    {
        SetFillBar();
    }
#endif

    [ContextMenu("SetFillBar")]
    public void SetFillBar()
    {
        bar.FillAmount = fillAmount;
        bar.SetFill();
    }
    public void Close()
    {
        scrollAnimator.SetTrigger("Close");
        StartCoroutine(WaitCloseAnimation());
    }

    private IEnumerator WaitCloseAnimation()
    {
        Debug.Log(scrollAnimator.GetCurrentAnimatorStateInfo(0).IsName("Closed"));
        yield return new WaitUntil(() => scrollAnimator.GetCurrentAnimatorStateInfo(0).IsName("Closed"));
        Debug.Log("Animation End!");
        this.gameObject.SetActive(false);

    }

    public void LoadBattle()
    {
        SceneManager.LoadScene(sceneName);
    }
}