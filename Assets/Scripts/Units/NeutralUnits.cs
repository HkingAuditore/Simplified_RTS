using System;
using System.Security.Cryptography;
using Player;
using UnityEngine;

namespace Units
{
    public class NeutralUnits : Unit
    {

        public GameObject attackedEffect;
        public GameObject deathEffect;
        protected override void Awake()
        {
            unitRigidbody      = GetComponent<Rigidbody>();
            UnitDeathEventHandler.AddListener((p, m) =>
            {
                p.ChangeResource(GameResourceType.Gold, this.deathReward);
            });
            this.BeAttackedEventHandler.AddListener((mUnit =>
                                                     {
                                                         var effect = Instantiate(attackedEffect,            this.transform.position,
                                                                                  Quaternion.Euler(0, 0, 0), this.transform.parent);
                                                         Destroy(effect, 5f);
                                                     }));
            this.UnitDeathEventHandler.AddListener(((player, mUnit) =>
                                                    {
                                                        Instantiate(deathEffect,               this.transform.position,
                                                                    Quaternion.Euler(0, 0, 0), this.transform.parent);

                                                    }));
        }
        

        public override void BeAttacked(IMilitaryUnit attacker)
        {

            float momentum = attacker.GetUnit().unitRigidbody.velocity.magnitude *
                             attacker.GetUnit().unitRigidbody.mass;
            var damage = ((momentum - defence) > 0) ? (momentum - defence) : 1;
            // Debug.Log("Damage:" + damage);
            this._attacker = attacker;

            HP             -= (int)damage;
            BeAttackedEventHandler?.Invoke(attacker);

        }

        private void OnCollisionEnter(Collision other)
        {
            try
            {
                this.BeAttacked(other.gameObject.GetComponent<IMilitaryUnit>());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

            }
        }
        
    }
}
