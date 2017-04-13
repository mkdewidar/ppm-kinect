using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;
namespace KinectFace
{
    class Logfps
    {
        private static Stopwatch sampletimer = new Stopwatch();
        static ThreadStart writeRef = new ThreadStart(writeNewFrameToFile);
        Thread _writeThread =new Thread(writeRef);
        public static bool _faceDetected;
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
                while (sampletimer.ElapsedMilliseconds<=30000)
                   {
                    if (frameTimesToWrite.Count != 0&&!_faceDetected)
                    {
                    sampletimer.Start();
                        long currentFrameTime = frameTimesToWrite.Dequeue();
                        frametimeWriter.WriteLine(currentFrameTime);
                        frametimeWriter.Flush();
                    }
                }
            sampletimer.Stop();
        }
    }
}
