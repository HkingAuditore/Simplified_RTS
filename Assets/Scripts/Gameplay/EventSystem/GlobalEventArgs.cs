namespace Gameplay.EventSystem
{
    /// <summary>
    /// 全局事件委托
    /// </summary>
    /// <param name="aSide"></param>
    /// <param name="bSide"></param>
    /// <param name="args"></param>
    public delegate void GlobalEventHandler(Player.Player aSide, Player.Player bSide, EventTransferArgs args);

    /// <summary>
    /// 事件执行状态
    /// </summary>
    public enum EventState
    {
        /// <summary>
        /// 未开始
        /// </summary>
        NotStarted,
        /// <summary>
        /// 正在进行
        /// </summary>
        OnGoing,
        /// <summary>
        /// 已完成
        /// </summary>
        Finished
    }

    /// <summary>
    /// 全局事件传递
    /// </summary>
    public class GlobalEventArgs
    {
        /// <summary>
        /// 携带数据
        /// </summary>
        public EventTransferArgs  Args;
        /// <summary>
        /// 延迟执行时间
        /// </summary>
        public float              DelayTime;
        /// <summary>
        /// 完成后事件
        /// </summary>
        public GlobalEventHandler FinishEventFunction;
        /// <summary>
        /// 初始化事件
        /// </summary>
        public GlobalEventHandler InitEventFunction;
        /// <summary>
        /// 是否延时执行
        /// </summary>
        public bool               IsDelay;
        /// <summary>
        /// 是否为限时事件
        /// </summary>
        public bool               IsTimeLimitation;
        /// <summary>
        /// 限时长度
        /// </summary>
        public float              LimitTime;
        /// <summary>
        /// 事件执行状态
        /// </summary>
        public EventState         State = EventState.NotStarted;

        public GlobalEventArgs(bool isDelay, float delayTime, GlobalEventHandler initEventFunction,
                               bool isTimeLimitation, float limitTime, GlobalEventHandler finishEventFunction,
                               EventTransferArgs args)
        {
            IsDelay             = isDelay;
            DelayTime           = delayTime;
            InitEventFunction   = initEventFunction;
            IsTimeLimitation    = isTimeLimitation;
            LimitTime           = limitTime;
            FinishEventFunction = finishEventFunction;
            Args                = args;
        }
    }

    public class EventTransferArgs
    {
        
        public string Content;

        public EventTransferArgs(string content)
        {
            Content = content;
        }
    }
}