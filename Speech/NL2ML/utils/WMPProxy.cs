using NL2ML.consts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NL2ML.utils
{
    public class WMPProxy
    {
        enum WMPStatus
        {
            Invalid,
            Playing,
            Pausing,
            Idle
        }

        [DllImport("User32.dll")]
        public static extern int FindWindow(string strClassName, string strWindowName);
        [DllImport("User32.dll")]
        public static extern Int32 SendMessage(
            int hWnd,               // handle to destination window
            int Msg,                // message
            int wParam,             // first message parameter
            int lParam);            // second message parameter



        private const int WM_APPCOMMAND = 0x0319;
        private static System.Int32 mWindowHandle = 0;

        private static WMPProxy instance = new WMPProxy();
        private WMPStatus status = WMPStatus.Idle;

        private WMPProxy()
        {

        }

        public void Play(string url)
        {
            Stop();
            Process.Start("wmplayer", "/play /close " + url);
            status = WMPStatus.Playing;
        }

        public void Resume()
        {
            if (status == WMPStatus.Playing)
            {
                return;
            }

            SendCommand(WMPConstants.APPCOMMAND_MEDIA_PLAY_PAUSE);
            status = WMPStatus.Playing;
        }

        public void Pause()
        {
            if (status == WMPStatus.Pausing)
            {
                return;
            }

            SendCommand(WMPConstants.APPCOMMAND_MEDIA_PLAY_PAUSE);
            status = WMPStatus.Pausing;
        }

        private int SendCommand(int pCommand)
        {
            //get the win handle
            mWindowHandle = FindWindow("WMPlayerApp", "Windows Media Player");
            if (mWindowHandle != 0)
            {
                return SendMessage(mWindowHandle, WM_APPCOMMAND, 0x00000000, pCommand << 16);
            }

            return -1;
        }

        public void VolumeUp()
        {

        }

        public void VolumeDown()
        {

        }

        public void Mute()
        {

        }

        public void Stop()
        {
            Process[] ps = Process.GetProcessesByName("wmplayer");
            if (ps != null)
            {
                foreach (Process p in ps)
                {
                    p.Kill();
                }
            }

            status = WMPStatus.Idle;
        }

        internal static WMPProxy Instance
        {
            get { return WMPProxy.instance; }
        }

    }
}
