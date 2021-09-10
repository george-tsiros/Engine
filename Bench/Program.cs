namespace Bench;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

class Program {
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static void PushAscii (Span<byte> bytes, ref long int64, ref int offset) {
        int64 = Math.DivRem(int64, 10, out var d);
        bytes[--offset] = (byte)(d + '0');
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static int ToChars (long int64, Span<byte> bytes) {
        var isNegative = int64 < 0l;
        if (isNegative)
            int64 = -int64;
        var offset = 20;
        do
            PushAscii(bytes, ref int64, ref offset);
        while (int64 != 0);
        if (isNegative)
            bytes[--offset] = (byte)'-';
        return offset;
    }

    public static int ToCharsInlined (long int64, Span<byte> bytes) {
        var isNegative = int64 < 0l;
        if (isNegative)
            int64 = -int64;
        var offset = 20;
        do {
            int64 = Math.DivRem(int64, 10, out var d);
            bytes[--offset] = (byte)(d + '0');
        } while (int64 != 0);
        if (isNegative)
            bytes[--offset] = (byte)'-';
        return offset;
    }

    static void Main () {
        Bench_Actual_Stream();
        _ = Console.ReadLine();
    }
    private static void Trace () => Console.WriteLine(new StackFrame(1).GetMethod().Name);
    private static void Bench_Null_Stream () {
        Trace();
        using var stream = File.Create("test.bin");
        Bench(stream);
    }
    private static void Bench_Actual_Stream () {
        Trace();
        Bench(Stream.Null);
    }

    private static void Bench_StringFormat_Actual_StreamWriter () {
        Trace();
        using var writer = new StreamWriter("test.txt");
        Bench(writer);
    }
    private static void Bench_StringFormat_Null_StreamWriter () {
        Trace();
        Bench(StreamWriter.Null);
    }

    private static void Bench (StreamWriter writer, long count = 1000000l) {
        const string format = "{0} {1}\n";
        var longs = new long[count];
        var r = new Random();
        var strings = new string[count];
        do {
            for (var i = 0; i < count; ++i) {
                longs[i] = r.NextInt64(1_000_000, 1_000_000_000);
                var randomString = RandomString(r);
                strings[i] = randomString;
            }
            var t0 = Stopwatch.GetTimestamp();
            for (var i = 0; i < count; ++i)
                writer.Write(string.Format(format, longs[i], strings[i]));
            var t1 = Stopwatch.GetTimestamp();
            Rep(t1 - t0, count, "string.Format: ");

            t0 = Stopwatch.GetTimestamp();
            for (var i = 0; i < count; ++i)
                writer.Write($"{longs[i]} {strings[i]}\n");
            t1 = Stopwatch.GetTimestamp();
            Rep(t1 - t0, count, "interp. str.: ");
            Console.WriteLine();
        } while (!Console.KeyAvailable);
    }

    private static void Bench (Stream stream, long count = 1000000l) {
        var longs = new long[count];
        var r = new Random();
        var arrays = new byte[count][];
        var strings = new string[count];
        do {
            for (var i = 0; i < count; ++i) {
                longs[i] = r.NextInt64(1_000_000, 1_000_000_000);
                var randomString = RandomString(r);
                strings[i] = randomString;
                arrays[i] = Encoding.ASCII.GetBytes(randomString);
            }
            var t0 = Stopwatch.GetTimestamp();
            for (var i = 0; i < count; ++i)
                Foo(stream, longs[i], arrays[i]);
            var t1 = Stopwatch.GetTimestamp();
            Rep(t1 - t0, count, "byte arrays, normal: ");

            t0 = Stopwatch.GetTimestamp();
            for (var i = 0; i < count; ++i)
                FooInlined(stream, longs[i], arrays[i]);
            t1 = Stopwatch.GetTimestamp();
            Rep(t1 - t0, count, "byte arrays, inlined: ");

            t0 = Stopwatch.GetTimestamp();
            for (var i = 0; i < count; ++i)
                Foo(stream, longs[i], strings[i]);
            t1 = Stopwatch.GetTimestamp();
            Rep(t1 - t0, count, "strings, normal: ");

            t0 = Stopwatch.GetTimestamp();
            for (var i = 0; i < count; ++i)
                FooInlined(stream, longs[i], strings[i]);
            t1 = Stopwatch.GetTimestamp();
            Rep(t1 - t0, count, "strings, inlined: ");
            Console.WriteLine();

        } while (!Console.KeyAvailable);
    }

    private static void Foo (Stream stream, long int64, ReadOnlySpan<byte> str) {
        Span<byte> bytes = stackalloc byte[20];
        var offset = ToChars(int64, bytes);
        stream.Write(bytes.Slice(offset));
        stream.WriteByte((byte)' ');
        stream.Write(str);
        stream.WriteByte((byte)'\n');
    }

    private static void FooInlined (Stream stream, long int64, ReadOnlySpan<byte> str) {
        Span<byte> bytes = stackalloc byte[20];
        var offset = ToCharsInlined(int64, bytes);
        stream.Write(bytes.Slice(offset));
        stream.WriteByte((byte)' ');
        stream.Write(str);
        stream.WriteByte((byte)'\n');
    }

    private static void Foo (Stream stream, long int64, string str) {
        var l = str.Length;
        Span<byte> bytes = stackalloc byte[l + 21];
        bytes[20] = (byte)' ';
        bytes[l + 20] = (byte)'\n';
        var offset = ToChars(int64, bytes);
        _ = Encoding.ASCII.GetBytes(str, bytes.Slice(20));
        stream.Write(bytes.Slice(offset));
    }

    private static void FooInlined (Stream stream, long int64, string str) {
        var l = str.Length;
        Span<byte> bytes = stackalloc byte[l + 21];
        bytes[20] = (byte)' ';
        bytes[l + 20] = (byte)'\n';
        var offset = ToCharsInlined(int64, bytes);
        _ = Encoding.ASCII.GetBytes(str, bytes.Slice(20));
        stream.Write(bytes.Slice(offset));
    }

    private static void Rep (long ticks, long count, string info = null) => Console.WriteLine(Format(ticks, count, info));

    private static string RandomString (Random r) {
        var l = r.Next(5, 20);
        Span<byte> bytes = stackalloc byte[l];
        for (var i = 0; i < l; ++i)
            bytes[i] = (byte)r.Next(' ', '~');
        return System.Text.Encoding.ASCII.GetString(bytes);
    }


    public static string ToEng (double value, string unit = null) {
        int exp = (int)(Math.Floor(Math.Log10(value) / 3.0) * 3.0);
        double newValue = value * Math.Pow(10.0, -exp);
        if (newValue >= 1000.0) {
            newValue = newValue / 1000.0;
            exp += 3;
        }

        var symbol = exp switch {
            3 => "k",
            6 => "M",
            9 => "G",
            12 => "T",
            -3 => "m",
            -6 => "u",
            -9 => "n",
            -12 => "p",
            _ => "",
        };
        return $"{newValue:##0.000} {symbol}{unit}";
    }
    private static string Format (long ticks, long count, string info = null) => $"{info}{ticks} ticks, {(double)ticks / count} ticks/item, {ToEng((double)ticks / Stopwatch.Frequency)}s, {ToEng((double)ticks / count / Stopwatch.Frequency)}s/item ";
}