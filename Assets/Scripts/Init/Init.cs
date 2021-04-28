using Saver;
using UnityEngine;

namespace Init
{
    public class Init : MonoBehaviour
    {
        private void Start()
        {
            DataTransfer.GetDataTransfer.LoadSceneInLoadingScene("Start");
        }
    }
}