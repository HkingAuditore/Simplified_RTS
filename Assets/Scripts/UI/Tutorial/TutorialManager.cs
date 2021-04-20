using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]private          int                             _nextTutorialIndex;
    private readonly Dictionary<int, TutorialClipUI> tutorialDict = new Dictionary<int ,TutorialClipUI>();

    public int NextTutorialIndex
    {
        get => _nextTutorialIndex;
        set => _nextTutorialIndex = value >= 0 ? value : 0;
    }

    private void Update()
    {
        Debug.Log("Dict Count:" + tutorialDict.Count);
        Debug.Log("Cur Count:" + NextTutorialIndex);
        if (tutorialDict.Count <= 0 || !tutorialDict.ContainsKey(NextTutorialIndex) ) return;
        if (tutorialDict[NextTutorialIndex].awakeGameObject.activeInHierarchy)
        {
            tutorialDict[NextTutorialIndex].ShowTutorialClip();
        }
    }

    public void RegisterTutorialClip(TutorialClipUI tutorialClip)
    {
        if (tutorialClip.tutorialIndex >= NextTutorialIndex)
        {
            RemoveTutorialClip(tutorialClip);
            tutorialDict.Add(tutorialClip.tutorialIndex, tutorialClip);
        }
        
    }

    public void RemoveTutorialClip(TutorialClipUI tutorialClip)
    {
        if (tutorialDict.ContainsKey(tutorialClip.tutorialIndex))
        {
            tutorialDict[tutorialClip.tutorialIndex].gameObject.SetActive(false);
            tutorialDict.Remove(tutorialClip.tutorialIndex);
            // Debug.Log("Remove:" +tutorialClip.tutorialIndex);
        }
    }

    public void GoToNextClip()
    {
        (tutorialDict.Where(tutorialClipUI => tutorialClipUI.Key <= NextTutorialIndex)
                     .Select(tutorialClipUI => tutorialClipUI.Value)).ToList().ForEach(RemoveTutorialClip);
        NextTutorialIndex++;
    }

    public void ReturnToFormerClip()
    {
        NextTutorialIndex--;
    }
    
    
}
