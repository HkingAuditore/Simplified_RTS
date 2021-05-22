using System;
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

        private int            _isAside;
        private Camera         _mainCamera;
        private Unit           _parentUnit;
        private SpriteRenderer _sprite;
        private Vector3        _oriLocalPos;
        private float          _lastOffsetX;

        [ExecuteAlways]
        private void Start()
        {
            _mainCamera  = GameManager.GameManager.GetManager.mainCamera;
            _oriLocalPos = transform.localPosition;
            _sprite      = this.GetComponent<SpriteRenderer>();
            if (isUnit)
            {
                _parentUnit = transform.parent.GetComponent<Unit>();
                _isAside    = LayerMask.LayerToName(transform.parent.gameObject.layer) == "ASide" ? 1 : -1;
            }

            GetComponent<SpriteRenderer>().shadowCastingMode = ShadowCastingMode.On;
        }



        private void LateUpdate()
        {
            if (isUnit)
            {
                float offsetX = ( _parentUnit.pathFinder.steeringTarget - this.transform.position).x;
                if (Mathf.Abs(offsetX) < 1f)
                {
                    offsetX = _lastOffsetX;
                }

                _lastOffsetX = offsetX;
                if (offsetX > 0)
                {
                    transform.rotation      = Quaternion.Euler(25f, 0, 0);
                    transform.localPosition = _oriLocalPos;
                }
                else
                {
                    transform.rotation      = Quaternion.Euler(-25f, 180, 0);
                    transform.localPosition = new Vector3(-_oriLocalPos.x, _oriLocalPos.y, _oriLocalPos.z);

                }
                
            }

            if (isArrow)
            {
                transform.forward  = _mainCamera.transform.forward;
                transform.rotation = _mainCamera.transform.rotation;
            }
        }
    }
}