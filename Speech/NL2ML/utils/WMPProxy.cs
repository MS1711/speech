using log4net;
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
    public class WMPProxy : IPlayer
    {
        private static ILog logger = LogManager.GetLogger("common");

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

        public int Play(string url)
        {
            try
            {
                Stop();
                logger.Debug("playing: " + url);
                Process.Start("wmplayer", "/play /close " + url);
            }
            catch (Exception e)
            {
                logger.Debug("start mplayer error: " + e.ToString());
            }
            
            status = WMPStatus.Playing;

            return 0;
        }

        public int Resume()
        {
            if (status == WMPStatus.Playing)
            {
                return 0;
            }

            SendCommand(WMPConstants.APPCOMMAND_MEDIA_PLAY_PAUSE);
            status = WMPStatus.Playing;

            return 0;
        }

        public int Pause()
        {
            if (status == WMPStatus.Pausing)
            {
                return 0;
            }

            SendCommand(WMPConstants.APPCOMMAND_MEDIA_PLAY_PAUSE);
            status = WMPStatus.Pausing;
            return 0;
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

        public int SetVolume(int volume)
        {
            return 0;
        }

        public void VolumeDown()
        {

        }

        public int Mute()
        {
            return 0;
        }

        public int Stop()
        {
            try
            {
                Process[] ps = Process.GetProcessesByName("wmplayer");
                if (ps != null)
                {
                    foreach (Process p in ps)
                    {
                        p.Kill();
                    }
                }
            }
            catch (Exception e)
            {

            }

            status = WMPStatus.Idle;
            return 0;
        }

        internal static WMPProxy Instance
        {
            get { return WMPProxy.instance; }
        }
    }
}
