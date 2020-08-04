using System;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                await new ClientMaster().Start();
            }
            catch (Exception e)
            {
                //log.Error
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //log.Fatal
        }
    }
}
