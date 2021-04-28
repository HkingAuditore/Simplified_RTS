using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
   /// <summary>
   ///     资源掉落
   /// </summary>
   public class ResourceDroper : MonoBehaviour
    {
       /// <summary>
       ///     掉落冷却时间
       /// </summary>
       public float dropColdTime;

       /// <summary>
       ///     掉落资源值
       /// </summary>
       public int dropValue;

       /// <summary>
       ///     掉落最大量
       /// </summary>
       public int dropCountMax;

       /// <summary>
       ///     掉落最少量
       /// </summary>
       public int dropCountMin;

       /// <summary>
       ///     掉落类型
       /// </summary>
       [FormerlySerializedAs("resourceType")] public GameResourceType gameResourceTypeType;

       /// <summary>
       ///     掉落预制体
       /// </summary>
       public GameObject dropPrefab;

       /// <summary>
       ///     掉落区域
       /// </summary>
       public GameObject dropArea;

        private Vector3[] _dropArea;
        private float     _xMax;
        private float     _xMin;
        private float     _zMax;
        private float     _zMin;

        private void Start()
        {
            var mesh = dropArea.GetComponent<MeshFilter>().mesh;
            _dropArea = mesh.vertices;
            _xMin     = _dropArea.Min(p => p.x);
            _xMax     = _dropArea.Max(p => p.x);
            _zMin     = _dropArea.Min(p => p.z);
            _zMax     = _dropArea.Max(p => p.z);
            Debug.Log("points drop area:" + _dropArea[0]);

            StartCoroutine(Wait2Drop(dropColdTime));
        }

        private IEnumerator Wait2Drop(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            DropResource();
            StartCoroutine(Wait2Drop(dropColdTime));
        }

        private Vector3[] RandomPointInArea(int count = 1)
        {
            var points = new Vector3[count];

            for (var i = 0; i < count; i++)
            {
                // Debug.Log("random point x Sum:" + xSum);
                // Debug.Log("random point y Sum:" + ySum);
                // Debug.Log("random point z Sum:" + zSum);

                points[i] = new Vector3(Random.Range(_xMin, _xMax),
                                        dropArea.transform.position.y,
                                        Random.Range(_zMin, _zMax)
                                       );
                Debug.Log("random point x Sum:" + points[i]);
            }

            return points;
        }

        /// <summary>
        ///     抛出资源
        /// </summary>
        public void DropResource()
        {
            var dropPoints = RandomPointInArea(Random.Range(dropCountMin, dropCountMax + 1));
            for (var i = 0; i < dropPoints.Length; i++)
            {
                var obj = Instantiate(dropPrefab, dropPoints[i], Quaternion.Euler(0, 0, 0), transform);
                obj.GetComponent<ResourceDroperUI>().resourceDroper = this;
            }
        }

        /// <summary>
        ///     收集资源
        /// </summary>
        public void CollectResource()
        {
            GameManager.GameManager.GetManager.aSide.ChangeResource(gameResourceTypeType, dropValue);
        }
    }
}