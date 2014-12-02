using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace AsyncWithIteratorDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var mre = new ManualResetEvent(false))
            {
                var ctrl = new AsyncController();
                ctrl.AllDone = () => { mre.Set(); };

                // rozpocznij asynchroniczny odczyt z pliku
                ctrl.BeginOperation(ReadFileOperation(ctrl));

                mre.WaitOne();
            }
            Console.WriteLine("all done!");
            Console.ReadKey();
        }

        // metoda, ktora de-facto implementuje maszyne stanowa pozwalajaca na asynchroniczny
        // odczyt pliku w obrebie jednej metody z zachowaniem wszystkich udogodnien jezyka!
        static IEnumerator<int> ReadFileOperation(AsyncController ctrl)
        {
            // otworz plik - dyrektywa USING!!
            using (var fs = new FileStream("Content.txt",
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                512,
                // otworz plik w trybie asynchronicznym!
                FileOptions.Asynchronous))
            {

                byte[] buffer = new byte[fs.Length];
                // rozpocznij odczyt
                // UWAGA - parametr callback odnosi sie do metody w kontrolerze maszyny stanowej
                IAsyncResult ar = fs.BeginRead(
                    buffer, 
                    0, 
                    buffer.Length, 
                    ctrl.AsyncOperationCompleted, 
                    null);

                Console.WriteLine("Starting async operation on thread: "
                    + Thread.CurrentThread.ManagedThreadId);

                // zwroc kontrole do kontrollera maszyny stanowej - wykonanie kodu jest przerwane!
                yield return 1;

                // ta linia kodu jest wykonywana po zakonczeniu odczytu - kontroller
                // maszyny stanowej wznawia iteracje!
                int bytesRead = fs.EndRead(ar);

                // odczyt danych 
                using (var ms = new MemoryStream(buffer))
                using (var str = new StreamReader(ms))
                {
                    string fileName = null;

                    // odczyt danych z pilku - linia po linii
                    while (!string.IsNullOrEmpty(fileName = str.ReadLine()))
                    {
                        // dla każdej linii - sprobuj otworzy nowy plik
                        using (var contentFs = new FileStream(
                            fileName,
                            FileMode.Open,
                            FileAccess.Read,
                            FileShare.Read,
                            512,
                            // otworz plik w trybie asynchronicznym!
                            FileOptions.Asynchronous))
                        {
                            buffer = new byte[contentFs.Length];
                            // rozpocznij odczyt z pliku
                            IAsyncResult contentAr = contentFs.BeginRead(
                                buffer,
                                0,
                                buffer.Length,
                                ctrl.AsyncOperationCompleted,
                                null);

                            // poczekaj az dane zostana odczytane
                            yield return 1;

                            contentFs.EndRead(contentAr);

                            // i wyswietl je na konsole
                            using (var contentMs = new MemoryStream(buffer))
                            using (var contentStr = new StreamReader(contentMs))
                            {
                                Console.Write(contentStr.ReadToEnd());
                            }
                        }
                    }

                    Console.WriteLine("Read Completed on thread " +
                        Thread.CurrentThread.ManagedThreadId);

                }
                // zakonczenie przetwarzania - nie ma wiecej operacji do wykonania!
            }
        }
    }

    // bardzo prosta (i ZLA) implementacja kontrolera maszyny stanowej - 
    // poprawna wersja - http://www.wintellect.com/PowerThreading.aspx
    class AsyncController
    {
        public AsyncController()
        {
            this.AsyncOperationCompleted = this.OnAsyncOperationCompleted;
        }
     
   
        public AsyncCallback AsyncOperationCompleted { get; private set; }
        public Action AllDone { get; set; }
        private IEnumerator<int> stateMachine;
        private int waitOps;
        private int canRun = 1;

        private void MoveToNext()
        {
            Interlocked.Exchange(ref this.canRun, 0);
            if (this.stateMachine.MoveNext())
            {
                waitOps = this.stateMachine.Current;
            }
            else if (null != this.AllDone)
            {
                this.AllDone();
            }
            Interlocked.Exchange(ref this.canRun, 1);
        }

        private void OnAsyncOperationCompleted(IAsyncResult ar)
        {
            if (Thread.VolatileRead(ref canRun) == 1)
            {
                if (Interlocked.Decrement(ref waitOps) == 0)
                {
                    this.MoveToNext();
                }
            }
            else
            {
                ThreadPool.QueueUserWorkItem(
                    (state) => { this.OnAsyncOperationCompleted((IAsyncResult)state); }, ar);
            }
        }

        public void BeginOperation(IEnumerator<int> machine)
        {
            this.stateMachine = machine;
            this.MoveToNext();
        }
    }
}
