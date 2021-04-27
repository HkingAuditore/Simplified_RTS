using UnityEngine;

namespace Controllers.Camera
{
    /// <summary>
    /// 相机控制器
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        private static readonly float   _border = 3.84f;
        private                 Vector3 _leftBorder;
        private                 Vector3 _rightBorder;

        private void Awake()
        {
            var position = transform.position;
            _leftBorder  = new Vector3(_border,  position.y, position.z);
            _rightBorder = new Vector3(-_border, position.y, position.z);
        }

        private void Update()
        {
            if (Input.mousePosition.x < Screen.width * 0.2f)
                transform.position = Vector3.Lerp(transform.position, _rightBorder, Time.deltaTime * 1.5f);
            else if (Input.mousePosition.x > Screen.width * 0.8f)
                transform.position = Vector3.Lerp(transform.position, _leftBorder, Time.deltaTime * 1.5f);
        }
    }
}