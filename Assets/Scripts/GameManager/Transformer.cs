using UnityEngine;

namespace GameManager
{
    /// <summary>
    /// 数据传递工具（已弃用）
    /// </summary>
    public class Transformer : MonoBehaviour
    {
        private static Transformer _transformer;

        public static Transformer getTransformer => _transformer;

        public bool isSoundsActive;

        void Awake () {
            _transformer = this;
        }
    }
}
