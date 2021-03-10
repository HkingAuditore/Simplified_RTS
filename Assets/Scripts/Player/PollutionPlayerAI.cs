public class PollutionPlayerAI : PlayerAI
{
    public override void Start()
    {
        base.Start();
    }

    public void FixedUpdate()
    {
        updateEventHandler?.Invoke();
    }

    protected override void AIGaming()
    {
        CountEnemy();
        DispatchUnits();
        _isColdDown = true;
        Invoke("RestEnd", aiRestTime);
    }
}