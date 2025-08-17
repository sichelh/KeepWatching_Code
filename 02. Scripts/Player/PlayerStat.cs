using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatBase
{
    public StatType Type { get; protected set; }
    public Action<float> OnValueChanged;

    public StatBase(StatType type)
    {
        Type = type;
    }

    public abstract float GetCurrent();
}

public class CalculatedStat : StatBase
{
    public float BaseValue   { get; private set; }
    public float BuffFlat    { get; private set; }
    public float BuffPercent { get; private set; }
    public float EquipValue  { get; private set; }

    public float FinalValue => Mathf.Max((BaseValue + BuffFlat + EquipValue) * (1 + BuffPercent), 0);

    public CalculatedStat(StatType type, float baseValue) : base(type)
    {
        BaseValue = baseValue;
    }

    public void ModifyBaseValue(float value)
    {
        BaseValue += value;
        OnValueChanged?.Invoke(FinalValue);
    }

    public void ModifyBuffFlat(float value)
    {
        BuffFlat += value;
        OnValueChanged?.Invoke(FinalValue);
    }

    public void ModifyBuffPercent(float value)
    {
        BuffPercent += value;
        OnValueChanged?.Invoke(FinalValue);
    }

    public void ModifyEquipmentValue(float value)
    {
        EquipValue += value;
        OnValueChanged?.Invoke(FinalValue);
    }

    public override float GetCurrent() => FinalValue;
}

public class ResourceStat : StatBase
{
    public float CurrentValue { get; private set; }
    public float MaxValue     { get; private set; }


    public ResourceStat(StatType type, float maxValue) : base(type)
    {
        CurrentValue = maxValue;
        MaxValue = maxValue;
    }

    public void Recover(float value)
    {
        CurrentValue = Mathf.Min(CurrentValue + value, MaxValue);
        OnValueChanged?.Invoke(CurrentValue);
    }

    public void Consume(float value)
    {
        CurrentValue = Mathf.Max(CurrentValue - value, 0);
        OnValueChanged?.Invoke(CurrentValue);
    }

    public void SetMax(float max)
    {
        MaxValue = max;
        CurrentValue = Mathf.Min(CurrentValue, MaxValue);
        OnValueChanged?.Invoke(CurrentValue);
    }


    public override float GetCurrent() => CurrentValue;
}