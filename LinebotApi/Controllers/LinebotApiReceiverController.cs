﻿using LinebotApi.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace LinebotApi.Controllers
{
    public class LinebotApiReceiverController : Controller
    {
        // GET: LinebotApiReceiver
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Receive()
        {
            JsonResult _Result = new JsonResult();

            try
            {
                MessageProcessUtility _MessageProcessUtility = new MessageProcessUtility(Request);

                if (_MessageProcessUtility.validateSignature() == false)
                    return _Result;

                HttpClient _HttpClient = new HttpClient();
                foreach (var e in _MessageProcessUtility.MessageContainer.events)
                {
                    var _Content = _MessageProcessUtility.processReplyMessage(e.replyToken, e.message.text);
                    new Thread(() => this.reply(_Content)).Start();
                }
            }
            catch (Exception exp)
            {
                string _ErrorMessage = exp.Message;
            }

            return _Result;
        }
        private void reply(StringContent content)
        {
            using (HttpClient _HttpClient = new HttpClient())
            {
                _HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "7p0/MQPVVSJughWp6FUTc49ofViK95pGhK0VdF1XgGiRa6gLieB+hAc4cLanu43vtJvc6xUCsAXKLkUz4GmCkdro5YUKrkzdDLRTyTb+r5hxXI6f5bZvgNVHl5f2Ew4sdIcMbQyc5he5dQYdAxuMLwdB04t89/1O/w1cDnyilFU=");
                _HttpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                Task<HttpResponseMessage> _Task = _HttpClient.PostAsync(@"https://api.line.me/v2/bot/message/reply", content);
                _Task.Wait();
                var _ResponseString = _Task.Result.Content.ReadAsStringAsync();
            }
        }
    }
}