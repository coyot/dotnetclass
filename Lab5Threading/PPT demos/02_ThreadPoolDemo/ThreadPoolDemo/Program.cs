using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ThreadPoolDemo
{
    class Program
    {
        private const int duration = 3000;
        private static int index = 0;
        private static int[] usedIds;

        static void Main(string[] args)
        {
            RunDemo(ComputeBoundOperation, "compute bound");
            RunDemo(IOBoundOperation, "i/o bound");
            RunDemo(ComputeBoundOperation, "compute bound again");
        }

        static void RunDemo( Action method, string description)
        {
            // index od ktorego bedziemy zapisywac identyfikatory watkow
            index = 0;
            // tablica w ktorej przechowujemy identyfikatory watkow wykonujacych zadanie
            usedIds = new int[20];

            // zakolejkujmy n zadan do wykonania
            for (int i = 0; i < usedIds.Length; ++i)
            {
                ThreadPool.QueueUserWorkItem(ThreadOperation, method);
            }

            Console.WriteLine("Started all threads for " + description + 
                " main thread: " + Thread.CurrentThread.ManagedThreadId);
            
            // dopoki wszystkie operacje sie nie zakoncza - 
            // zwracajmy time slot do systemu operacyjnego
            while (Thread.VolatileRead(ref index) != 20)
            {
                SwitchToThread();
            }

            // ile watkow zostalo wykorzystanych..?
            int numberUsed = usedIds
                .Where(p => p != 0)
                .Distinct()
                .Count();

            Console.WriteLine("number of threads used: " + numberUsed +
                              "\r\nnumber of processors: " + Environment.ProcessorCount +
                              "\r\npress key to continue...");
            Console.ReadKey();            
        }

        static void ThreadOperation(object state)
        {
            Console.WriteLine("thread ID " + Thread.CurrentThread.ManagedThreadId + " start");

            //wykonajmy akcje
            ((Action) state)();

            // zapiszmy identyfikator
            usedIds[Interlocked.Increment(ref index) - 1] = Thread.CurrentThread.ManagedThreadId;

            if (index == 20)
            {
                Console.WriteLine("press a key to continue");
            }
        }

        static void ComputeBoundOperation()
        {
            DateTime end = DateTime.Now + new TimeSpan(0, 0, 0, 0, duration);
            int i = 0;

            // zasymulujmy jakas zlozona operacje - 
            // ciasna petla w wersji debug powinna zalatwic sprawe
            while (DateTime.Now < end)
            {
                ++i;
            }
        }

        static void IOBoundOperation()
        {
            // symulacja operacji WE/WY - watek nie ma nic do roboty!
            Thread.Sleep(duration);
        }

        [DllImport("Kernel32", ExactSpelling = true)]
        private static extern Boolean SwitchToThread();
    }
}
