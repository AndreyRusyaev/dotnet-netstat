using System.Runtime.InteropServices;

internal static partial class IpHlpApi
{
    public enum UDP_TABLE_CLASS 
    {
        UDP_TABLE_BASIC,
        UDP_TABLE_OWNER_PID,
        UDP_TABLE_OWNER_MODULE
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDPTABLE_OWNER_MODULE
    {
        public MIB_UDPTABLE_OWNER_MODULE()
        {            
        }

        public int dwNumEntries;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public MIB_UDPROW_OWNER_MODULE[] table = new MIB_UDPROW_OWNER_MODULE[1];

        public IntPtr[] GetTableEntries(IntPtr tablePointer)
        {
            var tableEntries = new IntPtr[dwNumEntries];
            IntPtr itemPtr = tablePointer + Marshal.OffsetOf<MIB_UDPTABLE_OWNER_MODULE>(nameof(table));
            for (int ii = 0; ii < dwNumEntries; ii += 1)
            {
                tableEntries[ii] = itemPtr;
                itemPtr += Marshal.SizeOf<MIB_UDPROW_OWNER_MODULE>();
            }
            return tableEntries;
        }

        public MIB_UDPROW_OWNER_MODULE GetTableEntry(IntPtr itemPtr)
        {
            return Marshal.PtrToStructure<MIB_UDPROW_OWNER_MODULE>(itemPtr);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDPROW_OWNER_MODULE 
    {
        public MIB_UDPROW_OWNER_MODULE()
        {            
        }

        public int dwLocalAddr;
        public int dwLocalPort;
        public int dwOwningPid;
        public long liCreateTimestamp;
        public int dwFlags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public long[] OwningModuleInfo = new long[16];        
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDP6TABLE_OWNER_MODULE
    {
        public MIB_UDP6TABLE_OWNER_MODULE()
        {            
        }

        public int dwNumEntries;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public MIB_UDP6ROW_OWNER_MODULE[] table = new MIB_UDP6ROW_OWNER_MODULE[1];

        public IntPtr[] GetTableEntries(IntPtr tablePointer)
        {
            var tableEntries = new IntPtr[dwNumEntries];
            IntPtr itemPtr = tablePointer + Marshal.OffsetOf<MIB_UDP6TABLE_OWNER_MODULE>(nameof(table));
            for (int ii = 0; ii < dwNumEntries; ii += 1)
            {
                tableEntries[ii] = itemPtr;
                itemPtr += Marshal.SizeOf<MIB_UDP6ROW_OWNER_MODULE>();
            }
            return tableEntries;
        }

        public MIB_UDP6ROW_OWNER_MODULE GetTableEntry(IntPtr itemPtr)
        {
            return Marshal.PtrToStructure<MIB_UDP6ROW_OWNER_MODULE>(itemPtr);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MIB_UDP6ROW_OWNER_MODULE
    {
        public MIB_UDP6ROW_OWNER_MODULE()
        {            
        }

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] ucLocalAddr = new byte[16];

        public int dwLocalScopeId;

        public int dwLocalPort;

        public int dwOwningPid;

        public long liCreateTimestamp;

        public int dwFlags;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public long[] OwningModuleInfo = new long[16];
    }

    [DllImport("iphlpapi.dll")]
    public static extern int GetExtendedUdpTable(IntPtr pUdpTable, ref int pdwSize, bool bOrder, int ulAf, UDP_TABLE_CLASS TableClass, int Reserved);
}