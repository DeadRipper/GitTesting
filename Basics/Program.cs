using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basics
{
	class Program
	{
		static void Main(string[] args)
		{
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            var a = IPAddress.Loopback;
            var b = IPAddress.Any;
            var c = IPAddress.Broadcast;

            IPHostEntry host = Dns.GetHostEntry(a);
            Console.WriteLine(host.HostName);

            foreach(IPAddress _ip in host.AddressList)
                Console.WriteLine(_ip.ToString());

            IPHostEntry _host = Dns.GetHostEntry("somesite.com");
            Console.WriteLine(_host.HostName);

            foreach(IPAddress ip1 in _host.AddressList)
                Console.WriteLine(ip1.ToString());

            WebClient client = new WebClient();

            try
            {
                using (Stream s = client.OpenRead($"{_host.HostName}" + "/sometest.txt"))
                {
                    using (StreamReader reader = new StreamReader(s))
                    {
                        string line = "";

                        while ((line == reader.ReadLine()) != null)
                            Console.WriteLine(line);
                    }
                }
            }

            catch (Exception ex) { Console.WriteLine(ex.Message); }

            finally
            {
                DownloadFile().GetAwaiter();
            }


           //try
           //{
           //     client.DownloadFile(ip.ToString(), "dddd.pdf");
           //     client.OpenRead(ip.ToString());
           //     Console.WriteLine("Got it " + client.ResponseHeaders);
           //}
           //catch (Exception ex) { Console.WriteLine(ex.Message); }
           //finally
           //{
           //     Console.WriteLine("Got it");
           //}

			Console.ReadKey();
		}

        private static async Task DownloadFile()
        {
            WebClient client = new WebClient();
            await client.DownloadFileTaskAsync(new Uri("https://somesite.com/sometest.txt"), "myfile.txt");
        }
    }
}