using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Resource
{
    Food,Wood,Gold
}

public class Player : MonoBehaviour
{
    public int Food { get; private set; }
    public int Wood { get; private set; }
    public int Gold { get; private set; }
    public int HP { get; private set; }

    private void Start()
    {
        this.Food = 10;
        this.Wood = 10;
        this.Gold = 0;
        this.HP = 100;
    }

    public void ChangeResource(Resource resource, int count)
    {
        switch (resource)
        {
            case Resource.Food:
                this.Food += count;
                break;
            case Resource.Wood:
                this.Wood += count;
                break;
            case Resource.Gold:
                this.Gold += count;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(resource), resource, null);
        }
    }
}
