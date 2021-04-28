using Saver;
using UnityEngine;

namespace SoundsManager
{
    /// <summary>
    ///     BGM管理器
    /// </summary>
    public class BGMManager : MonoBehaviour
    {
        /// <summary>
        ///     平时BGM
        /// </summary>
        public AudioClip normalBgm;

        /// <summary>
        ///     结果BGM
        /// </summary>
        public AudioClip resultBgm;

        /// <summary>
        ///     音源
        /// </summary>
        public AudioSource audioSource;

        private void Start()
        {
            GameManager.GameManager.GetManager.winEvent.AddListener(() =>
                                                                    {
                                                                        audioSource.clip = resultBgm;
                                                                        if (DataTransfer.GetDataTransfer.isSoundsActive)
                                                                            audioSource.Play();
                                                                    });
            GameManager.GameManager.GetManager.loseEvent.AddListener(() =>
                                                                     {
                                                                         audioSource.clip = resultBgm;
                                                                         if (DataTransfer.GetDataTransfer.isSoundsActive)
                                                                             audioSource.Play();
                                                                     });
        }
    }
}