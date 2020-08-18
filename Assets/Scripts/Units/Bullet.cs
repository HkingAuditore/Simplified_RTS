using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static float Gravity { get; } = 0.01f;

    public float initSpeed;


    private void Start()
    {
        _curSpeed = initSpeed;
        _curAngle = this.transform.rotation.z;
    }

    private float _curSpeed;
    private float _curAngle;
    private bool _isFlying = true;
    
    private void FixedUpdate()
    {
        if (_isFlying)
        {
            
            //水平速度
            float horizontalSpeed = _curSpeed * (float)Math.Sin(_curAngle *Mathf.Deg2Rad );
            //竖直速度
            float verticalSpeed = _curSpeed * (float)Math.Cos(_curAngle*Mathf.Deg2Rad ) - Gravity * Time.deltaTime;

            _curAngle = (float)Math.Atan(verticalSpeed / horizontalSpeed) * Mathf.Rad2Deg;
            _curSpeed = (float)Math.Sqrt(horizontalSpeed * horizontalSpeed + verticalSpeed * verticalSpeed);
        
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.x, this.transform.rotation.y, _curAngle);
            this.transform.Translate(this.transform.right * _curSpeed * Time.deltaTime);
            
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("COL!");
        this._isFlying = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRI!");
        this._isFlying = false;
        this.gameObject.transform.SetParent(other.gameObject.transform);
    }
}
