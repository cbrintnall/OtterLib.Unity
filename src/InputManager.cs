using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class PlayerAction<T>
    where T : unmanaged
{
    public virtual string Representation => "";
    public event Action<T> Changed;

    [Required]
    public KeyCode Default;

    [HideInInspector]
    public KeyCode Override;
    public T Value;
    public bool Started;
    public bool Stopped;

    public static implicit operator T(PlayerAction<T> pa) => pa.Value;

    public virtual void UpdateValue(T value)
    {
        Value = value;
    }

    public void Reset() => Value = default;
}

public class PlayerBinaryAction : PlayerAction<bool>
{
    public float HoldTime
    {
        get => ts;
    }

    TimeSince ts;

    public override void UpdateValue(bool value)
    {
        Stopped = Value && !value;
        Started = !Value && value;

        if (Started)
            ts = 0;

        if (!Value)
            ts = 0;

        base.UpdateValue(value);
    }
}

[Singleton]
public class InputManager
{
    [MenuItem("otter/Input/Bool")]
    public static void CreateBoolAction() { }
}
