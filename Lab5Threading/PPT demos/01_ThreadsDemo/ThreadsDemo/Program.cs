using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace ThreadsDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            int threadCount = 0;
            // zdarzenie na ktorym zablokujemy wszystkie watki
            using (var re = new ManualResetEvent(false))
            {
                // zapiszmy ile pamieci jest zuzyte na "dzien dobry"
                long privateMemoryStart = Process.GetCurrentProcess().PrivateMemorySize64;
                try
                {
                    while (true)
                    {
                        // utworzmy nowy watek 
                        // UWAGA: new Thread nie powoduje utworzenia fizycznego wątku w systemie
                        var thread = new Thread((ctx) =>
                        {
                            // zwiekszmy licznik wątków
                            int current = Interlocked.Increment(ref threadCount);
                            Console.WriteLine(string.Format(
                                CultureInfo.InvariantCulture, "thread {0} started...", current));
                            // zablokujmy watek do czasu zdarzenia
                            re.WaitOne();
                            Interlocked.Decrement(ref threadCount);
                        });
                        // uruchomienie watku - fizyczne utworzenie watku
                        thread.Start();                        
                    }
                }
                    // chyba cos poszlo nie tak....
                catch (Exception e)
                {
                    Console.WriteLine("start");
                    Console.WriteLine("Private memory: " + privateMemoryStart.ToString("N"));
                    
                    // ciekawe jak wygladamy z pamiecia
                    long privateMemoryStop = Process.GetCurrentProcess().PrivateMemorySize64;

                    Console.WriteLine("stop");
                    Console.WriteLine("Private memory: " + privateMemoryStop.ToString("N"));
                    // ile pamieci jest zuzywane na watek?
                    Console.WriteLine("memory per thread: " + 
                        ((privateMemoryStop - privateMemoryStart)/threadCount).ToString("N"));

                    Console.WriteLine(e.GetType().Name);
                    Debugger.Break();
                    // pozwolmy wszystkim watkam sie zakonczyc
                    re.Set();

                    //dopoki wszystkie sie nie zakoncza - poczekajmy
                    while (Thread.VolatileRead(ref threadCount) != 0)
                    {
                        ;
                    }
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "all threads done {0}", threadCount));
                    Debugger.Break();
                }
            }
        }
    }
}
