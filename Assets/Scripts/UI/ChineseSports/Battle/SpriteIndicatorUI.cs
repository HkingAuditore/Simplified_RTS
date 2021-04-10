using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteIndicatorUI : MonoBehaviour
{
    private Sprite _unitSprite;

    public SpriteRenderer spriteRenderer;

    public Sprite UnitSprite
    {
        get => _unitSprite;
        set
        {
            _unitSprite             = value;
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
            this.transform.position = pos;
        }
    }
}
