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

        public void Speak(string msg)
        {
            m_speaker.Speak(msg);
        }
    }
}