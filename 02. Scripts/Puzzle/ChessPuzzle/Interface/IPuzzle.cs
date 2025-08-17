using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzle
{
    void StartPuzzle();
    void ResetPuzzle();
    bool IsSolved { get; }
    event Action OnPuzzleComplete;
}
