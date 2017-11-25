using UnityEngine;

namespace Assets.ChatRobot.Cleverbot
{
    public class CleverBotManager : WebChatBotManager<CleverBot>
    {
        protected override void OnGetResponse(string res)
        {
            var response = JsonUtility.FromJson<CleverBotResponseBase>(res);
            Speecher.Speech(response.Text);
            ChatBot.Cs = response.cs;
        }
    }
}