using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Saver.Quiz
{
    public static class QuizReader
    {
        public static          string quizSaverPath = Application.dataPath + "/Quiz/";
        public static readonly string quizFileName  = "Quiz";

        private static QuizOption ConvertXmlToQuizOption(XmlNode xmlNode)
        {
            QuizOption option = new QuizOption(xmlNode?.InnerText.Trim(),
                                               bool.Parse(xmlNode.Attributes["IsAnswer"].Value));
            return option;
        }

        private static global::Saver.Quiz.Quiz ConvertXmlToQuiz(XmlNode xmlNode)
        {
            global::Saver.Quiz.Quiz quiz = new global::Saver.Quiz.Quiz();
            quiz.QuizIndex   = int.Parse(xmlNode.Attributes["QuizIndex"].Value);
            quiz.QuizType    = (QuizType) Enum.Parse(typeof(QuizType), xmlNode.Attributes["QuizType"].Value);
            quiz.QuizContent = xmlNode["QuizContent"]?.InnerText.Trim();
            quiz.QuizOptions = new List<QuizOption>();
            XmlNodeList options = xmlNode["Options"]?.GetElementsByTagName("Option");
            foreach (XmlNode node in options)
            {
                quiz.QuizOptions.Add(ConvertXmlToQuizOption(node));
            }

            return quiz;
        }

        public static List<global::Saver.Quiz.Quiz> ReadQuizList()
        {
            List<global::Saver.Quiz.Quiz> quizList = new List<global::Saver.Quiz.Quiz>();
            var                           xmlNodes = QuizReader.LoadXml().DocumentElement.SelectNodes("/Quiz");
            // Debug.Log(xmlNodes.Count);
            foreach (XmlNode xmlNode in xmlNodes)
            {
                quizList.Add(QuizReader.ConvertXmlToQuiz(xmlNode));
            }

            return quizList;
        }
        
        
        private static XmlDocument LoadXml()
        {
            var path = QuizReader.quizSaverPath + QuizReader.quizFileName + ".xml";
            if (File.Exists(path))
            {
                var xmlDoc = new XmlDocument();
                xmlDoc.Load(path);
                // AssetDatabase.Refresh();
                return xmlDoc;
            }

            throw new Exception(path + "文件不存在");
        }

    }
}
