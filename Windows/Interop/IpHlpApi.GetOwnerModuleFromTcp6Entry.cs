using System.ComponentModel;
using System.Runtime.InteropServices;

internal static partial class IpHlpApi
{
    [DllImport("iphlpapi.dll")]
    public static extern int GetOwnerModuleFromTcp6Entry(
        ref MIB_TCP6ROW_OWNER_MODULE pTcpEntry,
        TCPIP_OWNER_MODULE_INFO_CLASS Class,
        IntPtr pBuffer,
        ref int pdwSize);

    public static TCPIP_OWNER_MODULE_BASIC_INFO? GetOwnerModuleBasicInfoForTcp6Entry(ref MIB_TCP6ROW_OWNER_MODULE tcp6TableEntry)
    {
        int moduleInfoSize = 0;

        int result = GetOwnerModuleFromTcp6Entry(
            ref tcp6TableEntry,
            TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
            IntPtr.Zero,
            ref moduleInfoSize);

        if (result != WinErrorCodes.ERROR_INSUFFICIENT_BUFFER)
        {
            // Failed to retrieve module info (Access denied, etc.)
            return null;
        }

        IntPtr moduleInfoPtr = Marshal.AllocHGlobal(moduleInfoSize);
        try
        {
            result = GetOwnerModuleFromTcp6Entry(
                ref tcp6TableEntry,
                TCPIP_OWNER_MODULE_INFO_CLASS.TCPIP_OWNER_MODULE_INFO_BASIC,
                moduleInfoPtr,
                ref moduleInfoSize);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            return Marshal.PtrToStructure<TCPIP_OWNER_MODULE_BASIC_INFO>(moduleInfoPtr);
        }
        finally
        {
            Marshal.FreeHGlobal(moduleInfoPtr);
        }
    }    
}