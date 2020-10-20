using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiThreadCopy
{
    class Program
    {
        static void Main(string[] args)
        {
            int threadsCount;
            if (!(args.Length == 3))
            {
                Console.WriteLine("Wrong Input.\nFormat: MultiThreadCopier.exe <SourceDir> <DestDir> <ThreadsNumber>");
                return;
            }

            if (!int.TryParse(args[2], out threadsCount))
            {
                Console.WriteLine("Wrong Threads Number.");
                return;
            }

            try
            {
                MultiThreadCopier copier = new MultiThreadCopier(args[0], args[1], threadsCount);
                copier.Copy();
                Console.WriteLine(copier.GetStatistics());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return;
        }
    }
}
