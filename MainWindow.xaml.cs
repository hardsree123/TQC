using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace TQC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoCaptureDevice outerVdoSource = null, innerVdoSource = null;
        VideoRecorder recOuterVdo = null, recInnerVdo = null;
        private JPEGStream innerJpegStream = null, outerJpegStream = null;
        private MJPEGStream innerMjpegStream = null, outerMjpegStream = null;
        public MainWindow()
        {
            InitializeComponent();
            LoadUsbCameras();
        }

        private void LoadUsbCameras()
        {
            var videoDeviceList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo fc in videoDeviceList)
            {
                vdoOuterDeviceList.Items.Add(fc.Name);
                vdoInnerDeviceList.Items.Add(fc.Name);
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RecordOuter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SerialNo.Text))
            {
                MessageBox.Show("Enter serial number");
            }
            else
            {
                if ((bool)outerJpeg.IsChecked)
                {
                    if (outerJpegStream != null && outerJpegStream.IsRunning)
                    {
                        outerJpegStream.Stop();
                        recOuterVdo?.StopRecording();
                        RecordOuter.Content = "Record";
                    }
                    else
                    {
                        outerJpegStream = new JPEGStream(outerIPCamUrl.Text);
                        StartStreamingForOuter(outerJpegStream);
                        RecordOuter.Content = "Stop";
                    }
                }
                else // UseMJpegStream
                {
                    if (outerMjpegStream != null && outerMjpegStream.IsRunning)
                    {
                        outerMjpegStream.Stop();
                        recOuterVdo?.StopRecording();
                        RecordOuter.Content = "Record";
                    }
                    else
                    {
                        outerMjpegStream = new MJPEGStream(outerIPCamUrl.Text);
                        StartStreamingForOuter(outerMjpegStream);
                        RecordOuter.Content = "Stop";
                    }
                }
            }
        }

        private void StartStreamingForOuter(JPEGStream _videoSource)
        {
            if (recOuterVdo == null || !recOuterVdo.RecordingStarted())
            {
                recOuterVdo = new VideoRecorder();
                recOuterVdo.SerialNumber = SerialNo.Text;
            }
            _videoSource.NewFrame += new NewFrameEventHandler(outervideo_NewFrame);
            _videoSource.Start();
        }

        private void StartStreamingForOuter(MJPEGStream _videoSource)
        {
            _videoSource.NewFrame += new NewFrameEventHandler(outervideo_NewFrame);
            _videoSource.Start();
        }


        private void RecordInner_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SerialNo.Text))
            {
                MessageBox.Show("Enter serial number");
            }
            else
            {
                if ((bool)innerJpeg.IsChecked)
                {
                    if (innerJpegStream != null && innerJpegStream.IsRunning)
                    {
                        innerJpegStream.Stop();
                        recInnerVdo?.StopRecording();
                        RecordInner.Content = "Record";
                    }
                    else
                    {
                        innerJpegStream = new JPEGStream(innerIPCamUrl.Text);
                        StartStreamingForInner(innerJpegStream);
                        RecordInner.Content = "Stop";
                    }
                }
                else // UseMJpegStream
                {
                    if (innerMjpegStream != null && innerMjpegStream.IsRunning)
                    {
                        innerMjpegStream.Stop();
                        recInnerVdo?.StopRecording();
                        RecordInner.Content = "Record";
                    }
                    else
                    {
                        innerMjpegStream = new MJPEGStream(innerIPCamUrl.Text);
                        StartStreamingForInner(innerMjpegStream);
                        RecordInner.Content = "Stop";
                    }
                }
            }
        }

        private void StartStreamingForInner(JPEGStream _videoSource)
        {
            if (recInnerVdo == null || !recInnerVdo.RecordingStarted())
            {
                recInnerVdo = new VideoRecorder();
                recInnerVdo.SerialNumber = SerialNo.Text;
            }
            _videoSource.NewFrame += new NewFrameEventHandler(innervideo_NewFrame);
            _videoSource.Start();
        }

        private void StartStreamingForInner(MJPEGStream _videoSource)
        {
            _videoSource.NewFrame += new NewFrameEventHandler(innervideo_NewFrame);
            _videoSource.Start();
        }



        private void UseInnerJpeg(object sender, RoutedEventArgs e)
        {

        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void UseOuterJpeg(object sender, RoutedEventArgs e)
        {

        }

        private bool CheckAlreadyExisting()
        {
            if (vdoOuterDeviceList.SelectedIndex.Equals(vdoInnerDeviceList.SelectedIndex))
            {
                return true;
            }
            return false;
        }

        private void StopOuter_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// function to initiate outer cam feed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordOuterCamFeed_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SerialNo.Text))
            {
                MessageBox.Show("Enter serial number");
            }
            else
            {
                try
                {
                    if (recOuterVdo != null)
                    {
                        RecordOuterCamFeed.Content = "Record cam feed";
                        recOuterVdo?.StopRecording();
                        recOuterVdo = null;
                    }
                    else
                    {

                        if (CheckAlreadyExisting())
                        {
                            innerVdoSource?.Stop();
                            innerVdoSource = null;
                        }
                        if (recOuterVdo == null || !recOuterVdo.RecordingStarted())
                        {
                            recOuterVdo = new VideoRecorder();
                            recOuterVdo.SerialNumber = SerialNo.Text;
                        }
                        var videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                        outerVdoSource = new VideoCaptureDevice(videoDevicesList[vdoOuterDeviceList.SelectedIndex].MonikerString);
                        outerVdoSource.NewFrame += new NewFrameEventHandler(outervideo_NewFrame);
                        outerVdoSource.Start();
                        RecordOuterCamFeed.Content = "Recording... (Click to stop)";
                    }
                }
                catch (Exception ex)
                {
                    RecordOuterCamFeed.Content = "Record cam feed";
                }
            }
        }
        /// <summary>
        /// function to initiate inner cam feed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordInnerCamFeed_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SerialNo.Text))
            {
                MessageBox.Show("Enter serial number");
            }
            else
            {
                try
                {
                    if (recInnerVdo != null)
                    {
                        RecordInnerCamFeed.Content = "Record cam feed";
                        recInnerVdo?.StopRecording();
                        recInnerVdo = null;
                    }
                    else
                    {
                        if (CheckAlreadyExisting())
                        {
                            outerVdoSource?.Stop();
                            outerVdoSource = null;
                        }
                        if (recInnerVdo == null || !recInnerVdo.RecordingStarted())
                        {
                            recInnerVdo = new VideoRecorder();
                            recInnerVdo.SerialNumber = SerialNo.Text;
                        }
                        var videoDevicesList = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                        innerVdoSource = new VideoCaptureDevice(videoDevicesList[vdoInnerDeviceList.SelectedIndex].MonikerString);
                        innerVdoSource.NewFrame += new NewFrameEventHandler(innervideo_NewFrame);
                        innerVdoSource.Start();
                        RecordInnerCamFeed.Content = "Recording... (Click to stop)";
                    }
                }
                catch (Exception ex)
                {
                    RecordInnerCamFeed.Content = "Record cam feed";
                }
            }
        }

        private void outervideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (recOuterVdo != null)
            {
                if (!recOuterVdo.RecordingStarted())
                {
                    recOuterVdo.NewRecording("OV_" + recOuterVdo.SerialNumber, eventArgs.Frame.Width, eventArgs.Frame.Height, 15.0);
                    recOuterVdo.RecordVideo(eventArgs.Frame);//first frame
                }
                else
                {
                    recOuterVdo.RecordVideo(eventArgs.Frame);//independent thread to record the video from streamed bitmap frame.
                }
            }
            // get new frame
            System.Drawing.Image imgforms = (Bitmap)eventArgs.Frame.Clone();
            
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            MemoryStream ms = new MemoryStream();
            imgforms.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);

            bi.StreamSource = ms;
            bi.EndInit();

            //Using the freeze function to avoid cross thread operations 
            bi.Freeze();

            //Calling the UI thread using the Dispatcher to update the 'Image' WPF control         
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                outerVdo.Source = bi; /*frameholder is the name of the 'Image' WPF control*/
            }));

        }

        private void innervideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (recInnerVdo != null)
            {
                if (!recInnerVdo.RecordingStarted())
                {
                    recInnerVdo.NewRecording("IV_" + recInnerVdo.SerialNumber, eventArgs.Frame.Width, eventArgs.Frame.Height, 15.0);
                    recInnerVdo.RecordVideo(eventArgs.Frame);//first frame
                }
                else
                {
                    recInnerVdo.RecordVideo(eventArgs.Frame);//independent thread to record the video from streamed bitmap frame.
                }
            }

            // get new frame
            System.Drawing.Image imgforms = (Bitmap)eventArgs.Frame.Clone();

            BitmapImage bi = new BitmapImage();
            bi.BeginInit();

            MemoryStream ms = new MemoryStream();
            imgforms.Save(ms, ImageFormat.Bmp);
            ms.Seek(0, SeekOrigin.Begin);

            bi.StreamSource = ms;
            bi.EndInit();

            //Using the freeze function to avoid cross thread operations 
            bi.Freeze();

            //Calling the UI thread using the Dispatcher to update the 'Image' WPF control         
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                innerVdo.Source = bi; /*frameholder is the name of the 'Image' WPF control*/
            }));

        }
        private void vdoInnerDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void vdoOuterDeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
