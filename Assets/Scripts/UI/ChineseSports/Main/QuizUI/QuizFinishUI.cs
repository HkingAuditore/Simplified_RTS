using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChineseSports.Main.QuizUI
{
    public class QuizFinishUI : MonoBehaviour
    {
        public CharacterViewUI characterViewUI;
        public int             unlockedUnitIndex;
        public Image           unitImage;

        private void OnEnable()
        {
            unitImage.sprite = characterViewUI.characterSprites[unlockedUnitIndex];
        }
    }
}