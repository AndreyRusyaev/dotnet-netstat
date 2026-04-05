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

    public static string FormatActiveTime(TimeSpan? activeTime)
    {
        if (activeTime == null)
        {
            return "";
        }

        var activeTimeValue = activeTime.Value;

        if (activeTimeValue.TotalSeconds < 60)
        {
            return $"{(int)activeTimeValue.TotalSeconds}s";
        }
        else if (activeTimeValue.TotalMinutes < 60)
        {
            return $"{(int)activeTimeValue.TotalMinutes}m";
        }
        else if (activeTimeValue.TotalHours < 24)
        {
            return $"{(int)activeTimeValue.TotalHours}h";
        }
        else
        {
            return $"{(int)activeTimeValue.TotalDays}d";
        }
    }
}