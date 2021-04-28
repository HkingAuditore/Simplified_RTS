using Units;
using UnityEngine;
using UnityEngine.Rendering;

namespace Render
{
    /// <summary>
    ///     Sprite渲染加成
    /// </summary>
    public class SpriteRendererPlus : MonoBehaviour
    {
        /// <summary>
        ///     是否是单位
        /// </summary>
        public bool isUnit;

        /// <summary>
        ///     是否为抛射物
        /// </summary>
        public bool isArrow;

        /// <summary>
        ///     是否朝右
        /// </summary>
        public bool faceRight;

        private int    _isAside;
        private Camera _mainCamera;
        private Unit   _parentUnit;

        [ExecuteAlways]
        private void Start()
        {
            _mainCamera = GameManager.GameManager.GetManager.mainCamera;
            if (isUnit)
            {
                _parentUnit = transform.parent.GetComponent<Unit>();
                _isAside    = LayerMask.LayerToName(transform.parent.gameObject.layer) == "ASide" ? 1 : -1;
            }

            GetComponent<SpriteRenderer>().shadowCastingMode = ShadowCastingMode.On;
        }

        private void Update()
        {
            if (isUnit)
            {
                var yDir = _parentUnit.transform.rotation.y;
                faceRight = (int) yDir % 360 < 180 ? true : false;
                if (_isAside == -1) faceRight = !faceRight;
                transform.rotation = Quaternion.Euler(25f * (faceRight ? 1 : -1), faceRight ? 0 : 180, 0);
            }

            if (isArrow)
            {
                transform.forward  = _mainCamera.transform.forward;
                transform.rotation = _mainCamera.transform.rotation;
            }
        }
    }
}