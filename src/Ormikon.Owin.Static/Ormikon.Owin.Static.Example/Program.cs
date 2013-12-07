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
                #if DEBUG //mono debug
                var cki = Console.ReadKey();
				if (cki.KeyChar == '\0')
                    while(true){}//if debug in mono
                #else
                Console.ReadKey();
                #endif
            }
        }
    }
}
