using System;
using System.Collections.Generic;

namespace Saver.Quiz
{
    [Serializable]
    public class Quiz
    {
        public int   QuizIndex;
        public QuizType QuizType;
        public string   QuizContent;
        public List<QuizOption> QuizOptions = new List<QuizOption>();
        
    }
}