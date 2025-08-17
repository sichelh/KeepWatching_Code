using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PuzzleController : MonoBehaviour, IPuzzle
{
    public abstract bool IsSolved { get; protected set; }
    public event Action OnPuzzleComplete = delegate { };

    public virtual void StartPuzzle() { }
    public virtual void ResetPuzzle() { }

    protected void Complete()
    {
        IsSolved = true;
        OnPuzzleComplete.Invoke();
    }
}