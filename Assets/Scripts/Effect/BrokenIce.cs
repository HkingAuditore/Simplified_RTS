using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BrokenIce : MonoBehaviour
{
    public List<Rigidbody> iceRigidbodies = new List<Rigidbody>();
    public Transform       explosionPos;
    public float           explosionForce;
    public float           explosionRadius;
    void Start()
    {
        iceRigidbodies.ForEach(r =>
                               {
                                   r.AddExplosionForce(explosionForce, explosionPos.position, explosionRadius);
                                   Destroy(r.gameObject,5f);
                               });
        Destroy(this.gameObject, 6f);
    }

    [ContextMenu("Get Ice List")]
    public void GetIceList()
    {
        iceRigidbodies = this.transform.GetComponentsInChildren<Rigidbody>().ToList();
    }

}
