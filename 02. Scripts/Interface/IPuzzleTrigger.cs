public interface IPuzzleTrigger
{
    public int  ObjectIndex { get; }
    public void TriggerEnter();
    public void TriggerExit();
}