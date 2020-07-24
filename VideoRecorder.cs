using Accord.Video.FFMPEG;
using System;
using System.Drawing;
using System.IO;

namespace TQC
{
    public class VideoRecorder : TyreQCCheckList
    {
        private VideoFileWriter _writer;
        private DateTime? firstFrameTime = null;
        private bool _recording;
        public VideoRecorder()
        {
        }

        public bool RecordingStarted()
        {
            return this._recording;
        }

        public void StopRecording()
        {
            lock (this)
            {
                _writer.Close();
                _writer.Dispose();
                this._recording = false;
            }
        }

        public void NewRecording(string fileName, int width, int height, double frameRate)
        {
            try
            {
                this._writer = new VideoFileWriter();
                string path = Directory.GetCurrentDirectory() + @"\Recordings\" ;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                fileName = path + fileName;
                //cannot provide the video type .mp4 that throws some exception in accord.
                //cannot specify h264 encoding in accord that already has a recording issue.
                this._recording = true;
                this._writer.Open(fileName, width, height);
            }
            catch (Exception ex)
            { 
                throw ex;
            }
        }

        public void RecordVideo(Bitmap frame)
        {
            lock (this)
            {
                TimeSpan ts;
                try
                {
                    if (this._recording && _writer != null)
                    {
                        if (firstFrameTime != null)
                        {
                            ts = TimeSpan.FromMilliseconds(DateTime.Now.Subtract(firstFrameTime.Value).TotalMilliseconds);
                            _writer.WriteVideoFrame(frame,ts);
                        }
                        else
                        {
                            _writer.WriteVideoFrame(frame);
                            firstFrameTime = DateTime.Now;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

    }
}
