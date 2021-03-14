using Gameplay.EventSystem;
using Units;
using UnityEngine;

public class FarmerPollution : Farmer
{
    public bool isPollution;
    public int  pollutionProduct;

    public override void Start()
    {
        base.Start();
        pollutionProduct = maxLoad[(int) road];
        if (sidePlayer.gameObject.name == "BPlayer") isPollution = true;
    }

    protected override void Work()
    {
        Debug.Log("Pollution In Work");
        base.Work();
        GlobalEventManager.GetManager.bSide.InstantiateFarmer(road, transform, pollutionProduct);
    }
}