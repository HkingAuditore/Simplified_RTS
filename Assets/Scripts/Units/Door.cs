using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class Door : Unit
{
    
    private string _enemyLayer;
    
    private void Awake()
    {
        this.HP = this.sidePlayer.HP;
        _enemyLayer = LayerMask.LayerToName(this.gameObject.layer) == "ASide" ? "BSide" : "ASide";
    }

    private new void Update()
    {
        this.sidePlayer.HP = this.HP;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == (1 << LayerMask.NameToLayer(_enemyLayer)))
        {
            other.gameObject.GetComponent<Unit>().IsAtEnemyDoor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == (1 << LayerMask.NameToLayer(_enemyLayer)))
        {
            other.gameObject.GetComponent<Unit>().IsAtEnemyDoor = false;
        }

    }
}
