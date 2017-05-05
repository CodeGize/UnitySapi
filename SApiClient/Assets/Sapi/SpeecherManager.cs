using UnityEngine;

public class SpeecherManager : MonoBehaviour
{
    protected Speecher Speecher;

    internal void Awake()
    {
        Speecher = GetComponent<Speecher>();
    }

    /***测试代码，可删除Start***/

    private string m_msg = "Hello World";
    public virtual void OnGUI()
    {
        if (GUILayout.Button("Connect"))
        {
            StartCoroutine(Speecher.Connect());
        }
        if (GUILayout.Button("InitServer"))
        {
            StartCoroutine(Speecher.InitServer());
        }

        m_msg = GUILayout.TextField(m_msg);
        if (GUILayout.Button("Speak"))
        {
            Speecher.Speech(m_msg);
        }

        if (GUILayout.Button("Recognize Start"))
        {
            Speecher.Recognize(true);
        }
        if (GUILayout.Button("Recognize End"))
        {
            Speecher.Recognize(false);
        }
    }
}