using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TuLingChatRobot : MonoBehaviour
{
    public string Url = "http://www.tuling123.com/openapi/api";
    public string Key = "7c4071624723cdd32213619dd69fd163";
    public delegate void GetResponse(string res);

    public GetResponse OnGetResponse;

    public void Require(string req)
    {
        StartCoroutine(_Require(req));
    }

    public IEnumerator _Require(string req)
    {
        var pf = new WWWForm();
        pf.AddField("key", Key);
        pf.AddField("info", req);
        var www = new WWW(Url, pf);
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
}

public class TuLingResponseBase
{
    public int code;
    public string text;
}

public class TuLingUrlResponse: TuLingResponseBase
{
    public string url;
}

public class TuLingNewsResponse: TuLingUrlResponse
{
    public List<NewsData> list;

    public class NewsData
    {
        public string article;
        public string source;
        public string icon;
        public string detailurl;
    }
}

public class TuLingCookBookResponse : TuLingResponseBase
{
    public List<CookBookData> list;

    public class CookBookData
    {
        public string name;
        public string info;
        public string icon;
        public string detailurl;
    }
}