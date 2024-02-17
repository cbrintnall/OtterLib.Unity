using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CompoundValueHandle<T>
{
    public T Value;
    public Action Kill;

    public static implicit operator CompoundValueHandle<T>(T val) =>
        new CompoundValueHandle<T>() { Value = val };

    public static implicit operator T(CompoundValueHandle<T> c) => c.Value;

    public override bool Equals(object obj)
    {
        return obj is CompoundValueHandle<T> c && c.Value.Equals(Value);
    }

    public IEnumerator WaitSecondsThenKill(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Kill();
    }
}

public class CompoundValue
{
    public float Value => RecalculateValues();
    public Action<float> ValueChanged;

    private List<CompoundValueHandle<float>> values = new();

    public static implicit operator float(CompoundValue c) => c.Value;

    public CompoundValueHandle<float> Add(float val)
    {
        var handle = new CompoundValueHandle<float>() { Value = val };
        handle.Kill = () =>
        {
            values.Remove(handle);
        };
        values.Add(handle);
        ValueChanged?.Invoke(Value);
        return handle;
    }

    float RecalculateValues()
    {
        return values.Sum(handle => handle.Value);
    }
}
