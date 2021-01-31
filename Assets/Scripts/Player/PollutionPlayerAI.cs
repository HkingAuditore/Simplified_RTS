using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PollutionPlayerAI : PlayerAI
{
    public void FixedUpdate()
    {
        updateEventHandler?.Invoke();
    }

    public override void Start()
    {
        base.Start();
    }

    protected override void AIGaming()
    {
        CountEnemy();
        DispatchUnits();
        _isColdDown = true;
        Invoke("RestEnd", aiRestTime);

    }
}
