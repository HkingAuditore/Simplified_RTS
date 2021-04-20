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
    public int        tutorialIndex;

    public void OnClick()
    {
        clickEvent.Invoke();
    }

    private void Start()
    {
        Transformer.getTransformer.tutorialManager.RegisterTutorial(this);
    }

    public void ShowTutorial()
    {
        this.tutorialStartEvent.Invoke();;
    }
}
