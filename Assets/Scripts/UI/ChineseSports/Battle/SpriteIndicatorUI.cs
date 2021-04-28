using UnityEngine;

namespace UI.ChineseSports.Battle
{
    public class SpriteIndicatorUI : MonoBehaviour
    {
        public  SpriteRenderer spriteRenderer;
        private Sprite         _unitSprite;

        public Sprite UnitSprite
        {
            get => _unitSprite;
            set
            {
                _unitSprite           = value;
                spriteRenderer.sprite = UnitSprite;
            }
        }


        private void Update()
        {
            var        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            var        layerMask = 1 << 11;

            if (Physics.Raycast(ray, out rayHit, 50f, layerMask))
            {
                //初始位置
                var pos = new Vector3(rayHit.point.x, rayHit.point.y, rayHit.point.z);
                transform.position = pos;
            }
        }
    }
}