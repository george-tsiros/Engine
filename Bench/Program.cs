namespace Bench;
using Engine;
using System;
using System.Diagnostics;
class Program {
    static void Main () {
        using var f = FancyStream.WriteTo("test.bin");
        Bench(f);
        _ = Console.ReadLine();
    }

    private static void Bench (FancyStream f) {
        const int mil = 1000000;
        var ints = new int[mil];
        var r = new Random();
        for (var i = 0; i < mil; ++i)
            ints[i] = r.Next();

        var t = Stopwatch.StartNew();
        for (var i = 0; i < mil; ++i)
            f.Write(ints[i]);
        t.Stop();
        Console.WriteLine(Format(t.ElapsedTicks, mil, "ints: "));

        var strings = new string[mil];
        for (var i = 0; i < mil; ++i)
            strings[i] = RandomString(r);

        t.Restart();
        for (var i = 0; i < mil; ++i)
            f.Write(strings[i]);
        t.Stop();
        Console.WriteLine(Format(t.ElapsedTicks, mil, "strings: "));

    }
    private static string RandomString (Random r) {
        var l = r.Next(5, 20);
        Span<byte> bytes = stackalloc byte[l];
        for (var i = 0; i < l; ++i)
            bytes[i] = (byte)r.Next(' ', '~');
        return System.Text.Encoding.ASCII.GetString(bytes);
    }

    private static string ToEng (double d) {
        var exponent = Math.Log10(Math.Abs(d));
        if (Math.Abs(d) >= 1) {
            switch ((int)Math.Floor(exponent)) {
                case 0:
                case 1:
                case 2:
                    return d.ToString();
                case 3:
                case 4:
                case 5:
                    return (d / 1e3).ToString() + " k";
                case 6:
                case 7:
                case 8:
                    return (d / 1e6).ToString() + " M";
                case 9:
                case 10:
                case 11:
                    return (d / 1e9).ToString() + " G";
                case 12:
                case 13:
                case 14:
                    return (d / 1e12).ToString() + " T";
                case 15:
                case 16:
                case 17:
                    return (d / 1e15).ToString() + " P";
                case 18:
                case 19:
                case 20:
                    return (d / 1e18).ToString() + " E";
                case 21:
                case 22:
                case 23:
                    return (d / 1e21).ToString() + " Z";
                default:
                    return (d / 1e24).ToString() + " Y";
            }
        }
        else if (Math.Abs(d) > 0) {
            switch ((int)Math.Floor(exponent)) {
                case -1:
                case -2:
                case -3:
                    return (d * 1e3).ToString() + " m";
                case -4:
                case -5:
                case -6:
                    return (d * 1e6).ToString() + " u";
                case -7:
                case -8:
                case -9:
                    return (d * 1e9).ToString() + " n";
                case -10:
                case -11:
                case -12:
                    return (d * 1e12).ToString() + " p";
                case -13:
                case -14:
                case -15:
                    return (d * 1e15).ToString() + " f";
                case -16:
                case -17:
                case -18:
                    return (d * 1e15).ToString() + " a";
                case -19:
                case -20:
                case -21:
                    return (d * 1e15).ToString() + " z";
                default:
                    return (d * 1e15).ToString() + " y";
            }
        }
        else {
            return "0";
        }
    }
    private static string Format (long ticks, long count, string info = null) => $"{info}{ticks} ticks, {(double)ticks / count} ticks/item, {ToEng((double)ticks / Stopwatch.Frequency)}s, {ToEng((double)ticks / count / Stopwatch.Frequency)}s/item ";
}