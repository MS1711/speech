using Microsoft.Speech.TtsService.HttpClient;
using NAudio.Wave;
using NL2ML.models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VoiceReceiver
{
    public enum CortanaStatus
    {
        Calm,
        Thinking,
        Speaking
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IWaveIn waveIn;
        private WaveFileWriter writer;
        private bool isRecording = false;
        private NL2ML.api.NL2ML ins = NL2ML.api.NL2ML.Instance;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SpeakButton2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void mediaEleCalm_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaEleCalm.Position = new TimeSpan(0, 0, 0);
            mediaEleCalm.Play();
        }

        private void mediaEleListen_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaEleListen.Position = new TimeSpan(0, 0, 0);
            mediaEleListen.Play();
        }

        private void mediaEleThink_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaEleThink.Position = new TimeSpan(0, 0, 0);
            mediaEleThink.Play();
        }

        private void mediaEleSpeak_MediaEnded(object sender, RoutedEventArgs e)
        {
            mediaEleSpeak.Position = new TimeSpan(0, 0, 0);
            mediaEleSpeak.Play();
        }

        private void SpeakButton2_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartRecordUserVoice();
        }

        private void CreateWaveInDevice()
        {
            waveIn = new WaveIn();
            waveIn.WaveFormat = new WaveFormat(8000, 1);

            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped;
        }

        void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            FinalizeWaveFile();
        }

        void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (writer != null)
            {
                writer.Write(e.Buffer, 0, e.BytesRecorded);
            }
        }

        private void Cleanup()
        {
            if (waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
            }
            FinalizeWaveFile();
        }

        private void FinalizeWaveFile()
        {
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }

        private async void SpeakButton2_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ProcessUserVoice();

        }

        private void SwitchTo(CortanaStatus cortanaStatus)
        {
            this.mediaEleCalm.Stop();
            this.mediaEleCalm.Visibility = Visibility.Collapsed;
            this.mediaEleListen.Stop();
            this.mediaEleListen.Visibility = Visibility.Collapsed;
            this.mediaEleThink.Stop();
            this.mediaEleThink.Visibility = Visibility.Collapsed;
            this.mediaEleSpeak.Stop();
            this.mediaEleSpeak.Visibility = Visibility.Collapsed;
            Grid.SetRowSpan(this.mediaEleSpeak, 2);

            switch (cortanaStatus)
            {
                case CortanaStatus.Calm:
                    {
                        this.mediaEleCalm.Play();
                        this.mediaEleCalm.Visibility = Visibility.Visible;
                        break;
                    }
                case CortanaStatus.Thinking:
                    {
                        this.mediaEleThink.Play();
                        this.mediaEleThink.Visibility = Visibility.Visible;
                        break;
                    }
                case CortanaStatus.Speaking:
                    {
                        this.mediaEleSpeak.Play();
                        this.mediaEleSpeak.Visibility = Visibility.Visible;
                        Grid.SetRowSpan(this.mediaEleSpeak, 1);
                        break;
                    }
            }
        }

        private string resultProcess(string result)
        {
            string _result = result;
            //处理天气  
            if (_result.Contains("°C"))
            {
                _result = _result.Replace("°C", "℃");
                int h;
                for (int i = 0; i < _result.Length; i++)
                {
                    if (_result[i] == '℃' || _result[i] == '~')
                    {
                        h = i - 1;
                        while (h >= 0)
                        {
                            if (_result[h] >= '0' && _result[h] <= '9')
                                h--;
                            else
                                break;
                        }
                        if (h >= 0 && _result[h] == '-')
                        {

                            _result.Insert(h, "零下");
                            _result = _result.Remove(h, 1);
                        }

                    }
                }
            }
            return _result;
        }

        public async Task<bool> WaveGenerate(string str)
        {
            str = TextToSsml(str);
            int tryTimes = 0;
            HttpStatusCode status = HttpStatusCode.Unused;
            byte[] waveHead = new byte[48];
            while (tryTimes < 2)
            {
                using (MemoryStream fs = new MemoryStream())
                {
                    try
                    {
                        bool flag = await TtsServiceHelper.SynthesizeUsingService("111.221.30.185", 80, "TTSService-PROD-HK2", str, "riff-16khz-16bit-mono-pcm", fs, status);
                        if (flag)
                        {
                            string tempPath = System.IO.Path.GetTempPath();
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, (int)fs.Length);
                            using (FileStream sw = new FileStream(System.IO.Path.Combine(tempPath, "sample.wav"), FileMode.Create, FileAccess.ReadWrite))
                            {
                                await sw.WriteAsync(buffer, 0, buffer.Length);
                            }
                        }
                    }
                    catch (WebException e)
                    {
                        if (e.Status == WebExceptionStatus.RequestCanceled)
                        {
                            status = HttpStatusCode.NotFound;
                        }
                    }
                }

                if (status == HttpStatusCode.NotFound)
                {
                    tryTimes++;
                }
                else
                {
                    break;
                }
            }
            return true;
        }

        //将text转换为SSML
        private string TextToSsml(string str)
        {
            string[] sEmphasis = { "none", "strong", "moderate", "reduced" };
            string[] sPitch = { "default", "x-high", "x-low", "x-low" };
            string[] sRate = { "default", "fast", "slow", "x-slow" };
            string[] sVolume = { "default", "default", "loud", "loud" };
            string tmp;
            int index = 0;


            tmp = "<?xml version=\"1.0\"?>\r\n<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\"\r\n"
            + "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n"
            + "xsi:schemaLocation=\"http://www.w3.org/2001/10/synthesis\r\n"
            + "http://www.w3.org/TR/speech-synthesis/synthesis.xsd\"\r\n"
            + "xml:lang=\"zh-CN\">\r\n";

            //ComboBox a = new ComboBox();
            //paras[0] = ((ComboBoxItem)EmphasisList.SelectedItem).Content.ToString();
            //paras[1] = ((ComboBoxItem)PitchList.SelectedItem).Content.ToString();
            //paras[2] = ((ComboBoxItem)RateList.SelectedItem).Content.ToString();
            //paras[3] = ((ComboBoxItem)VolumeList.SelectedItem).Content.ToString();


            tmp += "<voice gender = \"female\" age = \"30\">"
                + "<emphasis level=\"" + sEmphasis[index] + "\">\r\n"
                + "<prosody pitch=\"" + sPitch[index] + "\" rate=\"" + sRate[index]
                + "\" volume=\"" + sVolume[index] + "\">\r\n"
                + str + "</prosody>\r\n</emphasis>\r\n</voice>\r\n</speak>";

            return tmp;
        }

        private async Task<bool> pronounce(string ack)
        {
            media.Stop();
            media.Source = null;
            media.Source = new Uri(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "sample.wav"));
            media.Play();
            resultDetails2.Text = ack;
            return true;
        }

        private void media_MediaEnded(object sender, RoutedEventArgs e)
        {
            media.Stop();
            media.Source = null;
            resultDetails2.Text = "";
            SwitchTo(CortanaStatus.Calm);
        }

        private async void Window_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartRecordUserVoice();
        }

        private async void StartRecordUserVoice()
        {
            Cleanup();
            if (waveIn == null)
            {
                CreateWaveInDevice();
            }
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "result.wav");
            //if (File.Exists(path))
            //{
            //    File.Delete(path);
            //}
            writer = new WaveFileWriter(path, waveIn.WaveFormat);
            waveIn.StartRecording();
        }

        private async void Window_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ProcessUserVoice();
        }

        private async void ProcessUserVoice()
        {
            if (waveIn != null) waveIn.StopRecording();
            //send to server
            string tempWav = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "result.wav");

            var task = SpeechUtil.GetResultAsync(tempWav);
            SRResult res = await task;
            if (res != null && res.IsSuccess)
            {
                SwitchTo(CortanaStatus.Thinking);
                string result = res.Best;
                result = PreprocessingText(result);

                if (string.IsNullOrEmpty(result))
                {
                    SwitchTo(CortanaStatus.Speaking);
                    string ack = "对不起，我没听懂您的意思";
                    bool flag = await WaveGenerate(ack);
                    await pronounce(ack);
                }
                else
                {
                    var nlpTask = ins.ProcessAsync(result);
                    Result ret = await nlpTask;
                    string s = ret.Msg;
                    if (string.IsNullOrEmpty(s))
                    {
                        s = "好的";
                    }
                    string ack = resultProcess(s);
                    bool flag = await WaveGenerate(ack);
                    SwitchTo(CortanaStatus.Speaking);
                    await pronounce(ack);
                }
                

            }
            else
            {
                SwitchTo(CortanaStatus.Speaking);
                string ack = "对不起，我没听懂您的意思";
                bool flag = await WaveGenerate(ack);
                await pronounce(ack);
            }
        }

        private string PreprocessingText(string result)
        {
            result = result.Trim(new char[] { '.', '。','!',',','，'});
            return result;
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !isRecording)
            {
                isRecording = true;
                StartRecordUserVoice();

            }
        }

        private async void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ProcessUserVoice();
                isRecording = false;
            }
        }
    }
}
