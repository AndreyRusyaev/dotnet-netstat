using System.Net;

internal static class Formatting
{
    public static string FormatModuleName(string? moduleName)
    {
        if (string.IsNullOrEmpty(moduleName))
        {
            return "<Unknown>";
        }

        int maxLength = 24;

        if (moduleName.Length <= maxLength)
        {
            return moduleName;
        }

        return moduleName.Substring(0, maxLength - 3) + "...";
    }

    public static string FormatIpAddress(IPAddress ipAddress)
    {
        if (ipAddress.Equals(IPAddress.Any))
        {
            return "0.0.0.0";
        }
        else if (ipAddress.Equals(IPAddress.IPv6Any))
        {
            return "[::]";
        }

        return ipAddress.ToString();
    }

    public static string FormatActiveTime(TimeSpan activeTime)
    {
        if (activeTime.TotalSeconds < 60)
        {
            return $"{(int)activeTime.TotalSeconds}s";
        }
        else if (activeTime.TotalMinutes < 60)
        {
            return $"{(int)activeTime.TotalMinutes}m";
        }
        else if (activeTime.TotalHours < 24)
        {
            return $"{(int)activeTime.TotalHours}h";
        }
        else
        {
            return $"{(int)activeTime.TotalDays}d";
        }
    }
}