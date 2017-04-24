using UnityEngine;

public class TuLingChatRobotManager : SpeecherManager
{
    public override void OnGUI()
    {
       base.OnGUI();

        if (GUILayout.Button("Tuling"))
        {
            var robot = GetComponent<TuLingChatRobot>();
            Speecher.OnGetRecognize = robot.Require;
            robot.OnGetResponse = OnGetResponse;
        }
    }

    private void OnGetResponse(string res)
    {
        var response = JsonUtility.FromJson<TuLingResponseBase>(res);
        Speecher.Speech(response.text);
    }
}