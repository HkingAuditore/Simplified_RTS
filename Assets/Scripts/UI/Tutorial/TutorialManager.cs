using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.Tutorial
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private int                             _nextTutorialIndex;
        private readonly         Dictionary<int, TutorialClipUI> tutorialDict = new Dictionary<int, TutorialClipUI>();

        public int NextTutorialIndex
        {
            get => _nextTutorialIndex;
            set => _nextTutorialIndex = value >= 0 ? value : 0;
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += (scene0, scene1) => { tutorialDict.Clear(); };
        }

        private void Update()
        {
            // Debug.Log("Dict Count:" + tutorialDict.Count);
            if (tutorialDict.Count <= 0 || !tutorialDict.ContainsKey(NextTutorialIndex)) return;
            if (tutorialDict[NextTutorialIndex].awakeGameObject.activeInHierarchy) tutorialDict[NextTutorialIndex].ShowTutorialClip();
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
            RemoveTutorialClip(tutorialClip.tutorialIndex);
        }

        public void RemoveTutorialClip(int tutorialClipIndex)
        {
            if (tutorialDict.ContainsKey(tutorialClipIndex))
            {
                tutorialDict[tutorialClipIndex].gameObject.SetActive(false);
                tutorialDict.Remove(tutorialClipIndex);
                // Debug.Log("Remove:" +tutorialClip.tutorialIndex);
            }
        }

        public void GoToNextClip()
        {
            tutorialDict.Where(tutorialClipUI => tutorialClipUI.Key <= NextTutorialIndex)
                        .Select(tutorialClipUI => tutorialClipUI.Value).ToList().ForEach(RemoveTutorialClip);
            NextTutorialIndex++;
        }

        public void ReturnToFormerClip()
        {
            NextTutorialIndex--;
        }
    }
}