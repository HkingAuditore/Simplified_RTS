using System;
using UnityEngine;

namespace Units.RangedAttack
{
    /// <summary>
    ///     子弹
    /// </summary>
    public class Bullet : MonoBehaviour
    {
        /// <summary>
        ///     初速度
        /// </summary>
        public float initSpeed;

        private float _curAngle;

        private float _curSpeed;

        private float _horizontalSpeed;
        private bool  _isFlying;

        private Unit  _shooter;
        private float _verticalSpeed;

        /// <summary>
        ///     重力
        /// </summary>
        public static float Gravity { get; } = 6f;


        private void Start()
        {
            _curSpeed = initSpeed;
            _isFlying = true;
            _curAngle = transform.eulerAngles.z;

            //水平速度
            _horizontalSpeed = _curSpeed * (float) Math.Sin(_curAngle * Mathf.Deg2Rad);
            //竖直速度
            _verticalSpeed = _curSpeed * (float) Math.Cos(_curAngle * Mathf.Deg2Rad);
        }

        private void FixedUpdate()
        {
            if (_isFlying)
            {
                //竖直速度
                _verticalSpeed = _verticalSpeed - Math.Abs(Gravity) * Time.fixedDeltaTime;

                var zAngle = (float) Math.Atan(_verticalSpeed / _horizontalSpeed) * Mathf.Rad2Deg;

                // Debug.Log("ANGLE:"+ _curAngle);

                _curSpeed                  = (float) Math.Sqrt(_horizontalSpeed * _horizontalSpeed + _verticalSpeed * _verticalSpeed);
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, zAngle);
                // Debug.DrawRay( transform.position,transform.right,Color.cyan,10f);
                transform.Translate(Vector3.right * _curSpeed * Time.fixedDeltaTime, Space.Self);

                _curAngle = zAngle;
                // Debug.Log(this.transform.TransformDirection(this.transform.right));
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer != gameObject.layer)
            {
                Debug.Log("COL!");
                _isFlying = false;
                Invoke("DestroyArrow", 5f);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.isTrigger)
            {
                _isFlying = false;
                Invoke("DestroyArrow", 5f);
                return;
            }

            if (other.gameObject.layer != gameObject.layer && other.gameObject.TryGetComponent(out Unit unitComponent))
            {
                //Debug.Log("TRI!");
                _isFlying = false;
                gameObject.transform.SetParent(other.gameObject.transform);
                unitComponent.BeAttacked(_shooter as IMilitaryUnit);
                Invoke("DestroyArrow", 0.1f);
            }
        }

        /// <summary>
        ///     设置射击者
        /// </summary>
        /// <param name="shooter"></param>
        public void SetShooter(Unit shooter)
        {
            _shooter = shooter;
        }

        private void DestroyArrow()
        {
            Destroy(gameObject);
        }
    }
}