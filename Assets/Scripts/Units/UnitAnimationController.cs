using System;
using UnityEngine;

namespace Units
{
    /// <summary>
    ///     单位动画控制器
    /// </summary>
    public class UnitAnimationController : MonoBehaviour
    {
        /// <summary>
        ///     单位
        /// </summary>
        public Unit unit;

        /// <summary>
        ///     单位动画机
        /// </summary>
        public Animator characterAnimator;

        /// <summary>
        ///     烟雾动画机
        /// </summary>
        public Animator smokeAnimator;

        public SpriteRenderer smokeRenderer;

        /// <summary>
        ///     烟雾判定阈值
        /// </summary>
        public float fastThreshold = 2;

        private void Start()
        {
            unit.UnitDeathEventHandler.AddListener((p, m) =>
                                                   {
                                                       characterAnimator.SetTrigger("Die");
                                                       smokeAnimator.SetTrigger("Die");
                                                   });
            ((IMilitaryUnit) unit).AttackEvent.AddListener(m =>
                                                           {
                                                               try
                                                               {
                                                                   characterAnimator.SetTrigger("Attack");
                                                               }
                                                               catch (Exception e)
                                                               {
                                                                   Console.WriteLine(e);
                                                               }
                                                           });
        }

        private void LateUpdate()
        {
            smokeRenderer.color = new Color(1, 1, 1, Mathf.Clamp01(unit.unitRigidbody.velocity.magnitude - fastThreshold));
        }
    }
}