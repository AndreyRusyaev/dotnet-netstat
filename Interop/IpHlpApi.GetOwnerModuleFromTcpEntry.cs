using System.Runtime.InteropServices;

internal static partial class IpHlpApi
{    
    public enum TCPIP_OWNER_MODULE_INFO_CLASS
    {
        TCPIP_OWNER_MODULE_INFO_BASIC
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct TCPIP_OWNER_MODULE_BASIC_INFO
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pModuleName;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pModulePath;
    }

    [DllImport("iphlpapi.dll")]
    public static extern int GetOwnerModuleFromTcpEntry(IntPtr pTcpTable, TCPIP_OWNER_MODULE_INFO_CLASS Class, IntPtr pBuffer, ref int pdwSize);
}