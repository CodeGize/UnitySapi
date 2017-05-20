using System.Linq;
using System.Speech.Synthesis;
using Speech.Properties;

namespace Speech
{
    public class Speecher
    {
        private readonly SpeechSynthesizer m_speaker;

        public Speecher()
        {
            m_speaker = new SpeechSynthesizer();
            var installs = m_speaker.GetInstalledVoices();

            m_speaker.Volume = Settings.Default.SpeakVolume;
            m_speaker.Rate = Settings.Default.SpeakRate;
            var voice = Settings.Default.SpeakVoice;

            var selected = false;
            if (!string.IsNullOrEmpty(voice))
            {
                if (installs.Any(install => install.VoiceInfo.Name == voice))
                {
                    m_speaker.SelectVoice(voice);
                    selected = true;
                }
            }
            if (!selected)
            {
                foreach (var install in installs.Where(install => install.VoiceInfo.Culture.Name == Settings.Default.DefaultLang/*"en-US"*/))
                {
                    m_speaker.SelectVoice(install.VoiceInfo.Name);
                    break;
                }
            }
        }

        private Prompt m_curPrompt;
        /// <summary>
        /// 读一个文本
        /// </summary>
        /// <param name="msg"></param>
        public void Speak(string msg)
        {
            if (m_curPrompt != null)
                m_speaker.SpeakAsyncCancel(m_curPrompt);

            m_curPrompt = m_speaker.SpeakAsync(msg);
        }

        public void Pause()
        {
            m_speaker.Pause();
        }

        public void Resume()
        {
            m_speaker.Resume();
        }

        public SynthesizerState State => m_speaker.State;
    }
}