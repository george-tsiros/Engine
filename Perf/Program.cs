namespace Perf {
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    class Program {
        private enum Kind:byte {
            Stamp,
            Enter,
            Leave,
        }
        /*
        Stamp:
        [Kind.Stamp][int64][strlen][str]
        Enter:
        [Kind.Enter][int64][strlen][str]
        Leave:
        [Kind.Leave][int64]
        */
        private class Entry { 
            public Kind Kind { get; }
            public long Ticks { get; }
            public string Message { get; } = null;
            public Entry (Kind k, long t, string m = null) => (Kind, Ticks, Message) = (k, t, m);
        }
        private static int Main (string[] args) {
            try {
                DoUnsafe(args[0]);
                return 0;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return -1;
            }
            finally {
                _ = Console.ReadLine();
            }
        }

        private static void DoUnsafe (string filepath) {
            using (var f = new StreamReader(filepath, Encoding.ASCII))
                DoUnsafe(f);
        }

        private static IEnumerable<string> Lines (TextReader reader) {
            for (; ; ) {
                if (reader.ReadLine() is string l)
                    yield return l;
                else
                    yield break;
            }
        }

        private static readonly List<string> strings = new List<string>();

        private static int TryAdd (List<string> strs, string s) {
            var i = strs.IndexOf(s);
            if (i < 0) {
                i = strs.Count;
                strs.Add(s);
            }
            return i;
        }

        private static void DoUnsafe (TextReader reader) {
            var r = new Regex(@"^(\d+) (\w+)$", RegexOptions.Compiled);
            var events = new List<(long t, int e)>();
            foreach (var l in Lines(reader)) {
                var m = r.Match(l);
                if (!m.Success)
                    continue;
                var ticks = long.Parse(m.Groups[1].Value);
                var name = m.Groups[2].Value;
                var i = TryAdd(strings, name);
                events.Add((ticks, i));
            }
            if (events.Count == 0)
                return;
            var frames = new List<(long t, int i)[]>();
            var frame = new List<(long t, int i)>();
            foreach (var (e, i) in events) {
                if (i == 0 && frame.Count > 0) {
                    frames.Add(frame.ToArray());
                    frame.Clear();
                }
                frame.Add((e, i));
            }
            frames.Add(frame.ToArray());
        }
    }
}
