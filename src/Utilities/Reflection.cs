using System.Reflection;

public static class ReflectionUtilities
{
    public static object CreateTypeFromAssembly(string type) =>
        Assembly.GetExecutingAssembly().CreateInstance(type);
}
