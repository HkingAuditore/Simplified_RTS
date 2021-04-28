using System.Collections;
using Saver;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Loading
{
    /// <summary>
    ///     加载管理
    /// </summary>
    public class LoadingManager : MonoBehaviour
    {
        /// <summary>
        ///     加载的下一场景
        /// </summary>
        public string nextLoadingScene;

        /// <summary>
        ///     加载进度条
        /// </summary>
        public Image loadingBar;

        private float          curProgressValue;
        private AsyncOperation operation;
        private float          showProgressValue;


        private void Start()
        {
            nextLoadingScene = DataTransfer.GetDataTransfer.nextLoadingSceneName;
            LoadNextScene();
        }

        private void Update()
        {
            if (showProgressValue < curProgressValue) showProgressValue += .02f;
            loadingBar.fillAmount = Mathf.SmoothStep(0, 0.85f, showProgressValue * 1.2f) / 0.85f;

            if (showProgressValue > .85) operation.allowSceneActivation = true; //启用自动加载场景  
        }

        private void LoadNextScene()
        {
            StartCoroutine(AsyncLoading());
        }

        private IEnumerator AsyncLoading()
        {
            operation = SceneManager.LoadSceneAsync(nextLoadingScene);
            //阻止当加载完成自动切换
            operation.allowSceneActivation = false;
            while (!operation.isDone)
            {
                curProgressValue = operation.progress;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}