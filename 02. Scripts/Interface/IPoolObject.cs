using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    public GameObject GameObject { get; }
    public PoolType   PoolType   { get; }
    public int        PoolSize   { get; }
}