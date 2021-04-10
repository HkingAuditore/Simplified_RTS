using UnityEngine;

public class ShootCalculatorUI : MonoBehaviour
{
    public  Rigidbody      shootRigidbody;
    public  PhysicMaterial groundMaterial;
    public  PhysicMaterial wallMaterial;
    private LineRenderer   _lineRenderer;

    private void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    public void CalculateShootLine(Vector3 oriPos, Vector3 force, int sampleTimes)
    {
        var ori         = oriPos;
        var oriVelocity = force / shootRigidbody.mass;
        for (var i = 0; i < sampleTimes; i++)
        {
            var mass = shootRigidbody.mass;
            var f    = groundMaterial.dynamicFriction * mass * Physics.gravity;
            var t    = oriVelocity.magnitude          / (f.magnitude / mass);
            var s    = oriVelocity * t - .5f * (f / mass) * t * t;
            
        }
    }
}