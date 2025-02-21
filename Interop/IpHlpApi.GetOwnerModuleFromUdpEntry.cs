using System.Runtime.InteropServices;

internal static partial class IpHlpApi
{
    [DllImport("iphlpapi.dll")]
    public static extern int GetOwnerModuleFromUdpEntry(IntPtr pUdpTable, TCPIP_OWNER_MODULE_INFO_CLASS Class, IntPtr pBuffer, ref int pdwSize);

    [DllImport("iphlpapi.dll")]
    public static extern int GetOwnerModuleFromUdp6Entry(IntPtr pUdpTable, TCPIP_OWNER_MODULE_INFO_CLASS Class, IntPtr pBuffer, ref int pdwSize);
}