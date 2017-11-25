using UnityEngine;

namespace Assets.ChatRobot.TuLing
{
    public class TuLingChatRobotManager : WebChatBotManager<TuLingChatRobot>
    {
        protected override void OnGetResponse(string res)
        {
            var response = JsonUtility.FromJson<TuLingResponseBase>(res);
            Speecher.Speech(response.text);
        }
    }
}