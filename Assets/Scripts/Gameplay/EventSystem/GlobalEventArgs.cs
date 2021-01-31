namespace Gameplay.EventSystem
{
    public delegate void GlobalEventHandler(Player aSide, Player bSide,EventTransferArgs args);

    public enum EventState
    {
        NotStarted,
        OnGoing,
        Finished
    }
    public class GlobalEventArgs
    {
        public EventState State = EventState.NotStarted;
        public bool IsDelay;
        public float DelayTime;
        public GlobalEventHandler InitEventFunction;
        public bool IsTimeLimitation;
        public float LimitTime;
        public GlobalEventHandler FinishEventFunction;
        public EventTransferArgs Args;

        public GlobalEventArgs(bool isDelay, float delayTime, GlobalEventHandler initEventFunction, bool isTimeLimitation, float limitTime, GlobalEventHandler finishEventFunction, EventTransferArgs args)
        {
            IsDelay = isDelay;
            DelayTime = delayTime;
            InitEventFunction = initEventFunction;
            IsTimeLimitation = isTimeLimitation;
            LimitTime = limitTime;
            FinishEventFunction = finishEventFunction;
            Args = args;
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
