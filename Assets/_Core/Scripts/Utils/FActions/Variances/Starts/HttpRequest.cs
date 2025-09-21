using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using BasePuzzle.Core.Scripts.Utils.FActions.Base;

namespace BasePuzzle.Core.Scripts.Utils.FActions.Variances.Starts
{
    using BasePuzzle.Core.Scripts.Utils.FActions.Base;

    public class HttpRequest : StartAction, IStartAction<String>
    {
        private static readonly HttpClient HttpClient = GetHttpClient();

        private static HttpClient GetHttpClient()
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.ConnectionClose = false;
            httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            return httpClient;
        }
        #region Params

        public Dictionary<string, string> Headers{get; set;}
        public string JsonBody{get; set;}
        public HttpMethod RequestType{get; set;}
        public TimeSpan Timeout{get; set;} = TimeSpan.FromSeconds(100);
        public string URL{get; set;}
        
        public Version HttpVersion{get; set;} = System.Net.HttpVersion.Version11;

        #endregion
        
        private Exception exception;
        private bool isDone;
        public override Exception Exception => exception;
        public override bool Done => isDone;
        
        public override void Invoke()
        {
            try
            {
                using (var request = new HttpRequestMessage())
                {
                    request.Method = RequestType;
                    request.RequestUri = new Uri(URL);
                    request.Version = HttpVersion;
                    if (JsonBody != null && !JsonBody.Trim().Equals(""))
                    {
                        request.Content = new StringContent(JsonBody, Encoding.UTF8, "application/json");
                    }

                    if (Headers != null)
                    {
                        using (var enumerator = Headers.GetEnumerator())
                        {
                            while (enumerator.MoveNext())
                                request.Headers.Add(enumerator.Current.Key, enumerator.Current.Value);
                        }
                    }

                    using (HttpResponseMessage response = HttpClient.SendAsync(request, new CancellationTokenSource(Timeout).Token).Result)
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            Result = response.Content.ReadAsStringAsync().Result;
                            isDone = true;
                        }
                        else
                        {
                            exception = new Exception(response.StatusCode + response.Content.ReadAsStringAsync().Result);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                exception = e;
            }
        }

        public string Result { get; private set; }

        public string InvokeAndGet()
        {
            Invoke();
            return Result;
        }
    }
}

