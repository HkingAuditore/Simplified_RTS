using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private static float _border = 3.84f;
    private Vector3 _leftBorder ;
    private Vector3 _rightBorder;

    private void Awake()
    {
        var position = this.transform.position;
        _leftBorder = new Vector3(_border,position.y,position.z);
        _rightBorder = new Vector3(-_border,position.y,position.z);
    }

    void Update()
    {
        if (Input.mousePosition.x < Screen.width * 0.2f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, _rightBorder, Time.deltaTime*1.5f);
        }else if (Input.mousePosition.x > Screen.width * 0.8f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, _leftBorder, Time.deltaTime*1.5f);
        }
    }
}
