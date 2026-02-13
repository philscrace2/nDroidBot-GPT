using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.IO;

namespace nTestar.UiaProbe;

internal static class Program
{
    private const int MaxTopLevel = 100;
    private const int MaxDepth = 3;

    [STAThread]
    private static void Main()
    {
        if (!OperatingSystem.IsWindows())
        {
            Console.WriteLine("This probe must run on Windows.");
            return;
        }

        object? root = GetRootAutomationElement();
        if (root == null)
        {
            Console.WriteLine("Could not resolve UIAutomation root element (UIAutomationClient).\n" +
                              "Check that UIAutomation is available on this machine.");
            return;
        }

        List<object> topLevelElements = EnumerateChildren(root).Take(MaxTopLevel).ToList();
        if (topLevelElements.Count == 0)
        {
            Console.WriteLine("Root child enumeration returned 0; trying Win32 fallback.");
            topLevelElements = EnumerateTopLevelWindowsViaHandles().Take(MaxTopLevel).ToList();
        }

        var snapshot = new ProbeSnapshot
        {
            CapturedAtUtc = DateTime.UtcNow,
            MachineName = Environment.MachineName,
            TopLevelCount = topLevelElements.Count,
            Windows = topLevelElements.Select(element => BuildNode(element, 0)).Where(node => node != null).Cast<UiaNode>().ToList()
        };

        var options = new JsonSerializerOptions { WriteIndented = true };
        string json = JsonSerializer.Serialize(snapshot, options);

        Console.WriteLine(json);

        string outputPath = Path.Combine(AppContext.BaseDirectory, "uia-probe-output.json");
        File.WriteAllText(outputPath, json);
        Console.WriteLine($"Wrote: {outputPath}");
        Console.Read();
    }

    private static UiaNode? BuildNode(object automationElement, int depth)
    {
        object? current = GetPropertyValue(automationElement, "Current");
        if (current == null)
        {
            return null;
        }

        var node = new UiaNode
        {
            Name = GetPropertyValue(current, "Name")?.ToString() ?? string.Empty,
            ClassName = GetPropertyValue(current, "ClassName")?.ToString() ?? string.Empty,
            AutomationId = GetPropertyValue(current, "AutomationId")?.ToString() ?? string.Empty,
            FrameworkId = GetPropertyValue(current, "FrameworkId")?.ToString() ?? string.Empty,
            ProcessId = ConvertToInt(GetPropertyValue(current, "ProcessId")),
            NativeWindowHandle = ConvertToInt(GetPropertyValue(current, "NativeWindowHandle")),
            IsOffscreen = ConvertToBool(GetPropertyValue(current, "IsOffscreen")),
            IsEnabled = ConvertToBool(GetPropertyValue(current, "IsEnabled")),
            ControlType = ReadControlTypeName(GetPropertyValue(current, "ControlType")),
            BoundingRectangle = ReadRectangle(GetPropertyValue(current, "BoundingRectangle"))
        };

        if (depth >= MaxDepth)
        {
            return node;
        }

        foreach (object child in EnumerateChildren(automationElement))
        {
            UiaNode? childNode = BuildNode(child, depth + 1);
            if (childNode != null)
            {
                node.Children.Add(childNode);
            }
        }

        return node;
    }

    private static IEnumerable<object> EnumerateChildren(object automationElement)
    {
        MethodInfo? findAll = automationElement.GetType().GetMethod("FindAll", BindingFlags.Public | BindingFlags.Instance);
        if (findAll != null)
        {
            object?[]? args = BuildFindAllArgs(findAll);
            if (args != null)
            {
                object? collection = SafeInvoke(findAll, automationElement, args);
                foreach (object child in EnumerateCollection(collection))
                {
                    yield return child;
                }

                yield break;
            }
        }

        foreach (object child in EnumerateChildrenWithTreeWalker(automationElement))
        {
            yield return child;
        }
    }

    private static object?[]? BuildFindAllArgs(MethodInfo findAll)
    {
        ParameterInfo[] parameters = findAll.GetParameters();
        if (parameters.Length != 2)
        {
            return null;
        }

        Type treeScopeType = parameters[0].ParameterType;
        Type conditionType = parameters[1].ParameterType;

        object? trueCondition = conditionType.GetProperty("TrueCondition", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
        if (trueCondition == null)
        {
            return null;
        }

        object treeScopeChildren = Enum.Parse(treeScopeType, "Children");
        return new[] { treeScopeChildren, trueCondition };
    }

    private static IEnumerable<object> EnumerateCollection(object? collection)
    {
        if (collection == null)
        {
            yield break;
        }

        Type collectionType = collection.GetType();
        PropertyInfo? countProperty = collectionType.GetProperty("Count");
        MethodInfo? getItemMethod = collectionType.GetMethod("get_Item");
        if (countProperty == null || getItemMethod == null)
        {
            yield break;
        }

        int count = ConvertToInt(countProperty.GetValue(collection));
        for (int i = 0; i < count; i++)
        {
            object? child = SafeInvoke(getItemMethod, collection, new object[] { i });
            if (child != null)
            {
                yield return child;
            }
        }
    }

    private static IEnumerable<object> EnumerateChildrenWithTreeWalker(object automationElement)
    {
        Type? treeWalkerType = ResolveUiaType("System.Windows.Automation.TreeWalker");
        if (treeWalkerType == null)
        {
            yield break;
        }

        object? walker = treeWalkerType.GetProperty("ControlViewWalker", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
        MethodInfo? getFirstChild = treeWalkerType.GetMethod("GetFirstChild", BindingFlags.Public | BindingFlags.Instance);
        MethodInfo? getNextSibling = treeWalkerType.GetMethod("GetNextSibling", BindingFlags.Public | BindingFlags.Instance);
        if (walker == null || getFirstChild == null || getNextSibling == null)
        {
            yield break;
        }

        object? child = SafeInvoke(getFirstChild, walker, new[] { automationElement });
        while (child != null)
        {
            yield return child;
            child = SafeInvoke(getNextSibling, walker, new[] { child });
        }
    }

    private static object? GetRootAutomationElement()
    {
        Type? automationElementType = ResolveUiaType("System.Windows.Automation.AutomationElement");
        return automationElementType?.GetProperty("RootElement", BindingFlags.Public | BindingFlags.Static)?.GetValue(null);
    }

    private static Type? ResolveUiaType(string fullName)
    {
        Type? direct = Type.GetType($"{fullName}, UIAutomationClient");
        if (direct != null)
        {
            return direct;
        }

        Assembly? loaded = AppDomain.CurrentDomain
            .GetAssemblies()
            .FirstOrDefault(a => string.Equals(a.GetName().Name, "UIAutomationClient", StringComparison.OrdinalIgnoreCase));
        if (loaded != null)
        {
            return loaded.GetType(fullName);
        }

        try
        {
            Assembly assembly = Assembly.Load("UIAutomationClient");
            return assembly.GetType(fullName);
        }
        catch
        {
            // Try Desktop framework assemblies when UIAutomationClient is not auto-resolved.
            try
            {
                Assembly presentationCore = Assembly.Load("PresentationCore");
                Type? fromPresentation = presentationCore.GetType(fullName);
                if (fromPresentation != null)
                {
                    return fromPresentation;
                }
            }
            catch
            {
                // Ignore and fall through.
            }

            return null;
        }
    }

    private static object? GetPropertyValue(object source, string propertyName)
    {
        try
        {
            return source.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)?.GetValue(source);
        }
        catch
        {
            return null;
        }
    }

    private static object? SafeInvoke(MethodInfo method, object? target, object?[]? args)
    {
        try
        {
            return method.Invoke(target, args);
        }
        catch
        {
            return null;
        }
    }

    private static int ConvertToInt(object? value)
    {
        if (value == null)
        {
            return 0;
        }

        try
        {
            return Convert.ToInt32(value);
        }
        catch
        {
            return 0;
        }
    }

    private static bool ConvertToBool(object? value)
    {
        if (value == null)
        {
            return false;
        }

        try
        {
            return Convert.ToBoolean(value);
        }
        catch
        {
            return false;
        }
    }

    private static string ReadControlTypeName(object? controlType)
    {
        if (controlType == null)
        {
            return string.Empty;
        }

        object? programmaticName = GetPropertyValue(controlType, "ProgrammaticName");
        return programmaticName?.ToString() ?? string.Empty;
    }

    private static RectSnapshot? ReadRectangle(object? rect)
    {
        if (rect == null)
        {
            return null;
        }

        return new RectSnapshot
        {
            X = ConvertToDouble(GetPropertyValue(rect, "X")),
            Y = ConvertToDouble(GetPropertyValue(rect, "Y")),
            Width = ConvertToDouble(GetPropertyValue(rect, "Width")),
            Height = ConvertToDouble(GetPropertyValue(rect, "Height"))
        };
    }

    private static double ConvertToDouble(object? value)
    {
        if (value == null)
        {
            return 0.0;
        }

        try
        {
            return Convert.ToDouble(value);
        }
        catch
        {
            return 0.0;
        }
    }

    private static IEnumerable<object> EnumerateTopLevelWindowsViaHandles()
    {
        List<object> result = new();
        Type? automationElementType = ResolveUiaType("System.Windows.Automation.AutomationElement");
        MethodInfo? fromHandle = automationElementType?.GetMethod("FromHandle", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(IntPtr) }, null);
        if (fromHandle == null)
        {
            return result;
        }

        EnumWindows((hWnd, lParam) =>
        {
            if (hWnd == IntPtr.Zero || !IsWindowVisible(hWnd))
            {
                return true;
            }

            object? element = SafeInvoke(fromHandle, null, new object[] { hWnd });
            if (element != null)
            {
                result.Add(element);
            }

            return true;
        }, IntPtr.Zero);

        return result;
    }

    [DllImport("user32.dll")]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
}

internal sealed class ProbeSnapshot
{
    public DateTime CapturedAtUtc { get; set; }
    public string MachineName { get; set; } = string.Empty;
    public int TopLevelCount { get; set; }
    public List<UiaNode> Windows { get; set; } = new();
}

internal sealed class UiaNode
{
    public string Name { get; set; } = string.Empty;
    public string ControlType { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string AutomationId { get; set; } = string.Empty;
    public string FrameworkId { get; set; } = string.Empty;
    public int ProcessId { get; set; }
    public int NativeWindowHandle { get; set; }
    public bool IsOffscreen { get; set; }
    public bool IsEnabled { get; set; }
    public RectSnapshot? BoundingRectangle { get; set; }
    public List<UiaNode> Children { get; set; } = new();
}

internal sealed class RectSnapshot
{
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
}
