using System;
using UnityEngine;

namespace Units
{
    public class NeutralUnits : Unit
    {
        protected override void Awake()
        {
            unitRigidbody      = GetComponent<Rigidbody>();
            UnitDeathEventHandler += (p, m) =>
                                     {
                                         p.ChangeResource(Resource.Gold, this.deathReward);
                                     };
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
