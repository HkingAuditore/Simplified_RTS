using System.Collections;
using System.Collections.Generic;
using Gameplay.EventSystem;
using Units;
using UnityEngine;

public class FarmerPollution : Farmer
{
    public bool isPollution = false;
    public int pollutionProduct;

    public override void Start()
    {
        base.Start();
        this.pollutionProduct = this.maxLoad[(int)this.road];
        if (this.sidePlayer.gameObject.name == "BPlayer")
        {
            this.isPollution = true;
        }

    }

    protected override void Work()
    {
        Debug.Log("Pollution In Work");
        base.Work();
        GlobalEventManager.GetManager.bSide.InstantiateFarmer(this.road,this.transform,pollutionProduct);
    }
}
