using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class CompoundValueHandle<T>
{
    public virtual T Value => value;
    public int Priority = 1;
    public Action Kill
    {
        get => killed_;
        set
        {
            // Decorate kill callback to avoid repeat "murders"
            killed_ = () =>
            {
                if (!Killed)
                {
                    value();
                    Killed = true;
                }
            };
        }
    }

    protected T value;

    private bool Killed;
    private Action killed_;

    public static implicit operator CompoundValueHandle<T>(T val) =>
        new CompoundValueHandle<T>() { value = val };

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

public class DynamicCompoundValueHandle<T> : CompoundValueHandle<T>
{
    public override T Value => Callback();
    public Func<T> Callback;

    public DynamicCompoundValueHandle()
    {
        Priority = 999;
        Callback = () => default;
    }
}

public class CompoundValueInt
{
    public int Value
    {
        get { return GetValue(true); }
        private set { this.val = value; }
    }
    public bool AboveBase => Value > start;
    public Action<int> ValueChanged;

    private List<CompoundValueHandle<int>> values = new();

    public static implicit operator float(CompoundValueInt c) => c.val;

    public static implicit operator CompoundValueInt(int f) => new CompoundValueInt(f);

    public static implicit operator int(CompoundValueInt c) => c.val;

    int start = 0;
    int permanent = 0;
    int val = 0;

    public override string ToString()
    {
        return Value.ToString();
    }

    public CompoundValueInt(int start)
    {
        this.start = start;
        this.Value = start;
    }

    public int GetValue(bool recalc = true)
    {
        if (recalc)
            RecalculateValues();
        return val;
    }

    public CompoundValueHandle<int> Add(int val)
    {
        CompoundValueHandle<int> handle = val;
        handle.Kill = () =>
        {
            values.Remove(handle);
            values = values.OrderByDescending(handle => handle.Priority).ToList();
            RecalculateValues();
        };
        values.Add(handle);
        values = values.OrderByDescending(handle => handle.Priority).ToList();
        RecalculateValues();
        ValueChanged?.Invoke(val);
        return handle;
    }

    public T Get<T>()
        where T : CompoundValueHandle<int>, new()
    {
        T handle = new T();
        handle.Kill = () =>
        {
            values.Remove(handle);
            values = values.OrderByDescending(handle => handle.Priority).ToList();
            RecalculateValues();
        };
        values.Add(handle);
        values = values.OrderByDescending(handle => handle.Priority).ToList();
        RecalculateValues();
        ValueChanged?.Invoke(val);
        return handle;
    }

    public void Reset()
    {
        foreach (var handle in values)
        {
            handle.Kill();
        }

        permanent = 0;
    }

    public void AddPermanent(int value)
    {
        permanent += value;
    }

    void RecalculateValues()
    {
        val = start + permanent;
        for (int i = 0; i < values.Count; i++)
        {
            val += values[i].Value;
        }
    }
}

public class CompoundValue
{
    public float Value => RecalculateValues();
    public bool AboveBase => value > start;
    public Action<float> ValueChanged;

    private List<CompoundValueHandle<float>> values = new();

    public static implicit operator float(CompoundValue c) => c.Value;

    public static implicit operator CompoundValue(float f) => new CompoundValue(f);

    public static implicit operator int(CompoundValue c) => Mathf.RoundToInt(c.Value);

    bool dirty = false;
    float value = 0.0f;
    float start = 0.0f;

    public override string ToString()
    {
        return Mathf.RoundToInt(Value).ToString();
    }

    public CompoundValue(float start = 0.0f)
    {
        this.start = start;
        Add(start);
    }

    public CompoundValueHandle<float> Add(float val)
    {
        CompoundValueHandle<float> handle = val;
        dirty = true;
        handle.Kill = () =>
        {
            values.Remove(handle);
            dirty = true;
        };
        values.Add(handle);
        ValueChanged?.Invoke(Value);
        return handle;
    }

    float RecalculateValues()
    {
        if (dirty)
        {
            value = values.Sum(handle => handle.Value);
            dirty = false;
        }
        return values.Sum(handle => handle.Value);
    }
}
