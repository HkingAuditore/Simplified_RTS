using System;
using System.Collections.Generic;
using System.Linq;
using Saver.Quiz;
using UnityEngine;
using UnityEngine.UI;

namespace UI.ChineseSports.Main.QuizUI
{
    public class QuizShowUI : MonoBehaviour
    {
        public QuizUIManager quizUIManager;
        public Quiz          quiz;
        public Text          quizContent;
        public List<GameObject>    quizOptionsButtons = new List<GameObject>();

        private List<Text>       quizOptionsButtonTexts;
        private List<GameObject> quizOptionsButtonImages;

        private int _selectedIndex;

        #region 题目展示与答题

        private void Awake()
        {
            quizOptionsButtonTexts = (from button in quizOptionsButtons
                                      select button.transform.Find("Text").GetComponent<Text>()).ToList();
            quizOptionsButtonImages = (from button in quizOptionsButtons
                                       select button.transform.Find("Selected").gameObject).ToList();
        }

        public void ShowQuiz()
        {
            quizContent.text = quiz.QuizContent;
            for (int i = 0; i < quizOptionsButtons.Count; i++)
            {
                string index ="";
                switch (i)
                {
                    case 0:
                        index = "A.";
                        break;
                    case 1:
                        index = "B.";
                        break;
                    case 2:
                        index = "C.";
                        break;
                    case 3:
                        index = "D.";
                        break;
                    default:
                        break;
                }
                quizOptionsButtonTexts[i].text = index + quiz.QuizOptions[i].QuizOptionContent;
            }
        }

        public void SelectOption(int index)
        {
            _selectedIndex = index;
            for (var i = 0; i < quizOptionsButtonImages.Count; i++)
            {
                quizOptionsButtonImages[i].SetActive(i == _selectedIndex);
            }

            quizOptionsButtonImages[_selectedIndex].SetActive(true);
        }
        public void Answer()
        {
            if (quiz.QuizOptions[_selectedIndex].IsAnswer)
            {
                quizUIManager.Next();
            }
        }


        #endregion

    }
}