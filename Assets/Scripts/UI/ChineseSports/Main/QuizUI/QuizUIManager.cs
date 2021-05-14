using System.Collections.Generic;
using System.Linq;
using Saver;
using Saver.Quiz;
using UnityEngine;

namespace UI.ChineseSports.Main.QuizUI
{
    public class QuizUIManager : MonoBehaviour
    {
        public GameObject            quizConfirmPanel;
        public QuizShowUI            quizShowUI;
        public QuizFinishUI          finishPanel;
        public List<QuizTextTypeSet> quizNumberTextList = new List<QuizTextTypeSet>();
        public List<GameObject>      quizPackList       = new List<GameObject>();
        public List<ItemUI>          ItemUis            = new List<ItemUI>();
        
        
        private List<Quiz> _allQuizList;
        private int        _curQuizIndex = 0;
        private List<Quiz> _curTypeQuizList;
        private int        _curUnitIndex;

        private void Start()
        {
            _allQuizList = DataTransfer.GetDataTransfer.quizList;
            foreach (var t in quizNumberTextList)
            {
                t.QuizNumber.text = (from quiz in _allQuizList
                                     where quiz.QuizType == t.QuizType
                                     select quiz).Count().ToString();
            }

            for (int i = 0; i < quizPackList.Count; i++)
            {
                quizPackList[i].SetActive(DataTransfer.GetDataTransfer.characterRevealedList[i]);
            }
        }

        public void ShowQuiz(Quiz quiz)
        {
            quizShowUI.quiz = quiz;
            quizShowUI.gameObject.SetActive(true);
            quizShowUI.ShowQuiz();
        }

        public void EnterQuiz()
        {
            SetQuizGroup(quizNumberTextList[_curUnitIndex].QuizType);
            quizConfirmPanel.SetActive(false);
        }

        private void SetQuizGroup(QuizType quizType)
        {
            _curTypeQuizList = (_allQuizList.Where(quiz => quiz.QuizType == quizType)).ToList();
            ShowQuiz(_curTypeQuizList[_curQuizIndex]);
        }

        public void Next()
        {
            _curQuizIndex++;
            if (_curQuizIndex >= _curTypeQuizList.Count)
            {
                Finish();
            }
            else
            {
                ShowQuiz(_curTypeQuizList[_curQuizIndex]);
            }
        }

        public void EnterQuizConfirm(int unitIndex)
        {
            _curUnitIndex = unitIndex;
            quizConfirmPanel.SetActive(true);
        }
        
        public void QuitQuizConfirm()
        {
            quizConfirmPanel.SetActive(false);
        }

        private void Finish()
        {
            quizShowUI.gameObject.SetActive(false);
            finishPanel.unlockedUnitIndex                                     = _curUnitIndex;
            DataTransfer.GetDataTransfer.characterUnlockedList[_curUnitIndex] = true;
            DataTransfer.GetDataTransfer.xmlSaver.SaveData();
            ItemUis[_curUnitIndex].IsRevealed = true;
            finishPanel.gameObject.SetActive(true);
        }

        public void Close()
        {
            quizShowUI.gameObject.SetActive(false);
            finishPanel.gameObject.SetActive(false);
            _curQuizIndex = 0;
        }
        
        public void CloseFinishPanel()
        {
            finishPanel.gameObject.SetActive(false);
            _curQuizIndex = 0;
        }
        
        
    }


}
