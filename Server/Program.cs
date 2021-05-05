using System;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            DataBaseController dataBaseController = new DataBaseController("127.0.0.1", 3306, "root", "root");

            Console.Read();

        }
    }
}
