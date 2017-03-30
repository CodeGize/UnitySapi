using System;
using System.Globalization;
using System.Speech.Recognition;
using Speech.Properties;

namespace Speech
{
    public delegate void SpeechRecognized(string text);

    public class Recognizer
    {
        private readonly SpeechRecognitionEngine m_recognizer;//语音识别引擎  
        private readonly DictationGrammar m_grammar; //自然语法  

        public Recognizer()
        {
            var myCIintl = new CultureInfo("en-US");
            var rs = SpeechRecognitionEngine.InstalledRecognizers();
            if (rs.Count > 0)
            {
                foreach (var config in rs)//获取所有语音引擎  
                {
                    if (config.Culture.Equals(myCIintl) && config.Id == "MS-1033-80-DESK")
                    {
                        m_recognizer = new SpeechRecognitionEngine(config);
                        break;
                    }//选择美国英语的识别引擎  
                }
                if (m_recognizer == null)//如果没有适合的语音引擎，则选用第一个
                    m_recognizer = new SpeechRecognitionEngine(rs[0]);
            }
            if (m_recognizer != null)
            {
                var kws = Settings.Default.Keywords;
                var fg = new string[kws.Count];
                kws.CopyTo(fg, 0);
                InitializeSpeechRecognitionEngine(fg);//初始化语音识别引擎  
                m_grammar = new DictationGrammar();
            }
            else
            {
                Console.WriteLine("创建语音识别失败");
            }
        }

        private void InitializeSpeechRecognitionEngine(string[] fg)
        {
            m_recognizer.SetInputToDefaultAudioDevice();//选择默认的音频输入设备  
            var customGrammar = CreateCustomGrammar(fg);
            //根据关键字数组建立语法  
            m_recognizer.UnloadAllGrammars();
            m_recognizer.LoadGrammar(customGrammar);
            //加载语法  
            m_recognizer.SpeechRecognized += recognizer_SpeechRecognized;
            //m_recognizer.SpeechHypothesized += recognizer_SpeechHypothesized;  
        }

        public SpeechRecognized OnRecognized;

        private void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Console.WriteLine("SpeechRecognized:" + e.Result.Text);
            OnRecognized?.Invoke(e.Result.Text);
        }

        private void recognizer_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {

        }

        /// <summary>
        /// 录音并识别
        /// </summary>
        public void BeginRec()
        {
            Console.WriteLine("BeginRec");
            TurnSpeechRecognitionOn();
            TurnDictationOn();
        }

        private void TurnDictationOn()
        {
            if (m_recognizer != null)
            {
                m_recognizer.LoadGrammar(m_grammar);
                //加载自然语法  
            }
            else
            {
                Console.WriteLine("创建语音识别失败");
            }
        }

        private void TurnSpeechRecognitionOn()//启动语音识别函数  
        {
            if (m_recognizer != null)
            {
                m_recognizer.RecognizeAsync(RecognizeMode.Multiple);
                //识别模式为连续识别  
            }
            else
            {
                Console.WriteLine("创建语音识别失败");
            }
        }

        public void EndRec()//停止语音识别引擎  
        {
            Console.WriteLine("EndRec");
            TurnSpeechRecognitionOff();
        }

        private void TurnSpeechRecognitionOff()//关闭语音识别函数  
        {
            if (m_recognizer != null)
            {
                m_recognizer.RecognizeAsyncStop();
                TurnDictationOff();
            }
            else
            {
                Console.WriteLine("创建语音识别失败");
            }
        }

        private void TurnDictationOff()
        {
            if (m_grammar != null)
            {
                m_recognizer.UnloadGrammar(m_grammar);
                //卸载自然语法  
            }
            else
            {
                Console.WriteLine("创建语音识别失败");
            }
        }

        private Grammar CreateCustomGrammar(string[] fg) //创造自定义语法  
        {
            var grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(new Choices(fg));
            return new Grammar(grammarBuilder);
        }
    }
}