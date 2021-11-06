namespace Engine;

using System;
using System.Runtime.InteropServices;

static class WindowsStuff {
    [Flags]
    internal enum Ver {
        MinorVersion = 0x0000001,
        MajorVersion = 0x0000002,
        BuildNumber = 0x0000004,
        PlatformId = 0x0000008,
        ServicePackMinor = 0x0000010,
        ServicePackMajor = 0x0000020,
        SuiteName = 0x0000040,
        ProductType = 0x0000080,
    }
    internal enum Op {
        Equal = 1,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
        And,
        Or,
    }

#pragma warning disable CS0649
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
#pragma warning restore CS0649

    internal unsafe static void Foob () {

    }

    [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
    internal unsafe static extern ulong VerSetConditionMask (ulong l, int i, byte condition);

    [DllImport("ntdll.dll", CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
    internal unsafe static extern int RtlVerifyVersionInfo (OsVersionInfoExW* p, uint i, ulong l);

    internal static bool IsWindows10BuildOrGreaterWin32 (ushort build) {
        var versionInfo = new OsVersionInfoExW() { dwOSVersionInfoSize = Marshal.SizeOf<OsVersionInfoExW>(), dwMajorVersion = 10, dwBuildNumber = build };
        var mask = Ver.MajorVersion | Ver.MinorVersion | Ver.BuildNumber;
        var condition = VerSetConditionMask(0, (int)Ver.MajorVersion, (byte)Op.GreaterEqual);
        condition = VerSetConditionMask(condition, (int)Ver.MinorVersion, (byte)Op.GreaterEqual);
        condition = VerSetConditionMask(condition, (int)Ver.BuildNumber, (byte)Op.GreaterEqual);

        throw new NotImplementedException();
    }
}