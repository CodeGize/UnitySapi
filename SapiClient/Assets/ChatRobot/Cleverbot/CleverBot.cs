using UnityEngine;

namespace Assets.ChatRobot.Cleverbot
{
    public class CleverBot : WebChatBot
    {
        public override string Url
        {
            get { return "https://www.cleverbot.com/getreply"; }
        }

        public override string Key
        {
            get { return "CC2me_S7B1Qh1diYLI9CJBZxXgQ"; }
        }

        public string Cs { get; set; }

        protected override WWW ProcessWWW(string req)
        {
            var url = string.Format("{0}?key={1}&input={2}&cs={3}", Url, Key, req, Cs);
            var www = new WWW(url);
            return www;
        }
    }

    internal class CleverBotResponseBase : IWebChatBotResponseBase
    {
        public string cs;
        public int interaction_count;
        public string input;
        public string output;
        public string conversation_id;

        public string Text
        {
            get { return output; }
        }
    }
}