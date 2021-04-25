using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TutorialClipUI : MonoBehaviour
{
    public GameObject awakeGameObject;
    public UnityEvent tutorialStartEvent = new UnityEvent();
    public UnityEvent clickEvent         = new UnityEvent();
    public UnityEvent closeEvent         = new UnityEvent();
    public int        tutorialIndex;
    public float      maxStayTime = 60f;
    public GameObject tutorialPanel;

    public void OnClick()
    {
        clickEvent.Invoke();
    }

    private void Start()
    {
        DataTransfer.GetDataTransfer.tutorialManager.RegisterTutorialClip(this);
        
    }

    IEnumerator WaitForStayTime()
    {
        yield return new WaitForSeconds(maxStayTime); 
        FinishTutorialClip();
    }

    public void ShowTutorialClip()
    {
        tutorialPanel.SetActive(true);
        this.tutorialStartEvent.Invoke();
        StartCoroutine(WaitForStayTime());
    }

    public void FinishTutorialClip()
    {
        this.closeEvent.Invoke();
        try
        {
            StopCoroutine(WaitForStayTime());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        DataTransfer.GetDataTransfer.tutorialManager.GoToNextClip();
        this.gameObject.SetActive(false);
    }

    public void ChangeTimeScale(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}
