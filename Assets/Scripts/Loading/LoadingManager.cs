using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Loading
{
    public class LoadingManager : MonoBehaviour
    {
        public string nextLoadingScene;
        public Image  loadingBar;

        private float          curProgressValue  = 0;
        private float          showProgressValue = 0;
        private AsyncOperation operation;


        private void Start()
        {
            nextLoadingScene = DataTransfer.GetDataTransfer.nextLoadingSceneName;
            LoadNextScene();
        }

        private void LoadNextScene()
        {
            StartCoroutine(AsyncLoading());
        }

        IEnumerator AsyncLoading()
        {
            operation = SceneManager.LoadSceneAsync(nextLoadingScene);
            //阻止当加载完成自动切换
            operation.allowSceneActivation = false;
            while(!operation.isDone) {          
                curProgressValue = operation.progress;
                yield return new WaitForEndOfFrame();
            }  
        }
    
        void Update()
        {
            if (showProgressValue < curProgressValue)
            {
                showProgressValue += .02f;
            }
            loadingBar.fillAmount = Mathf.SmoothStep(0,0.85f,showProgressValue * 1.2f) / 0.85f;

            if (showProgressValue > .85)
            {
                operation.allowSceneActivation = true; //启用自动加载场景  
            }
        }
    }
}
