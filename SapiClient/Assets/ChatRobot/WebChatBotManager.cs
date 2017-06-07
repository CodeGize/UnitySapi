using UnityEngine;

namespace Assets.ChatRobot
{
    public abstract class WebChatBotManager<T> : SpeecherManager where T : WebChatBot
    {
        private string m_req = "";
        protected T ChatBot;

        protected override void Awake()
        {
            base.Awake();
            ChatBot = GetComponent<T>();
            Speecher.OnGetRecognize = ChatBot.Require;
            ChatBot.OnGetResponse = OnGetResponse;
        }

        protected abstract void OnGetResponse(string res);

        public override void OnGUI()
        {
            base.OnGUI();

            m_req = GUILayout.TextField(m_req);
            if (GUILayout.Button("ChatBot"))
            {
                if (!string.IsNullOrEmpty(m_req))
                {
                    ChatBot.Require(m_req);
                }
            }
        }
    }
}