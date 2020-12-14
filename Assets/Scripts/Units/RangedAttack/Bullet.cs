using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static float Gravity { get; } = 6f;

    public float initSpeed;

    private Unit _shooter;


    private void Start()
    {
        _curSpeed = initSpeed;
        _isFlying = true;
        _curAngle = this.transform.eulerAngles.z;

        //水平速度
        _horizontalSpeed = _curSpeed * (float) Math.Sin(_curAngle * Mathf.Deg2Rad);
        //竖直速度
        _verticalSpeed = _curSpeed * (float) Math.Cos(_curAngle * Mathf.Deg2Rad);

    }

    private float _curSpeed;
    private float _curAngle;
    private bool _isFlying;

    private float _horizontalSpeed;
    private float _verticalSpeed;

    private void FixedUpdate()
    {
        if (_isFlying)
        {
            //竖直速度
            _verticalSpeed = _verticalSpeed - Math.Abs(Bullet.Gravity) * Time.fixedDeltaTime;

            float zAngle = (float) Math.Atan(_verticalSpeed/_horizontalSpeed)* Mathf.Rad2Deg;
            
            // Debug.Log("ANGLE:"+ _curAngle);

            _curSpeed = (float) Math.Sqrt(_horizontalSpeed * _horizontalSpeed + _verticalSpeed * _verticalSpeed);
            this.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x,this.transform.eulerAngles.y,zAngle);
            this.transform.Translate(Vector3.right * _curSpeed * Time.fixedDeltaTime,Space.Self);

            _curAngle = zAngle;
            // Debug.Log(this.transform.TransformDirection(this.transform.right));

        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != this.gameObject.layer)
        {
            Debug.Log("COL!");
            this._isFlying = false;
            Invoke("DestroyArrow", 5f);
  
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            this._isFlying = false;
            Invoke("DestroyArrow", 5f);
            return;
        }
        if (other.gameObject.layer != this.gameObject.layer && other.gameObject.TryGetComponent(out Unit unitComponent))
        {
            //Debug.Log("TRI!");
            this._isFlying = false;
            this.gameObject.transform.SetParent(other.gameObject.transform);
            unitComponent.BeAttacked(_shooter as IMilitaryUnit);
            Invoke("DestroyArrow",0.1f);
        }

    }


    public void SetShooter(Unit shooter) => _shooter = shooter;
    private void DestroyArrow()
    {
        Destroy(this.gameObject);
    }
}