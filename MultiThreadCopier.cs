using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MultiThreadCopy
{
    public class MultiThreadCopier
    {
        private TaskQueue taskQueue;

        private int filesCopied;

        private int errorsCount;

        private readonly string source;

        private readonly string destination;

        public MultiThreadCopier(string src, string dest, int threadsCount)
        {
            this.source = src;
            this.destination = dest;

            if (String.Compare(source, destination) == 0)
                throw new ArgumentException("Source path and Destination path cannot be the same.");

            if (!Directory.Exists(source))
                throw new ArgumentException("Wrong Source Path.");

            if (!Directory.Exists(destination))
            {
                try
                {
                    Directory.CreateDirectory(destination);
                }
                catch
                {
                    throw new Exception("Failed to create destination directory.");
                }
            }

            if (threadsCount <= 0)
                this.taskQueue = new TaskQueue();
            else
                this.taskQueue = new TaskQueue(threadsCount);
        }

        public void Copy()
        {
            string[] directories = Directory.GetDirectories(source, "*", SearchOption.AllDirectories);
            foreach (var dir in directories)
            {
                string newDir = dir.Replace(source, destination);
                try
                {
                    Directory.CreateDirectory(newDir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Cannot Create Directory. Error ({ex.Message}).");
                }
            }

            string[] files = Directory.GetFiles(source, "*.*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string newFile = file.Replace(source, destination);
                taskQueue.EnqueueTask(
                    delegate {
                        try
                        {
                            File.Copy(file, newFile, true);
                            Console.WriteLine($"Copied:\n   from {file} \n      to {newFile}");
                            Interlocked.Increment(ref filesCopied);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Copying Error ({ex.Message}):\n     from {file}\n       to {newFile}");
                            Interlocked.Increment(ref errorsCount);
                        }
                    });
            }

            taskQueue.Stop();
        }

        public string GetStatistics()
        {
            return $"Files copied: {this.filesCopied}\nErrors occured: {this.errorsCount}";
        }
    }
}
