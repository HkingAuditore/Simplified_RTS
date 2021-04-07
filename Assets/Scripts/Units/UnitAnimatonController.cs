﻿using System;
using System.Collections;
using System.Collections.Generic;
using Units;
using UnityEngine;

public class UnitAnimatonController : MonoBehaviour
{
    public Unit           unit;
    public Animator       characterAnimator;
    public Animator       smokeAnimator;
    public SpriteRenderer smokeRenderer;
    public float          fastThreshold = 2;

    private void Start()
    {
        unit.UnitDeathEventHandler.AddListener( ((p, m) =>
                                                 {
                                                     characterAnimator.SetTrigger("Die");
                                                     smokeAnimator.SetTrigger("Die");
                                                 }));
    }

    private void Update()
    {
        smokeRenderer.color = new Color(1, 1, 1, Mathf.Clamp01(unit.unitRigidbody.velocity.magnitude - fastThreshold));
    }
}