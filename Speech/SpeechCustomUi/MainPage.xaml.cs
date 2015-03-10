//Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
//    Use of this sample source code is subject to the terms of the Microsoft license 
//    agreement under which you licensed this sample source code and is provided AS-IS.
//    If you did not accept the terms of the license agreement, you are not authorized 
//    to use this sample source code.  For the terms of the license, please see the 
//    license agreement between you and Microsoft.

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Bing.Speech;
using Windows.Media.SpeechSynthesis;
using System.Net;   //WebRequest
using System.IO;    //StreamReader
using System.Runtime.Serialization; //解析JSON
using System.Runtime.Serialization.Json;    //解析JSON
using System.Text;  //Encoding
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.ApplicationModel;
using SpeechCustomUi.NLP;
using wp7ChsToSpell;
using Microsoft.Speech.TtsService.HttpClient;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Globalization;

namespace SpeechCustomUi
{



    public sealed partial class MainPage : Page
    {
        
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }
        String[] feedBackWords = new String[] { "好的", "OK", "没问题", "遵命", "明白", "马上做", "我知道了", "这就去办" };
        SpeechRecognizer SR;
        bool cancel = true;
        bool startFlag = false;
        bool usebing = true;
        SpeechSynthesizer synthesizer;
        SpeechSynthesisStream synthesisStream;
        string botchoice = "$";
        string stopWord = "";
        Windows.Storage.Streams.IRandomAccessStream audioStream;
        
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Apply credentials from the Windows Azure Data Marketplace.
            var credentials = new SpeechAuthorizationParameters();
            credentials.ClientId = "jsbtest";
            credentials.ClientSecret = "MZ/tFWuf4wlhufCJzx3/X928PdKHhhuVNwg5RpwLiBM=";
            // Initialize the speech recognizer.
            SR = new SpeechRecognizer("zh-CN", credentials);
            //media = new MediaElement();
            //root.Children.Add(media);
            
            //建立synthesizer
            synthesizer = new SpeechSynthesizer();
            var voices = SpeechSynthesizer.AllVoices;
            for (int i = 0; i < voices.Count; i++)
            {
                if (voices[i].Language == "zh-CN")
                {
                    synthesizer.Voice = voices[i];
                    break;
                }
            }

            // Add speech recognition event handlers.
            SR.AudioCaptureStateChanged += SR_AudioCaptureStateChanged;
            SR.AudioLevelChanged += SR_AudioLevelChanged;
            SR.RecognizerResultReceived += SR_RecognizerResultReceived;
            
            //media.CurrentStateChanged += media_CurrentStateChanged;
            CancelButton.IsEnabled = false;
            startFlag = true;

            audioStream = (new MemoryStream()).AsRandomAccessStream();
        }


        
        void SR_RecognizerResultReceived(SpeechRecognizer sender,
            SpeechRecognitionResultReceivedEventArgs args)
        {
            if (args.Text == null) return;
            //IntermediateResults.Text = args.Text;
        }
        
        void SR_AudioLevelChanged(SpeechRecognizer sender,
            SpeechRecognitionAudioLevelChangedEventArgs args)
        {
            var v = args.AudioLevel;
            volumebar.Value = v;
            if (v > 0) VolumeMeter_CP.Opacity = v / 50;
            else VolumeMeter_CP.Opacity = Math.Abs((v - 50) / 100);
        }

        void SR_AudioCaptureStateChanged(SpeechRecognizer sender,
            SpeechRecognitionAudioCaptureStateChangedEventArgs args)
        {
            // Show the panel that corresponds to the current state.
            switch (args.State)
            {
                case SpeechRecognizerAudioCaptureState.Canceled:
                    
                    break;
                case SpeechRecognizerAudioCaptureState.Cancelling:
                    
                    break;
                case SpeechRecognizerAudioCaptureState.Complete:
                    
                    break;
                case SpeechRecognizerAudioCaptureState.Initializing:
                    UserContent.Text = "初始化...";
                    break;
                case SpeechRecognizerAudioCaptureState.Listening:
                    UserContent.Text = "正在接收...";
                    break;
                case SpeechRecognizerAudioCaptureState.Thinking:
                    UserContent.Text = "正在处理...";
                    break;
                default:
                    break;
            }
        }

        private void addToBlock(string content)
        {
            resultDetails.Text += content + "\n";
            resultDetails2.Text = content;
            double off = scroll.ExtentHeight;
            
            scroll.ScrollToVerticalOffset(off);
            scroll.UpdateLayout();
        }

        private string getresponse(string ans)
        {
            //NLPWebServiceDelegateClient sc= new NLPWebServiceDelegateClient();
            NLPWebServiceClient sc = new NLPWebServiceClient();
            Task<DoNLPResponse> rt = sc.DoNLPAsync(ans);
            
            string ss = rt.Result.Body.@return;
            return ss;
        }

        private async Task<bool> pronounce(string ack)
        {
            media.Stop();
            media.Source = null;
            audioStream.Dispose();
            bool flag = await WaveGenerate(ack);
            StorageFolder storageFolder = Windows.Storage.KnownFolders.MusicLibrary;
            StorageFile pcmFile = await storageFolder.GetFileAsync("sample.wav");
            audioStream = await pcmFile.OpenAsync(FileAccessMode.ReadWrite);
            media.SetSource(audioStream, "");
            media.Play();

            if (this.cortanaPanel.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                this.mediaEleListen.Stop();
                this.mediaEleListen.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.mediaEleThink.Stop();
                this.mediaEleThink.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                this.mediaEleSpeak.Visibility = Windows.UI.Xaml.Visibility.Visible;
                this.mediaEleSpeak.Play();

                Grid.SetRowSpan(this.mediaEleSpeak, 1);
            }

            //synthesisStream = await synthesizer.SynthesizeSsmlToStreamAsync(TextToSsml(ack));
            //this.media.Stop();
            //this.media.AutoPlay = true;
            //this.media.SetSource(synthesisStream, synthesisStream.ContentType);
            //this.media.Play();
            while (media.NaturalDuration.TimeSpan.TotalSeconds == 0)
            {
                await Task.Delay(TimeSpan.FromSeconds(0.001));
            }
            await Task.Delay(TimeSpan.FromSeconds(media.NaturalDuration.TimeSpan.TotalSeconds));
            return true;
        }


        private bool testWakewords(string words)
        {
            if (words == null || words.Length < 2)
                return false;
            words = words.Substring(0, 2);
            string rtn = "";
            char[] charary = words.ToCharArray();
            for (int i = 0; i < charary.Length; i++)
            {
                var ch = charary[i];
                if (ChineseHelper.IsCharChinese(ch))
                {
                    for (int j = 0; j < ChineseHelper._Allhz.Length; j++)
                    {
                        if (ChineseHelper._Allhz[j][1].IndexOf(ch) != -1)
                        {
                            rtn += ChineseHelper._Allhz[j][0];
                            break;
                        }
                    }
                }
                else
                    rtn += ch;
            }

            if (rtn=="XiaoNa")
                return true;
            else
                return false;
        }

        private async void Rcn_Spk()
        {
            // Use a try block because RecognizeSpeechToTextAsync depends on a web service.
            media.Source = null;
            
            try
            {
                // Start speech recognition.
                var result = await SR.RecognizeSpeechToTextAsync();

                if (cancel)
                {
                    SR.StopListeningAndProcessAudio();
                    SR.RequestCancelOperation();
                    SR.Dispose();
                    throw new Exception();
                }

                string words = result.Text;
                if(!string.IsNullOrEmpty(stopWord))
                {
                    words = stopWord;
                    stopWord = "";
                }

                string ans;
                string ack;
                if (usebing)
                {
                    
                    if (!testWakewords(words))
                        throw new Exception();
                    /*if (words == null || words.Length < 2 || words.Substring(0, 2) != "小娜")
                        throw new Exception();*/
                    UserContent.Text = "小娜," + words.Substring(2, words.Length - 2);
                    ans = words.Substring(2, words.Length - 2);
                }
                else
                {
                    UserContent.Text = words;
                    ans = words;
                }

                if (!ChineseHelper.IsCharChinese(ans[ans.Length - 1]))
                    ans = ans.Substring(0, ans.Length - 1);
                addToBlock("我："+ans);
                
                if (ans.Contains("进入聊天模式")) 
                { 
                    chatbutton.IsChecked = true;
                }
                else if (ans.Contains("退出聊天模式"))
                {
                    commandbutton.IsChecked = true;
                }
                else
                {
                    ans = botchoice + ans;
                    if (!usebing) ans = "C" + ans;
                    ack = getresponse(ans);
                    if (ack == "SUCCESS")
                    {
                        Random ran = new Random();
                        int r = ran.Next(0, 7);
                        ack = feedBackWords[r];
                    }
                    addToBlock("小娜："+ack);
                    await pronounce(resultProcess(ack));
                    
                }
                
                //if(ListenAction.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                if (!cancel)    
                    Rcn_Spk();
                //ListenAction.IsEnabled = true;


            }
            catch (Exception ex)
            {
                //Debug.Text = "exception";
                // If there's an exception, show it in the Complete panel.
                if (ex.GetType() != typeof(OperationCanceledException))
                {
                    //if (ListenAction.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
                    if (!cancel)    
                        Rcn_Spk();
                    //ListenAction.IsEnabled = true;
                }
            }

        }

        private async void SpeakButton_Click(object sender, RoutedEventArgs e)
        {
            //await Task.Delay(TimeSpan.FromSeconds(0.5));
            cancel = false;
            //SR.Dispose();
            //ListenAction.IsEnabled = true;
            //if (ListenAction.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
            //{
            Rcn_Spk();
            //}
            CancelButton.IsEnabled = true;
            SpeakButton.IsEnabled = false;
            SpeakButton2.IsEnabled = false;

            //if (this.cortanaPanel.Visibility == Windows.UI.Xaml.Visibility.Visible)
            //{
            StopAllMedia();
            this.mediaEleListen.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.mediaEleListen.Play();
            //}
            if (this.cortanaPanel.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                this.UserContent2.Focus(Windows.UI.Xaml.FocusState.Programmatic);
            }

            //string msg = getresponse("$" + "查询室内空气状况");
            //resultDetails.Text += msg + "\n";
        }

        
        private async void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Cancel the current speech session and return to start.
            cancel = true;
            media.Stop();
            SR.StopListeningAndProcessAudio();
            SR.RequestCancelOperation();
            SR.Dispose();
            volumebar.Value = 0;
            CancelButton.IsEnabled = false;
            SpeakButton.IsEnabled = true;
            SpeakButton2.IsEnabled = true;
            UserContent.Text = "";

            StopAllMedia();
            this.mediaEleCalm.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.mediaEleCalm.Play();
            
            //ListenAction.IsEnabled = true;
            await Task.Delay(TimeSpan.FromSeconds(0.5));
            
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
                    if (_result[i] == '℃' || _result[i]=='~')
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
                            
                            _result=_result.Remove(h,1);
                            _result.Insert(h, "零下");
                        }
                        
                    }
                }
            }
            return _result;
        }

        //生成音频
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
                            audioStream = fs.AsRandomAccessStream();
                            StorageFolder folder = Windows.Storage.KnownFolders.MusicLibrary;
                            var files = await folder.GetFilesAsync();
                            StorageFile sampleFile = await folder.CreateFileAsync("sample.wav", CreationCollisionOption.ReplaceExisting);
                            byte[] buffer = new byte[fs.Length];
                            fs.Read(buffer, 0, (int)fs.Length);
                            var ibuffer = buffer.AsBuffer();
                            await FileIO.WriteBufferAsync(sampleFile, ibuffer);
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

            //--------------------------------test
            string strr = "<?xml version=\"1.0\"?>\r\n<speak version=\"1.0\" xmlns=\"http://www.w3.org/2001/10/synthesis\"\r\n"
            + "xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n"
            + "xsi:schemaLocation=\"http://www.w3.org/2001/10/synthesis\r\n"
            + "http://www.w3.org/TR/speech-synthesis/synthesis.xsd\"\r\n"
            + "xml:lang=\"zh-CN\">\r\n这里有<prosody rate=\"10%\">$45</prosody>\r\n</speak>";

            //------------------------------------test
        }

        

        /*private async void ListenModeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (startFlag)
            {
                SR.StopListeningAndProcessAudio();
                SR.RequestCancelOperation();
                SR.Dispose();
                ListenAction.IsEnabled = true;
                await Task.Delay(TimeSpan.FromSeconds(0.5)); 
                int index = (sender as ComboBox).SelectedIndex;
                if (index == 1)
                {
                    ListenAction.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
                else if (index == 0)
                {
                    ListenAction.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    if (!cancel) Rcn_Spk();
                }
            }
        }*/

        /*private void ListenAction_Click(object sender, RoutedEventArgs e)
        {
            ListenAction.IsEnabled = false;
            Rcn_Spk();
        }*/


        private void BotNameList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = (sender as ComboBox).SelectedIndex;
            if (startFlag)
            {
                if (index == 0)
                {
                    botchoice = "@";
                }
                else if (index == 1)
                {
                    botchoice = "#";
                }
            }
        }

        private void bing_Click(object sender, RoutedEventArgs e)
        {
            botchoice = "@";
            bingbrush.Opacity = 1;
            sobotbrush.Opacity = 0.5;
            turingbrush.Opacity = 0.5;
        }

        private void sobot_Click(object sender, RoutedEventArgs e)
        {
            botchoice = "#";
            bingbrush.Opacity = 0.5;
            sobotbrush.Opacity = 1;
            turingbrush.Opacity = 0.5;
        }

        private void turing_Click(object sender, RoutedEventArgs e)
        {
            botchoice = "$";
            bingbrush.Opacity = 0.5;
            sobotbrush.Opacity = 0.5;
            turingbrush.Opacity = 1;
        }


        private async void Command_Checked(object sender, RoutedEventArgs e)
        {
            if (startFlag)
            {
                usebing = true;
                addToBlock("已退出聊天模式");
                await pronounce("已退出聊天模式");
            }
        }

        private async void Chat_Checked(object sender, RoutedEventArgs e)
        {
            if (startFlag)
            {
                usebing = false;
                addToBlock("已进入聊天模式");
                await pronounce("已进入聊天模式");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CortanaButton_Click(object sender, RoutedEventArgs e)
        {
            this.detailPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.cortanaPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void StopAllMedia()
        {
            this.mediaEleListen.Pause();
            this.mediaEleThink.Pause();
            this.mediaEleSpeak.Pause();
            this.mediaEleCalm.Pause();

            this.mediaEleListen.Position = TimeSpan.FromSeconds(0);
            this.mediaEleThink.Position = TimeSpan.FromSeconds(0);
            this.mediaEleSpeak.Position = TimeSpan.FromSeconds(0);
            this.mediaEleCalm.Position = TimeSpan.FromSeconds(0);

            this.mediaEleListen.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.mediaEleSpeak.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.mediaEleThink.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            this.mediaEleCalm.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.detailPanel.Visibility = Windows.UI.Xaml.Visibility.Visible;
            this.cortanaPanel.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void UserContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.UserContent2.Text = this.UserContent.Text;
            if (this.cortanaPanel.Visibility == Windows.UI.Xaml.Visibility.Visible)
            {
                if (this.UserContent.Text.Equals("正在处理..."))
                {
                    StopAllMedia();

                    this.mediaEleThink.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    this.mediaEleThink.Play();
                }
                else if (this.UserContent.Text.Equals("正在接收..."))
                {
                    StopAllMedia();

                    this.mediaEleListen.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    this.mediaEleListen.Play();
                }
                
            }
        }

        private async void Page_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            
        }

        private async void Page_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            stopWord = "小娜停";
            //try
            //{
            //    SR.StopListeningAndProcessAudio();
            //    SR.RequestCancelOperation();
            //    SR.Dispose();

            //    UserContent.Text = "正在接收...";
            //    string words = "小娜停";
            //    string ans;
            //    string ack;
            //    if (usebing)
            //    {

            //        if (!testWakewords(words))
            //            throw new Exception();
            //        /*if (words == null || words.Length < 2 || words.Substring(0, 2) != "小娜")
            //            throw new Exception();*/
            //        UserContent.Text = "小娜," + words.Substring(2, words.Length - 2);
            //        ans = words.Substring(2, words.Length - 2);
            //    }
            //    else
            //    {
            //        UserContent.Text = words;
            //        ans = words;
            //    }

            //    if (!ChineseHelper.IsCharChinese(ans[ans.Length - 1]))
            //        ans = ans.Substring(0, ans.Length - 1);
            //    addToBlock("我：" + ans);

            //    if (ans.Contains("进入聊天模式"))
            //    {
            //        chatbutton.IsChecked = true;
            //    }
            //    else if (ans.Contains("退出聊天模式"))
            //    {
            //        commandbutton.IsChecked = true;
            //    }
            //    else
            //    {
            //        ans = botchoice + ans;
            //        if (!usebing) ans = "C" + ans;
            //        UserContent.Text = "正在处理...";
            //        ack = getresponse(ans);
            //        if (ack == "SUCCESS")
            //        {
            //            Random ran = new Random();
            //            int r = ran.Next(0, 7);
            //            ack = feedBackWords[r];
            //        }
            //        addToBlock("小娜：" + ack);
            //        await pronounce(resultProcess(ack));

            //    }

            //    Rcn_Spk();
            //}
            //catch(Exception)
            //{
            //    Rcn_Spk();
            //}
            
        }

        private void Page_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {

        }


    }

    #region
    //定义处理JSON的类
    [DataContract]
    class Result
    {
        [DataMember(Order = 0)]
        public string code { get; set; }
        [DataMember(Order = 1)]
        public string text { get; set; }
        [DataMember(Order = 2)]
        public Content[] list { get; set; }
    }

    [DataContract]
    class Content
    {
        [DataMember(Order = 0)]
        public string name { get; set; }
        [DataMember(Order = 1)]
        public string price { get; set; }
        [DataMember(Order = 2)]
        public string satisfaction { get; set; }
        [DataMember(Order = 3)]
        public string count { get; set; }
        [DataMember(Order = 4)]
        public string detailirl { get; set; }
        [DataMember(Order = 5)]
        public string icon { get; set; }
    }

    #endregion
}
