using System.Collections.Generic;
using UnityEngine;

namespace UI.ChineseSports.Battle
{
    public class ShootCalculatorUI : MonoBehaviour
    {
        public Rigidbody      shootRigidbody;
        public Vector3        oriVelocity;
        public Vector3        oriPos;
        public Vector3        oriScreenPos;
        public PhysicMaterial groundMaterial;
        public PhysicMaterial wallMaterial;

        private LineRenderer _lineRenderer;

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void FixedUpdate()
        {
            var nowPos      = Input.mousePosition;
            var oriVelocity = -(nowPos - oriScreenPos) * .1f;
            oriVelocity = new Vector3(oriVelocity.x, 0, oriVelocity.y);

            this.oriVelocity = oriVelocity;
            CalculateShootLine(oriPos, 3);
        }

        private Vector3 GetVector3WithConstantY(Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public void CalculateShootLine(Vector3 oriPos, int sampleTimes)
        {
            var hitPoints = new List<Vector3>();
            var velocity  = oriVelocity;
            hitPoints.Add(GetVector3WithConstantY(oriPos, .5f));
            for (var i = 0; i < sampleTimes; i++)
            {
                var        mass = shootRigidbody.mass;
                var        f    = groundMaterial.dynamicFriction          * mass * Physics.gravity;
                var        s    = velocity.magnitude * velocity.magnitude / (2 * (f.magnitude / mass));
                var        ray  = new Ray(hitPoints[hitPoints.Count - 1], velocity.normalized);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, s, ~(1 << 0), QueryTriggerInteraction.Ignore))
                {
                    hitPoints.Add(GetVector3WithConstantY(hit.point, .5f));
                    velocity = Vector3.Reflect(velocity, hit.normal) * (wallMaterial.bounciness * wallMaterial.bounciness);
                    // Debug.DrawLine(hitPoints[hitPoints.Count-2], hit.point, Color.magenta);
                }
                else
                {
                    hitPoints.Add(GetVector3WithConstantY(hitPoints[i] + velocity.normalized * s, .5f));
                    // Debug.DrawLine(hitPoints[hitPoints.Count -2], hitPoints[hitPoints.Count-1], Color.magenta);
                    break;
                }
            }

            _lineRenderer.positionCount = hitPoints.Count;
            _lineRenderer.SetPositions(hitPoints.ToArray());
        }
    }
}