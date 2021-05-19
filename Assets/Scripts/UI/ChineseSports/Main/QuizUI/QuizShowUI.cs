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
        public QuizUIManager    quizUIManager;
        public Quiz             quiz;
        public Text             quizContent;
        public List<GameObject> quizOptionsButtons = new List<GameObject>();
        
        private List<Text>       quizOptionsButtonTexts;
        private List<GameObject> quizOptionsButtonImages;
        
        public Text       resultText;
        public GameObject ansResultTrue;
        public GameObject ansResultFalse;
        public GameObject ansButton;

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
            ansResultTrue.SetActive(false);
            ansResultFalse.SetActive(false);
            resultText.gameObject.SetActive(false);
            ansButton.SetActive(true);
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
            // if (quiz.QuizOptions[_selectedIndex].IsAnswer)
            // {
            int trueAnsIndex = quiz.QuizOptions.IndexOf(quiz.QuizOptions.Find(x => x.IsAnswer));
            switch (trueAnsIndex)
            {
                case 0:
                    resultText.text = "正确答案：" + "A";
                    break;
                case 1:
                    resultText.text = "正确答案：" +"B";
                    break;
                case 2:
                    resultText.text = "正确答案：" +"C";
                    break;
                case 3:
                    resultText.text = "正确答案：" +"D";
                    break;
            }
            resultText.gameObject.SetActive(true);
            if (quiz.QuizOptions[_selectedIndex].IsAnswer)
            {
                ansResultTrue.SetActive(true);
            }
            else
            {
                ansResultFalse.SetActive(true);
            }
            // nextButtons.ForEach(b => b.SetActive(true));
            ansButton.SetActive(false);
            // }
        }

        public void Next()
        {
            resultText.gameObject.SetActive(false);

            ansResultTrue.SetActive(false);
            ansResultFalse.SetActive(false);
            // nextButtons.ForEach(b => b.SetActive(false));
 
            ansButton.SetActive(true);
            quizUIManager.Next(quiz.QuizOptions[_selectedIndex].IsAnswer);

        }


        #endregion

    }
}