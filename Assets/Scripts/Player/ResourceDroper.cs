using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ResourceDroper : MonoBehaviour
{
   public                                        float            dropColdTime;
   public                                        int              dropValue;
   public                                        int              dropCountMax;
   public                                        int              dropCountMin;
   [FormerlySerializedAs("resourceType")] public GameResourceType gameResourceTypeType;
   public                                        GameObject       dropPrefab;
   public                                        GameObject       dropArea;

   private Vector3[] _dropArea;
   private float     _xMin;
   private float     _xMax;
   private float     _zMin;
   private float     _zMax;

   private void Start()
   {
      Mesh      mesh     = dropArea.GetComponent<MeshFilter>().mesh;
      _dropArea = mesh.vertices;
      _xMin     = _dropArea.Min(p => p.x);
      _xMax     = _dropArea.Max(p => p.x);
      _zMin     = _dropArea.Min(p => p.z);
      _zMax     = _dropArea.Max(p => p.z);
      Debug.Log("points drop area:" + _dropArea[0]);
 
      StartCoroutine(Wait2Drop(dropColdTime));
   }
   
   IEnumerator Wait2Drop(float waitTime) {
      yield return new WaitForSeconds(waitTime);
      DropResource();
      StartCoroutine(Wait2Drop(dropColdTime));
   }

   private Vector3[] RandomPointInArea(int count = 1)
   {
      Vector3[] points = new Vector3[count];

      for (int i = 0; i < count; i++)
      {
         // Debug.Log("random point x Sum:" + xSum);
         // Debug.Log("random point y Sum:" + ySum);
         // Debug.Log("random point z Sum:" + zSum);

         points[i] = new Vector3(Random.Range(_xMin,_xMax),
                                 dropArea.transform.position.y,
                                 Random.Range(_zMin, _zMax)
                                 );
         Debug.Log("random point x Sum:" + points[i]);

      }

      return points;
   }

   public void DropResource()
   {
      Vector3[] dropPoints = RandomPointInArea(Random.Range(dropCountMin, dropCountMax + 1));
      for (int i = 0; i < dropPoints.Length; i++)
      {
         var obj = GameObject.Instantiate(dropPrefab, dropPoints[i], Quaternion.Euler(0, 0, 0), this.transform);
         obj.GetComponent<ResourceDroperUI>().resourceDroper = this;
      }
   }

   public void CollectResource()
   {
      GameManager.GameManager.GetManager.aSide.ChangeResource(gameResourceTypeType,dropValue);
   }
}
