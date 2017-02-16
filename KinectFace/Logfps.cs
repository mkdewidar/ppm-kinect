using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

namespace KinectFace
{
    class Logfps
    {
        static ThreadStart writeRef = new ThreadStart(writeNewFrameToFile);
        Thread _writeThread =new Thread(writeRef);
        
        long _frametime;
       static Queue<long> frameTimesToWrite =new Queue<long>() ;
        public Logfps()
        {
            _writeThread.Name = "Write Thread";
            _writeThread.Start();
        }
       public void addNewFrameTime(long frametime)
        {
            _frametime = frametime;
            
                frameTimesToWrite.Enqueue(frametime);
                  
        }

        private static void writeNewFrameToFile()
        {
            String path = Path.Combine(Environment.CurrentDirectory, @"frameratelog.txt");
            StreamWriter frametimeWriter = new StreamWriter(path);
            while (true)
            {
                if (frameTimesToWrite.Count != 0) { 
                    long currentFrameTime = frameTimesToWrite.Dequeue();
                    frametimeWriter.WriteLine(currentFrameTime);
                    frametimeWriter.Flush();
                }
            }
        }
    }
}
