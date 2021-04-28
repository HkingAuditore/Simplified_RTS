using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Effect
{
    /// <summary>
    ///     冰块破碎
    /// </summary>
    public class BrokenIce : MonoBehaviour
    {
        /// <summary>
        ///     碎冰刚体
        /// </summary>
        public List<Rigidbody> iceRigidbodies = new List<Rigidbody>();

        /// <summary>
        ///     爆炸点
        /// </summary>
        public Transform explosionPos;

        /// <summary>
        ///     爆炸力度
        /// </summary>
        public float explosionForce;

        /// <summary>
        ///     爆炸范围
        /// </summary>
        public float explosionRadius;

        private void Start()
        {
            iceRigidbodies.ForEach(r =>
                                   {
                                       r.AddExplosionForce(explosionForce, explosionPos.position, explosionRadius);
                                       Destroy(r.gameObject, 5f);
                                   });
            Destroy(gameObject, 6f);
        }

        /// <summary>
        ///     抓取冰块列表
        /// </summary>
        [ContextMenu("Get Ice List")]
        public void GetIceList()
        {
            iceRigidbodies = transform.GetComponentsInChildren<Rigidbody>().ToList();
        }
    }
}