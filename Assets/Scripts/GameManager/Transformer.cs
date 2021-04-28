using UnityEngine;

namespace GameManager
{
    /// <summary>
    ///     数据传递工具（已弃用）
    /// </summary>
    public class Transformer : MonoBehaviour
    {
        public bool isSoundsActive;

        public static Transformer getTransformer { get; private set; }

        private void Awake()
        {
            getTransformer = this;
        }
    }
}