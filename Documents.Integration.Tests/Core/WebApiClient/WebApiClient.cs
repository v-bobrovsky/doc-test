using Documents.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Documents.Integration.Tests.Core
{
    /// <summary>
    /// Provides the web api client functionality
    /// </summary>
    public class WebApiHttpClient
    {
        #region Constants

        private static readonly string _sessionKey = ".AspNet.ApplicationCookie";

        #endregion

        #region Members 

        private readonly ILogger _logger = null;
        private static string _sessionValue = "";

        private string _address = "";

        #endregion

        public WebApiResponse Send<T>(WebApiRequest<T> request)
             where T : class
        {
            WebApiResponse result = new WebApiResponse();

            Uri uri = null;

            try
            {
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler
                {
                    UseCookies = true,
                    UseDefaultCredentials = true,
                    CookieContainer = cookieContainer
                };

                var client = new HttpClient(handler);

                string url = string.Empty;

                HttpResponseMessage response = null;
                
                if (request.HttpMethod == HttpMethod.Get)
                {
                    url = String.Format("http://{0}/{1}/{2}", 
                        _address, 
                        request.Controller,
                        ((request.Value != null && request.Value is String) 
                            ? (request.Value  as String)
                            : String.Empty));

                    url = url.Replace("=/", "=");
                }
                else if (request.HttpMethod == HttpMethod.Post)
                {
                    url = String.Format("http://{0}/{1}", 
                        _address, 
                        request.Controller);
                }
                else if (request.HttpMethod == HttpMethod.Put)
                {
                    url = String.Format("http://{0}/{1}", 
                        _address,
                        request.Controller);
                }
                else if (request.HttpMethod == HttpMethod.Delete)
                {
                    url = String.Format("http://{0}/{1}/{2}", 
                        _address, 
                        request.Controller, 
                        (request.Value as String));
                }

                if (!String.IsNullOrEmpty(url))
                    uri = new Uri(url);

                _logger.LogInfo(String.Format("{0} {1}\r\nParams:", 
                    request.HttpMethod.Method, url));

                if (!(request.Value is String))
                    _logger.LogInfoObject(request.Value);

                if (!String.IsNullOrEmpty(_sessionValue))
                {
                    cookieContainer.Add(uri, new Cookie(_sessionKey, _sessionValue));

                    _logger.LogInfo(String.Format("Cookies:\r\n{0}={1}",
                        _sessionKey, _sessionValue));
                }
                    
                if (request.HttpMethod == HttpMethod.Get)
                {
                    response = client
                        .GetAsync(url)
                        .Result;
                }
                else if (request.HttpMethod == HttpMethod.Post)
                {
                    response = client
                        .PostAsJsonAsync<T>(url, request.Value)
                        .Result;
                }
                else if (request.HttpMethod == HttpMethod.Put)
                {
                    response = client
                        .PutAsJsonAsync<T>(url, request.Value)
                        .Result;
                }
                else if (request.HttpMethod == HttpMethod.Delete)
                {
                    response = client
                        .DeleteAsync(url)
                        .Result;
                }

                if (response != null)
                {
                    response
                        .EnsureSuccessStatusCode();
                    
                    var serverResponse = response
                        .Content
                        .ReadAsStringAsync()
                        .Result;

                    var responseCookies = cookieContainer
                        .GetCookies(uri)
                        .Cast<Cookie>();

                    foreach (Cookie cookie in responseCookies)
                    {
                        if (cookie.Name.ToLower() == _sessionKey.ToLower())
                        {
                            _sessionValue = cookie.Value;
                            break;
                        }
                    }

                    result.Code = response
                        .StatusCode;

                    result.Response = serverResponse;

                    result.Result = true;

                    _logger.LogInfo(String.Format("Http code: {0}\r\nResponse:\r\n{1}",
                        result.Code, result.Response));
                }
            }
            catch (HttpRequestException hre)
            {
                _logger.LogError(hre);

                var msg = hre.Message;

                if (msg.Contains("404 "))
                    result.Code = HttpStatusCode
                        .NotFound;
                else if (msg.Contains("401 "))
                    result.Code = HttpStatusCode
                        .Unauthorized;
                else if (msg.Contains("403 "))
                    result.Code = HttpStatusCode
                        .Forbidden;
                else if (msg.Contains("500 "))
                    result.Code = HttpStatusCode
                        .InternalServerError;

                result.Response = msg;

                result.Result = false;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex);

                result.Response = ex
                    .ToString();
                result
                    .Result = false;
            }

            return result;
        }

        /// <summary>
        /// Clear session
        /// </summary>
        public void ClearSession()
        {
            _sessionValue = string.Empty;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="address"></param>
        public WebApiHttpClient(ILogger logger, string address)
        {
            _logger = logger;
            _address = address;
        }
    }
}