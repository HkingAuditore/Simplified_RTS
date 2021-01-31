using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.EventSystem
{
    public class GlobalEventManager : MonoBehaviour
    {
        private static GlobalEventManager _globalEventManager;
        public static GlobalEventManager GetManager{
            get
            {
                return _globalEventManager;
            }
        }

        private void Awake()
        {
            _globalEventManager = this;
        }

        public Player aSide;
        public Player bSide;

        public List<GlobalEventArgs> globalEventArgsList = new List<GlobalEventArgs>();

        // void Start()
        // {
        //     RegisterGlobalEvent(new GlobalEventArgs(true,
        //         1f,
        //         ((a, b, args) => { Debug.Log(args.Content + a.Food); }),
        //         true,
        //         3f,
        //         ((a, b, args) => { Debug.Log(args.Content + b.Food); }),
        //         new EventTransferArgs("Global Event Test: ")));
        // }

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
                {
                    StartCoroutine(TimeLimitGlobalEvent(globalEventArgs.LimitTime, globalEventArgs));
                }
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