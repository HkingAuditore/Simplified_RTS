using System.Collections.Generic;
using Gameplay.EventSystem;
using UnityEngine;

namespace Gameplay.ScienceTree
{
    /// <summary>
    /// 科技树节点
    /// </summary>
    public class ScienceTreeNode : MonoBehaviour
    {
        /// <summary>
        /// 前置节点
        /// </summary>
        public List<ScienceTreeNode> previousNodes  = new List<ScienceTreeNode>();
        /// <summary>
        /// 后续节点
        /// </summary>
        public List<ScienceTreeNode> afterwardNodes = new List<ScienceTreeNode>();

        /// <summary>
        /// 节点名称
        /// </summary>
        public string nodeName;
        /// <summary>
        /// 木头消耗
        /// </summary>
        public int    woodCost;
        /// <summary>
        /// 食物消耗
        /// </summary>
        public int    foodCost;
        /// <summary>
        /// 黄金消耗
        /// </summary>
        public int    goldCost;
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool   isActive;

        /// <summary>
        /// 所属科技树
        /// </summary>
        public ScienceTreeBasic scienceTree;

        private GlobalEventArgs nodeEvent = new GlobalEventArgs(true,
                                                                1f,
                                                                (a, b, args) => { Debug.Log(args.Content + a.Food); },
                                                                true,
                                                                3f,
                                                                (a, b, args) => { Debug.Log(args.Content + b.Food); },
                                                                new EventTransferArgs("Node Global Event Test: "));

        /// <summary>
        /// 该节点是否可被激活
        /// </summary>
        /// <returns></returns>
        public bool IsActivatable()
        {
            return !previousNodes.Find(node => !node.isActive);
        }

        /// <summary>
        /// 激活
        /// </summary>
        /// <returns></returns>
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
}