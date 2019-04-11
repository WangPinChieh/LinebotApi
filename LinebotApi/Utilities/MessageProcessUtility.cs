using LinebotApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Web;

namespace LinebotApi.Utilities
{
    public class MessageProcessUtility
    {
        public MessageContainer MessageContainer
        {
            get
            {
                return m_MessageContainer;
            }
        }

        private MessageContainer m_MessageContainer;
        private HttpRequestBase m_Request;
        private string m_MessageSignature;
        private string m_RequestBody;

        public MessageProcessUtility(HttpRequestBase request)
        {
            m_Request = request;
            m_MessageSignature = request.Params["HTTP_X_LINE_SIGNATURE"];
            m_RequestBody = this.retriveMessageBody();
            m_MessageContainer = JsonConvert.DeserializeObject<MessageContainer>(m_RequestBody);
        }
        public bool validateSignature()
        {
            string _SecretKey = "da8364e24d575b296379539ae7a9d4c3";
            using (HMACSHA256 _HMACSHA256 = new HMACSHA256(System.Text.Encoding.UTF8.GetBytes(_SecretKey)))
            {
                byte[] _ComputedHash = _HMACSHA256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(this.m_RequestBody));
                string _ComputedSignature = Convert.ToBase64String(_ComputedHash);

                return _ComputedSignature.Equals(this.m_MessageSignature);
            }
        }
        public StringContent processReplyMessage(string replyToken, string message)
        {
            var _Message = new Dictionary<string, string> { { "type", "text" }, { "text", "Hello User, your message is: " + message } };
            List<Dictionary<string, string>> _Messages = new List<Dictionary<string, string>>();
            _Messages.Add(_Message);
            var _ReplyMessagesContainer = new Dictionary<string, object> { { "replyToken", replyToken }, { "messages", _Messages } };

            return new StringContent(JsonConvert.SerializeObject(_ReplyMessagesContainer), System.Text.Encoding.UTF8, "application/json");
        }
        private string retriveMessageBody()
        {
            string _RequestBody = string.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                this.m_Request.InputStream.CopyTo(ms);
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    _RequestBody = sr.ReadToEnd();
                }
            }
            return _RequestBody;
        }
    }
}