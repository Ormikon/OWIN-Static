using System;
using Microsoft.Owin.Hosting;

namespace Ormikon.Owin.Static.Example
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = args.Length > 0 ? args[0] : "http://*:8084";

            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine("OWIN Web application started on {0}", url);
                Console.Write("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
