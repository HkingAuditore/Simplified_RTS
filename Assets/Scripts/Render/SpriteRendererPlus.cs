﻿using Units;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class SpriteRendererPlus : MonoBehaviour
{
    public  bool isUnit;
    public  int  _isAside;
    private Unit _parentUnit;

    [ExecuteAlways]
    private void Start()
    {
        if (isUnit)
        {
            _parentUnit = transform.parent.GetComponent<Unit>();
            _isAside    = LayerMask.LayerToName(transform.parent.gameObject.layer) == "ASide" ? 1 : -1;
        }

        GetComponent<SpriteRenderer>().shadowCastingMode = ShadowCastingMode.On;
    }

    public bool faceRight;
    private void Update()
    {
        if (isUnit)
        {
            float yDir = _parentUnit.transform.rotation.y;
           faceRight = ((int) yDir % 360 < 180) ? true : false;
            if (_isAside == -1)
            {
                faceRight = !faceRight;
            }
            transform.rotation = Quaternion.Euler(25f * (faceRight ? 1 : -1), faceRight ? 0 : 180, 0);
        }
    }
}