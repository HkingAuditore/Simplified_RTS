using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay.ScienceTree;
using UnityEngine;

public class ScienceTreeUI : MonoBehaviour
{
    public ScienceTreeBasic ScienceTreeBasic;
    


    public ScienceTreeNodeUI FindNodeUI(ScienceTreeNode node) => this.gameObject.transform.Find(node.nodeName).GetComponent<ScienceTreeNodeUI>();
}
