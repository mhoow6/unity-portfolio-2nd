public interface ISubscribable
{
    public void DisposeEvents();
}
public interface IAlarmReactable
{
    public void React(AlarmEvent alarmEvent);
}