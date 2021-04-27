using UnityEngine;

namespace Init
{
    public class Init : MonoBehaviour
    {
        void Start()
        {
            DataTransfer.GetDataTransfer.LoadSceneInLoadingScene("Start");
        }

    }
}
