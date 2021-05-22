using System;
using Player;
using UnityEngine;

namespace Units
{
    /// <summary>
    ///     中立单位
    /// </summary>
    public class NeutralUnits : Unit
    {
        /// <summary>
        ///     被攻击效果
        /// </summary>
        public GameObject attackedEffect;

        /// <summary>
        ///     死亡效果
        /// </summary>
        public GameObject deathEffect;

        private int _maxHp;
        protected override void Awake()
        {
            _maxHp        = this.HP;
            unitRigidbody = GetComponent<Rigidbody>();
            UnitDeathEventHandler.AddListener((p, m) => { p.ChangeResource(GameResourceType.Gold, deathReward); });
            BeAttackedEventHandler.AddListener(mUnit =>
                                               {
                                                   var momentum = mUnit.GetUnit().unitRigidbody.velocity.magnitude *
                                                                        mUnit.GetUnit().unitRigidbody.mass;
                                                   var damage = momentum - defence > 0 ? momentum - defence : 1;

                                                   var mainModule = attackedEffect.GetComponent<ParticleSystem>().main;
                                                   mainModule.maxParticles = (int)Mathf.Lerp(0f, 1000f, damage/this._maxHp);
                                                   var effect = Instantiate(attackedEffect,            transform.position,
                                                                            Quaternion.Euler(0, 0, 0), transform.parent);
                                                   Destroy(effect, 5f);
                                               });
            UnitDeathEventHandler.AddListener((player, mUnit) =>
                                              {
                                                  Instantiate(deathEffect,               transform.position,
                                                              Quaternion.Euler(0, 0, 0), transform.parent);
                                              });
        }

        private void OnCollisionEnter(Collision other)
        {
            try
            {
                BeAttacked(other.gameObject.GetComponent<IMilitaryUnit>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        ///     被攻击
        /// </summary>
        /// <param name="attacker"></param>
        public override void BeAttacked(IMilitaryUnit attacker)
        {
            var momentum = attacker.GetUnit().unitRigidbody.velocity.magnitude *
                           attacker.GetUnit().unitRigidbody.mass;
            var damage = momentum - defence > 0 ? momentum - defence : 1;
            // Debug.Log("Damage:" + damage);
            _attacker = attacker;

            HP -= (int) damage;
            BeAttackedEventHandler?.Invoke(attacker);
        }
    }
}