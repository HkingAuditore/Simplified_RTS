using Gameplay.ScienceTree;
using UnityEngine;

public class ScienceTreeUI : MonoBehaviour
{
    public ScienceTreeBasic ScienceTreeBasic;


    public ScienceTreeNodeUI FindNodeUI(ScienceTreeNode node)
    {
        return gameObject.transform.Find(node.nodeName).GetComponent<ScienceTreeNodeUI>();
    }
}