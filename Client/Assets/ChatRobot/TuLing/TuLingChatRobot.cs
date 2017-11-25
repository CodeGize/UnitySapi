using System.Collections.Generic;
using UnityEngine;

namespace Assets.ChatRobot.TuLing
{
    public class TuLingChatRobot : WebChatBot
    {
        public override string Url
        {
            get { return "http://www.tuling123.com/openapi/api"; }
        }

        public override string Key
        {
            get { return "7c4071624723cdd32213619dd69fd163"; }
        }

        protected override WWW ProcessWWW(string req)
        {
            var pf = new WWWForm();
            pf.AddField("key", Key);
            pf.AddField("info", req);
            var www = new WWW(Url, pf);
            return www;
        }
    }



    public class TuLingResponseBase : IWebChatBotResponseBase
    {
        public int code;
        public string text;

        public string Text { get { return text; } }
    }

    public class TuLingUrlResponse : TuLingResponseBase
    {
        public string url;
    }

    public class TuLingNewsResponse : TuLingUrlResponse
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
}