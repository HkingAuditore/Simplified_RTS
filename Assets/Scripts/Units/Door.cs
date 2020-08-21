using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class Door : Unit
{
    private void Awake()
    {
        this.HP = this.sidePlayer.HP;
    }

    private void Update()
    {
        this.sidePlayer.HP = this.HP;
    }
}
