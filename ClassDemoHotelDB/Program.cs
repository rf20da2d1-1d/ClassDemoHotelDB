using System;

namespace ClassDemoHotelDB
{
    class Program
    {
        static void Main(string[] args)
        {
            DBWorker worker = new DBWorker();
            worker.Start();

            Console.WriteLine("Slut");
        }
    }
}
