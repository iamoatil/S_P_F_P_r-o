using System;
using System.ServiceModel;
using X86DllServer.Service;

namespace DllServerHost
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (ServiceHost host = new ServiceHost(typeof(X86DLLService)))
                {
                    host.Open();
                    Console.WriteLine("服务开启！");
                    Console.Read();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.Read();
            }
        }
    }
}
