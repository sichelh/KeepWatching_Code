using UnityEngine;

public interface IInteractable
{
    public Outline     Outline       { get; }
    public IVisibility VisibilityObj { get; }
    public void        Execute();
    public void        Exit();
}