using System.Collections.Generic;
using Gameplay.EventSystem;
using Gameplay.ScienceTree;
using UnityEngine;

public class ScienceTreeNode : MonoBehaviour
{
    public List<ScienceTreeNode> previousNodes  = new List<ScienceTreeNode>();
    public List<ScienceTreeNode> afterwardNodes = new List<ScienceTreeNode>();

    public string nodeName;
    public int    woodCost;
    public int    foodCost;
    public int    goldCost;
    public bool   isActive;

    public ScienceTreeBasic scienceTree;

    public GlobalEventArgs nodeEvent = new GlobalEventArgs(true,
                                                           1f,
                                                           (a, b, args) => { Debug.Log(args.Content + a.Food); },
                                                           true,
                                                           3f,
                                                           (a, b, args) => { Debug.Log(args.Content + b.Food); },
                                                           new EventTransferArgs("Node Global Event Test: "));

    public bool IsActivatable()
    {
        return !previousNodes.Find(node => !node.isActive);
    }

    public bool Activate()
    {
        if (!isActive && IsActivatable())
        {
            isActive = true;

            scienceTree.globalEventManager.RegisterGlobalEvent(nodeEvent);
        }

        return isActive;
    }
}