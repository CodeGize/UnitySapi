using System.Collections;
using UnityEngine;

namespace Assets.ChatRobot
{
    public delegate void WebChatResponse(string res);

    public abstract class WebChatBot : MonoBehaviour
    {
        public abstract string Url { get; }
        public abstract string Key { get; }

        public void Require(string req)
        {
            StartCoroutine(_Require(req));
        }

        public WebChatResponse OnGetResponse;

        private IEnumerator _Require(string req)
        {
            var www = ProcessWWW(req);
            yield return www;
            if (www.error != null)
            {
                Debug.Log(www.error);
                yield break;
            }
            var text = www.text;
            Debug.Log(text);
            if (OnGetResponse != null)
                OnGetResponse.Invoke(text);
        }

        protected abstract WWW ProcessWWW(string req);
    }

    public interface IWebChatBotResponseBase
    {
        string Text { get; }
    }
}