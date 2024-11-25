using System.Diagnostics;

public static class OtterDebug
{
    public static bool TryAssert(bool condition, string text = "Assertion failed")
    {
        Debug.Assert(condition, text);
        return condition;
    }
}
