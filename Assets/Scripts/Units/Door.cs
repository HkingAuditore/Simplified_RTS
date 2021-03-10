using Units;
using UnityEngine;

public class Door : Unit
{
    private string _enemyLayer;

    private void Awake()
    {
        HP          = sidePlayer.HP;
        _enemyLayer = LayerMask.LayerToName(gameObject.layer) == "ASide" ? "BSide" : "ASide";
    }

    private new void Update()
    {
        sidePlayer.HP = HP;
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