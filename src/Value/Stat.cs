using System;
using UnityEngine;

[Serializable]
public class Stat
{
    public int Base;
    public int Max;
    public int Min;
    public int Current
    {
        get => current_;
        set { current_ = Mathf.Clamp(value, Min, Max); }
    }

    int current_;

    public static implicit operator Stat(int i) => new Stat(int.MinValue, int.MaxValue, i);

    public static Stat operator -(Stat s, int i) => new Stat(s.Min, s.Max, s.current_ - i);

    public static Stat operator +(Stat s, int i) => new Stat(s.Min, s.Max, s.current_ + i);

    Stat()
    {
        Current = Base;
    }

    public override string ToString()
    {
        return Current.ToString();
    }

    public Stat(int min, int max, int start)
    {
        Min = min;
        Max = max;
        Base = start;
        Current = start;
    }
}
