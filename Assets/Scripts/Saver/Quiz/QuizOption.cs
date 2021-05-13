using System;

namespace Saver.Quiz
{
    [Serializable]
    public class QuizOption
    {
        public string QuizOptionContent;
        public bool   IsAnswer;

        public QuizOption(string quizOptionContent, bool isAnswer)
        {
            QuizOptionContent = quizOptionContent;
            IsAnswer          = isAnswer;
        }
    }
}