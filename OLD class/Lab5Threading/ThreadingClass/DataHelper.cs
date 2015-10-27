using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingClass
{
    public static class DataHelper
    {
        public static string LoadData()
        {
            using (StreamReader rdr = new StreamReader("input.txt"))
            {
                return rdr.ReadToEnd();
            }
        }

        public static ICollection<Func<int>> FindStringCountActions(string input, string searchTerm, long[] timeTaken, int? concurrencyOverride)
        {
            List<Func<int>> functions = new List<Func<int>>();
            Stopwatch sw = new Stopwatch();

            int totalParts = concurrencyOverride.GetValueOrDefault(Environment.ProcessorCount);
            int firstTaskStarting = 0;
            int lastTaskCompleting = 0;

            foreach (var part in DataHelper.DivideInputString(input, totalParts))
            {
                int startIdx = part.Item1;
                int len = part.Item2;

                Func<int> workDelegate = () =>
                    {
                        if (Interlocked.Increment(ref firstTaskStarting) == 1)
                        {
                            sw.Start();
                        }

                        int result = FindStringCount(input, startIdx, len, searchTerm);

                        if (Interlocked.Increment(ref lastTaskCompleting) == totalParts)
                        {
                            sw.Stop();
                            timeTaken[0] = sw.ElapsedMilliseconds;
                        }
                        return result;
                    };

                functions.Add(workDelegate);
            }

            return functions;
        }

        private static int FindStringCount(string input, int startIndex, int len, string searchTerm)
        {
            int foundCount = 0;
            char[] items = searchTerm.ToCharArray();
            int charsToSearch = searchTerm.Length;
            int findIndex = 0;

            for (int i = startIndex; i < startIndex + len + searchTerm.Length - 1 && i < input.Length; ++i)
            {
                char current = input[i];
                if (items[findIndex] == current)
                {
                    ++findIndex;
                    if (findIndex == charsToSearch)
                    {
                        ++foundCount;
                        findIndex = 0;
                    }
                }
                else
                {
                    findIndex = 0;
                }
                if (i % 100 == 0)
                {
                    Thread.SpinWait(50000);
                }
            }
            return foundCount;
        }

        private static IEnumerable<Tuple<int, int>> DivideInputString(string input, int concurrencyLevel = 1)
        {
            if (1 == concurrencyLevel)
            {
                yield return Tuple.Create(0, input.Length);
                yield break;
            }

            int partSize = input.Length / concurrencyLevel;
            int start = 0;

            for (int i = 0; i < concurrencyLevel; ++i)
            {
                yield return Tuple.Create(start, partSize);
                start += partSize;
            }
        }
    }
}
