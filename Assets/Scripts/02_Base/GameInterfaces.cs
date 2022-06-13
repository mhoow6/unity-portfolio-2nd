public interface IEventCallable
{
    public void DisposeEvents();
}

public interface IGameSystem
{
    public void Init();
    public void Tick();
}
