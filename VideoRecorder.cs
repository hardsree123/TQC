using Accord.Video.FFMPEG;
using System;
using System.Drawing;

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
            return _recording;
        }

        public void StopRecording()
        {
            this._recording = false;
            _writer.Close();
            _writer.Dispose();
        }

        public void NewRecording(string fileName, int width, int height, double frameRate)
        {
            this._writer = new VideoFileWriter();
            fileName = @"F:\Projects\L.TYREQC\Code\Recordings\"+ fileName;
            this._recording = true;
            this._writer.Open(fileName, width, height);
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
