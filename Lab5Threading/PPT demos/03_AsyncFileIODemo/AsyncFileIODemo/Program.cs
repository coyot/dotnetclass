using System;
using System.IO;
using System.Threading;

namespace AsyncFileIODemo
{
    class Program
    {
        // bufor na dane z pliku
        private static byte[] buffer;

        static void Main(string[] args)
        {
            // otworz plik 
            var fs = new FileStream("Content.txt", 
                FileMode.Open, 
                FileAccess.Read, 
                FileShare.Read, 
                512, 
                // otworz plik w trybie asynchronicznym!
                FileOptions.Asynchronous);

            buffer = new byte[fs.Length];
            // rozpocznij odczyt
            // UWAGA: ostatni element, stan - przekazuje referencje do strumienia!
            fs.BeginRead(buffer, 0, buffer.Length, CompleteRead, fs);

            // zablokujmy glowny watek
            Console.WriteLine("waiting for complete read... thread: " +
                Thread.CurrentThread.ManagedThreadId);
            Console.ReadKey();
        }

        static void CompleteRead(IAsyncResult ar)
        {
            // pobierz referencje do strumienia
            var fs = (FileStream) ar.AsyncState;
            // zakoncz odczyt danych - pobierz dane zwrocone przez system
            int bytesRead = fs.EndRead(ar);
            // zamknij strumien
            fs.Dispose();

            // odczyt danych i wydruk na konsole
            using (var ms = new MemoryStream(buffer, 0, bytesRead, false))
            using (var str = new StreamReader(ms))
            {
                Console.WriteLine(str.ReadToEnd());
                Console.WriteLine("Read Completed on thread " + 
                    Thread.CurrentThread.ManagedThreadId);
            }
            
        }
    }
}
