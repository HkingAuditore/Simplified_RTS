using System.Collections.Generic;
using Saver.Quiz;
using UI.Tutorial;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Saver
{
    /// <summary>
    ///     数据传输
    /// </summary>
    public class DataTransfer : MonoBehaviour
    {
        /// <summary>
        ///     教程管理
        /// </summary>
        public TutorialManager tutorialManager;

        /// <summary>
        ///     物品解锁表
        /// </summary>
        public List<bool> itemRevealedList = new List<bool>();

        /// <summary>
        ///     角色解锁表
        /// </summary>
        public List<bool> characterRevealedList = new List<bool>();

        /// <summary>
        ///     关卡解锁表
        /// </summary>
        public List<bool> levelRevealedList = new List<bool>();
        
        /// <summary>
        /// 题目表
        /// </summary>
        public List<Quiz.Quiz> quizList = new List<Quiz.Quiz>();

        /// <summary>
        ///     存档器
        /// </summary>
        public XMLSaver xmlSaver;

        /// <summary>
        ///     读档器
        /// </summary>
        public XMLReader xmlReader;

        /// <summary>
        ///     下一加载场景
        /// </summary>
        public string nextLoadingSceneName;

        private bool _isSoundsActive = true;

        public static DataTransfer GetDataTransfer { get; private set; }

        public bool isSoundsActive
        {
            get => _isSoundsActive;
            set
            {
                _isSoundsActive = value;
                if (_isSoundsActive)
                    GameManager.GameManager.GetManager.audioSource.Play();
                else
                    GameManager.GameManager.GetManager.audioSource.Stop();
            }
        }

        private void Awake()
        {
            GetDataTransfer = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            xmlReader.LoadSaver(true);
            this.quizList = QuizReader.ReadQuizList();

        }

        /// <summary>
        ///     使用进度条加载
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneInLoadingScene(string sceneName)
        {
            nextLoadingSceneName = sceneName;
            SceneManager.LoadSceneAsync("Loading");
        }
    }
}