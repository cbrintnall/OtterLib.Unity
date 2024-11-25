using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

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

public class DebugCommand
{
    public string Command;
    public Action<string[]> Callback;
}

[Singleton]
public class InGameDebugger : MonoBehaviour
{
    class LogData
    {
        public string Trace;
        public string Line;
        public LogType Type;
        public DateTime Time = DateTime.Now;

        public override string ToString()
        {
            return $"[{Time.ToLocalTime():hh:mm:ss}]: {Line}";
        }
    }

    public HashSet<DrawVar> DrawVars = new();
    public HashSet<DrawVar> TempDraw = new();
    public HashSet<DebugCommand> TextCommands = new();
    public bool active => debugLevel > 0;

    private Dictionary<KeyCode, DebugFunction> funcs = new();
    private Queue<KeyCode> dbgKeys;
    private KeyCode toggle = KeyCode.F12;
    private List<LogData> logStore = new();
    private GUIStyle styling = new();
    private Texture2D backgroundTexture;

    int debugLevel = 0;
    string text;

    public static void Draw(string text)
    {
        SingletonLoader
            .Get<InGameDebugger>()
            .TempDraw.Add(new DrawVar() { Get = () => "", Name = text });
    }

    void Awake()
    {
        Application.logMessageReceived += OnLogReceived;

        ColorUtility.TryParseHtmlString("#191a19", out Color clr);
        backgroundTexture = TextureUtilities.Create(1, 1, clr);

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

        AddCommand(new DebugFunction() { Name = "Pause Editor", Callback = () => Debug.Break() });

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
        DrawVars.Add(new DrawVar() { Name = "Command", Get = () => text });
        DrawVars.Add(new DrawVar() { Name = "UI Element", Get = GetUIElement });
        DrawVars.Add(
            new DrawVar()
            {
                Name = "Cursor",
                Get = () => $"visible={Cursor.visible},lock={Cursor.lockState}"
            }
        );
    }

    string GetUIElement()
    {
        var eventSystem = EventSystem.current;
        List<RaycastResult> raycastResults = new List<RaycastResult>();

        if (eventSystem == null)
        {
            return "No event system..";
        }

        // Create or reuse a PointerEventData object
        var pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition // Current mouse position
        };

        // Perform a raycast using the EventSystem
        raycastResults.Clear();
        eventSystem.RaycastAll(pointerEventData, raycastResults);

        // Debug output for hovered UI elements
        if (raycastResults.Count > 0)
        {
            return string.Join(',', raycastResults.Select(res => res.gameObject.name));
        }

        return "None";
    }

    void OnLogReceived(string condition, string stackTrace, LogType type)
    {
        logStore.Add(
            new LogData()
            {
                Line = condition,
                Trace = stackTrace,
                Type = type
            }
        );

        if (logStore.Count > 20)
        {
            logStore.RemoveAt(0);
        }
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
            debugLevel = (debugLevel + 1) % 3;
            text = "";
        }
        if (debugLevel == 0)
            return;
        foreach (var kc in funcs.Keys)
        {
            if (Input.GetKeyDown(kc))
            {
                funcs[kc].Callback();
            }
        }

        if (debugLevel > 1)
        {
            foreach (var c in Input.inputString)
            {
                if (c == '\b')
                {
                    text = text.Substring(0, text.Length - 1);
                }
                else if ((c == '\n') || (c == '\r'))
                {
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        text = "";
                        return;
                    }

                    var spl = text.Split(" ");
                    foreach (var cmd in TextCommands)
                    {
                        try
                        {
                            if (cmd.Command == spl[0])
                            {
                                if (spl.Length > 1)
                                {
                                    cmd.Callback(spl.Skip(1).ToArray());
                                }
                                else
                                {
                                    cmd.Callback(new string[] { });
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"Exception during command {e}");
                        }
                    }
                    text = "";
                }
                else
                {
                    text += Input.inputString;
                }
            }
        }
    }

    void LateUpdate()
    {
        TempDraw = new();
    }

    void OnDrawLogsGUI(float offsetX)
    {
        var errorTarget = logStore.LastOrDefault(line => line.Type == LogType.Exception);
        float logHeight = logStore.Sum(line => styling.CalcSize(new GUIContent(line.Line)).y);
        float width = logStore.Max(line => styling.CalcSize(new GUIContent(line.ToString())).x);

        if (errorTarget != null)
        {
            logHeight += styling.CalcSize(new GUIContent(errorTarget.Trace)).y;
            width = Mathf.Max(width, styling.CalcSize(new GUIContent(errorTarget.Trace)).x);
        }

        var boxStyle = new GUIStyle();
        boxStyle.normal.background = backgroundTexture;
        GUILayout.BeginArea(new Rect(offsetX + 20.0f, 0, width, logHeight + 20f), boxStyle);

        GUILayout.BeginVertical();
        foreach (var log in logStore)
        {
            var style = new GUIStyle();
            Color textColor = Color.white;

            switch (log.Type)
            {
                case LogType.Error:
                    textColor = Color.red;
                    break;
                case LogType.Assert:
                    textColor = Color.cyan;
                    break;
                case LogType.Warning:
                    textColor = Color.yellow;
                    break;
                case LogType.Exception:
                    textColor = Color.red;
                    break;
            }
            style.normal.textColor = textColor;
            GUILayout.Label(log.ToString(), style);
        }
        GUILayout.EndVertical();

        if (errorTarget != null)
        {
            GUILayout.BeginVertical();
            var errorStyle = new GUIStyle();
            errorStyle.normal.textColor = Color.red;
            GUILayout.Label(errorTarget.Trace, errorStyle);
            GUILayout.EndVertical();
        }

        GUILayout.EndArea();
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
            $"Debug ({toggle}/level={debugLevel}):"
        );
        int height = 0;
        GUI.BeginGroup(new Rect(0, 0, boxWidth, fontHeight * DrawVars.Count));
        var drawGroup = new List<DrawVar>();
        drawGroup.AddRange(DrawVars);
        drawGroup.AddRange(TempDraw);
        foreach (var dv in drawGroup)
        {
            try
            {
                if (dv.QueueRemoval)
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

        OnDrawLogsGUI(boxWidth);
    }
}
