using UnityEngine;

namespace Units
{
    /// <summary>
    ///     基地门
    /// </summary>
    public class Door : Unit
    {
        private string _enemyLayer;

        public override void Start()
        {
            base.Start();
            HP          = sidePlayer.HP;
            _enemyLayer = LayerMask.LayerToName(gameObject.layer) == "ASide" ? "BSide" : "ASide";
            // this.UnitDeathEventHandler.AddListener(((p, iM) =>
            //                                         {
            //                                             sidePlayer.HP = 0;
            //                                         }));
        }

        public override void Update()
        {
            sidePlayer.HP = HP;
        }

        private void OnDestroy()
        {
            sidePlayer.HP = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            var unit = other.transform.parent;
            if (unit.gameObject.layer == 1 << LayerMask.NameToLayer(_enemyLayer))
            {
                Debug.Log(unit.gameObject.name + " At Door!");
                unit.gameObject.GetComponent<Unit>().IsAtEnemyDoor = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            var unit = other.transform.parent;
            if (unit.gameObject.layer == 1 << LayerMask.NameToLayer(_enemyLayer))
                unit.gameObject.GetComponent<Unit>().IsAtEnemyDoor = false;
        }
    }
}