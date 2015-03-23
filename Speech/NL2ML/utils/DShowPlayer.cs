using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuartzTypeLib;

namespace NL2ML.utils
{
    class DShowPlayer : IPlayer
    {
        enum MediaStatus { None, Stopped, Paused, Running };

        private MediaStatus m_CurrentStatus = MediaStatus.None;
        private FilgraphManager m_objFilterGraph;
        private IBasicAudio m_objBasicAudio;
        private IMediaControl m_objMediaControl;
        private IMediaEvent m_objMediaEvent = null;
        private IMediaEventEx m_objMediaEventEx = null;
        private static int volumeGap = 10;

        public int Play(string url)
        {
            Stop();

            m_objFilterGraph = new FilgraphManager();
            m_objFilterGraph.RenderFile(url);

            m_objBasicAudio = m_objFilterGraph as IBasicAudio;
            m_objMediaControl = m_objFilterGraph as IMediaControl;

            if (m_objBasicAudio == null || m_objMediaControl == null)
            {
                return -1;
            }
            else
            {
                m_objMediaControl.Run();
                return 0;
            }
        }

        public int Pause()
        {
            if (m_objMediaControl != null)
            {
                m_objMediaControl.Pause();
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public int Resume()
        {
            if (m_objMediaControl != null)
            {
                m_objMediaControl.Run();
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public int Stop()
        {
            CleanUp();
            return 0;
        }

        public int Mute()
        {
            if (m_objBasicAudio != null)
            {
                m_objBasicAudio.Volume = 0;
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public int SetVolume(int volume)
        {
            if (m_objBasicAudio != null)
            {
                m_objBasicAudio.Volume = volume;
                return 0;
            }
            else
            {
                return -1;
            }
        }

        private void CleanUp()
        {
            if (m_objMediaControl != null)
                m_objMediaControl.Stop();

            m_CurrentStatus = MediaStatus.Stopped;

            if (m_objMediaEventEx != null)
                m_objMediaEventEx.SetNotifyWindow(0, 0, 0);

            if (m_objMediaControl != null) m_objMediaControl = null;
            if (m_objMediaEventEx != null) m_objMediaEventEx = null;
            if (m_objMediaEvent != null) m_objMediaEvent = null;
            if (m_objBasicAudio != null) m_objBasicAudio = null;
            if (m_objFilterGraph != null) m_objFilterGraph = null;
        }
    }
}
