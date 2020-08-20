using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class SpriteRendererPlus : MonoBehaviour
{
    private int _isAside;
    private NavMeshAgent _parentAgent;

    public bool isUnit;
    void Start()
    {
        if (isUnit)
        {
            _parentAgent = this.transform.parent.GetComponent<NavMeshAgent>();
            _isAside = LayerMask.LayerToName(this.transform.parent.gameObject.layer) == "ASide" ? 1 : -1;

        }
        this.GetComponent<SpriteRenderer>().shadowCastingMode = ShadowCastingMode.On;
    }

    private void Update()
    {
        if (isUnit)
        {
            int isForward = _parentAgent.velocity.x < 0 ? 1 : -1;
            // Debug.Log(isForward);
            this.transform.eulerAngles = new Vector3(0, isForward * 180 * (_isAside == 1 ? 0 : -1) - this.transform.parent.transform.eulerAngles.y + _isAside * 90f ,0);
        }
    }
}
