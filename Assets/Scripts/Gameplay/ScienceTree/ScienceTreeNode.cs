using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.EventSystem;
using Gameplay.ScienceTree;
using UnityEngine;

public class ScienceTreeNode : MonoBehaviour
{
    public List<ScienceTreeNode> previousNodes = new List<ScienceTreeNode>();
    public List<ScienceTreeNode> afterwardNodes = new List<ScienceTreeNode>();

    public string nodeName;
    public int woodCost;
    public int foodCost;
    public int goldCost;
    public bool isActive = false;

    public GlobalEventArgs nodeEvent = new GlobalEventArgs(true,
        1f,
        ((a, b, args) => { Debug.Log(args.Content + a.Food); }),
        true,
        3f,
        ((a, b, args) => { Debug.Log(args.Content + b.Food); }),
        new EventTransferArgs("Node Global Event Test: "));

    public ScienceTreeBasic scienceTree;

    public bool IsActivatable() => !previousNodes.Find((node => !node.isActive));

    public bool Activate()
    {
        if (!this.isActive && this.IsActivatable())
        {
            this.isActive = true;

            this.scienceTree.globalEventManager.RegisterGlobalEvent(nodeEvent);
        }

        return this.isActive;
    }
}
