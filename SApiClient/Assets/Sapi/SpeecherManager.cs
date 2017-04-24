using UnityEngine;

public class SpeecherManager : MonoBehaviour
{
    protected Speecher Speecher;

    internal void Awake()
    {
        Speecher = GetComponent<Speecher>();
    }

    /***测试代码，可删除Start***/

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

        if (GUILayout.Button("Speak"))
        {
            Speecher.Speech("hello world");
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