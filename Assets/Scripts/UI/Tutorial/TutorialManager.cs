using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TutorialManager : MonoBehaviour
{
    public         int                             nextTutorialIndex = 0;
    public readonly Dictionary<int, TutorialClipUI> tutorialDict      = new Dictionary<int ,TutorialClipUI>();

    private void Update()
    {
        if (tutorialDict[nextTutorialIndex].awakeGameObject.activeInHierarchy)
        {
            
        }
    }

    public void RegisterTutorial(TutorialClipUI tutorialClip)
    {
        tutorialDict.Add(tutorialClip.tutorialIndex,tutorialClip);
    }
    
    
}
