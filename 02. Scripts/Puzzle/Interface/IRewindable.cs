using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRewindable
{
    void Record();
    void Rewind(float t);
    void OnRewindStart();
    void OnRewindComplete();
}
