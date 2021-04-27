using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.EventSystem
{
    /// <summary>
    /// 全局事件管理
    /// </summary>
    public class GlobalEventManager : MonoBehaviour
    {
        /// <summary>
        /// 我方
        /// </summary>
        public Player.Player aSide;
        /// <summary>
        /// 敌方
        /// </summary>
        public Player.Player bSide;
        /// <summary>
        /// 事件执行列表
        /// </summary>
        public List<GlobalEventArgs> globalEventArgsList = new List<GlobalEventArgs>();

        public static GlobalEventManager GetManager { get; private set; }

        
        private void Awake()
        {
            GetManager = this;
        }
        
        /// <summary>
        /// 注册全局事件
        /// </summary>
        /// <param name="globalEventArgs"></param>
        public void RegisterGlobalEvent(GlobalEventArgs globalEventArgs)
        {
            //启动延时检查
            if (!globalEventArgs.IsDelay)
            {
                globalEventArgsList.Add(globalEventArgs);
                GlobalEventInit(globalEventArgs);
            }
            else
            {
                StartCoroutine(DelayGlobalEvent(globalEventArgs.DelayTime, globalEventArgs));
            }
        }

        private IEnumerator DelayGlobalEvent(float delayTime, GlobalEventArgs globalEventArgs)
        {
            Debug.Log("Delay Start");
            yield return new WaitForSeconds(delayTime);
            Debug.Log("Delay End");
            globalEventArgsList.Add(globalEventArgs);
            GlobalEventInit(globalEventArgs);
        }

        private void GlobalEventInit(GlobalEventArgs globalEventArgs)
        {
            if (globalEventArgsList.Contains(globalEventArgs))
            {
                globalEventArgs.InitEventFunction(aSide, bSide, globalEventArgs.Args);
                globalEventArgs.State = EventState.OnGoing;
                if (globalEventArgs.IsTimeLimitation)
                    StartCoroutine(TimeLimitGlobalEvent(globalEventArgs.LimitTime, globalEventArgs));
            }
            else
            {
                throw new Exception("事件表中找不到目标事件");
            }
        }

        private IEnumerator TimeLimitGlobalEvent(float duration, GlobalEventArgs globalEventArgs)
        {
            Debug.Log("Event Start");
            yield return new WaitForSeconds(duration);
            Debug.Log("Event End");
            GlobalEventFinish(globalEventArgs);
        }

        private void GlobalEventFinish(GlobalEventArgs globalEventArgs)
        {
            if (globalEventArgsList.Contains(globalEventArgs))
            {
                globalEventArgs.FinishEventFunction(aSide, bSide, globalEventArgs.Args);
                globalEventArgs.State = EventState.Finished;
                globalEventArgsList.Remove(globalEventArgs);
            }
            else
            {
                throw new Exception("事件表中找不到目标事件");
            }
        }
    }
}