using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public delegate object ValueGetter();

public class Track : Attribute
{
    public string Title;

    public Track(string title) => Title = title;

    public static void Watch(object obj)
    {
        var props = GetAllTrackables(obj);

        foreach (var prop in props)
        {
            SingletonLoader
                .Get<InGameDebugger>()
                .DrawVars.Add(
                    new DrawVar()
                    {
                        Name = prop.Item2.Title,
                        Owner = obj,
                        Get = prop.Item1,
                    }
                );
        }
    }

    private static IEnumerable<Tuple<ValueGetter, Track>> GetAllTrackables(object obj)
    {
        var fields = obj.GetType()
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => field.GetCustomAttribute<Track>() != null)
            .Select(
                field =>
                    Tuple.Create<ValueGetter, Track>(
                        () => field.GetValue(obj),
                        field.GetCustomAttribute<Track>()
                    )
            );

        var props = obj.GetType()
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => field.GetCustomAttribute<Track>() != null)
            .Select(
                field =>
                    Tuple.Create<ValueGetter, Track>(
                        () => field.GetValue(obj),
                        field.GetCustomAttribute<Track>()
                    )
            );

        var combined = new List<Tuple<ValueGetter, Track>>();

        combined.AddRange(fields);
        combined.AddRange(props);

        return combined;
    }
}

public class DrawVar
{
    public bool QueueRemoval;
    public string Name;
    public ValueGetter Get;
    public object Owner;

    public override int GetHashCode()
    {
        return Name.GetHashCode();
    }

    public override bool Equals(object obj)
    {
        return obj is DrawVar dv && dv.Name == Name && dv.Owner == Owner;
    }
}

public class DebugFunction
{
    public string Name;
    public Action Callback;
}

[Singleton]
public class InGameDebugger : MonoBehaviour
{
    public HashSet<DrawVar> DrawVars = new();

    [SerializeField]
    GUIStyle varStyle;

    private Dictionary<KeyCode, DebugFunction> funcs = new();
    private Queue<KeyCode> dbgKeys;
    private KeyCode toggle = KeyCode.F12;

    bool active;

    void Awake()
    {
        dbgKeys = new();

        dbgKeys.Enqueue(KeyCode.F1);
        dbgKeys.Enqueue(KeyCode.F2);
        dbgKeys.Enqueue(KeyCode.F3);
        dbgKeys.Enqueue(KeyCode.F4);
        dbgKeys.Enqueue(KeyCode.F5);
        dbgKeys.Enqueue(KeyCode.F6);
        dbgKeys.Enqueue(KeyCode.F7);
        dbgKeys.Enqueue(KeyCode.F8);
        dbgKeys.Enqueue(KeyCode.F9);
        dbgKeys.Enqueue(KeyCode.F10);

        AddCommand(
            new DebugFunction() { Name = "Timescale = 0", Callback = () => Time.timeScale = 0.0f }
        );

        AddCommand(
            new DebugFunction() { Name = "Timescale = 0.1", Callback = () => Time.timeScale = 0.1f }
        );

        AddCommand(
            new DebugFunction() { Name = "Timescale = 1", Callback = () => Time.timeScale = 1.0f }
        );

        DrawVars.Add(new DrawVar() { Name = "FPS", Get = () => 1.0f / Time.deltaTime });
    }

    public void AddCommand(DebugFunction f)
    {
        if (dbgKeys.TryDequeue(out KeyCode key))
        {
            funcs[key] = f;
        }
    }

    public void AddDrawVar(DrawVar dv)
    {
        DrawVars.Add(dv);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggle))
        {
            active = !active;
        }
        foreach (var kc in funcs.Keys)
        {
            if (Input.GetKeyDown(kc))
            {
                funcs[kc].Callback();
            }
        }
    }

    void OnGUI()
    {
        if (!active)
            return;
        var remove = new List<DrawVar>();
        float fontHeight = 20.0f;
        float boxWidth = 500.0f;
        GUI.Box(
            new Rect(0, 0, boxWidth + 20.0f, fontHeight * (DrawVars.Count + funcs.Count + 1)),
            $"Debug ({toggle}):"
        );
        int height = 0;
        GUI.BeginGroup(new Rect(0, 0, boxWidth, fontHeight * DrawVars.Count), varStyle);
        foreach (var dv in DrawVars)
        {
            try
            {
                if (dv.QueueRemoval || dv.Owner == null)
                {
                    remove.Add(dv);
                    continue;
                }
                GUI.Label(
                    new Rect(0, fontHeight * height++, boxWidth, fontHeight),
                    $"{dv.Name}: {dv.Get()}"
                );
            }
            catch (Exception)
            {
                remove.Add(dv);
                UnityEngine.Debug.LogWarning($"Remove drawvar {dv.Name}");
            }
        }
        GUI.EndGroup();

        foreach (var df in funcs)
        {
            GUI.Label(
                new Rect(0, fontHeight * height++, boxWidth, fontHeight),
                $"[{df.Key}] {df.Value.Name}"
            );
        }

        foreach (var dv in remove)
        {
            DrawVars.Remove(dv);
        }
    }
}
