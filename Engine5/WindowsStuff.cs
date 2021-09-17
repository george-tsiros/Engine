namespace Engine;

using System;
using System.Runtime.InteropServices;

static class WindowsStuff {
    [Flags]
    internal enum Ver {
        MINORVERSION = 0x0000001,
        MAJORVERSION = 0x0000002,
        BUILDNUMBER = 0x0000004,
        PLATFORMID = 0x0000008,
        SERVICEPACKMINOR = 0x0000010,
        SERVICEPACKMAJOR = 0x0000020,
        SUITENAME = 0x0000040,
        PRODUCT_TYPE = 0x0000080,
    }
    internal enum Op {
        Equal = 1,
        Greater,
        Greater_equal,
        Less,
        Less_equal,
        And,
        Or,
    }

    internal unsafe struct OsVersionInfoExW {
        public int dwOSVersionInfoSize;
        public int dwMajorVersion;
        public int dwMinorVersion;
        public int dwBuildNumber;
        public int dwPlatformId;
        public fixed ushort szCSDVersion[128];
        public short wServicePackMajor;
        public short wServicePackMinor;
        public short wSuiteMask;
        public byte wProductType;
        public byte wReserved;
    }
    internal unsafe static void Foob () {

    }
    [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
    internal unsafe static extern ulong VerSetConditionMask (ulong l, int i, byte condition);

    [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
    internal unsafe static extern int RtlVerifyVersionInfo (OsVersionInfoExW* p, uint i, ulong l);

    internal static bool IsWindows10BuildOrGreaterWin32 (ushort build) {
        var versionInfo = new OsVersionInfoExW() { dwOSVersionInfoSize = Marshal.SizeOf<OsVersionInfoExW>(), dwMajorVersion = 10, dwBuildNumber = build };
        var mask = Ver.MAJORVERSION | Ver.MINORVERSION | Ver.BUILDNUMBER;
        var condition = VerSetConditionMask(0, (int)Ver.MAJORVERSION, (byte)Op.Greater_equal);
        condition = VerSetConditionMask(condition, (int)Ver.MINORVERSION, (byte)Op.Greater_equal);
        condition = VerSetConditionMask(condition, (int)Ver.BUILDNUMBER, (byte)Op.Greater_equal);

        throw new NotImplementedException();
    }

}
