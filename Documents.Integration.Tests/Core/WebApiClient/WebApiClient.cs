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
        #region Members 

        public string Adress
        {
            get;
            protected set;
        }

        #endregion

        public WebApiResponse Send<T>(WebApiRequest<T> request)
             where T : class
        {
            WebApiResponse result = new WebApiResponse();

            try
            {
                HttpClient client = new HttpClient();

                string url = string.Empty;

                HttpResponseMessage response = null;
                
                if (request.HttpMethod == HttpMethod.Get)
                {
                    url = String.Format("http://{0}/{1}/{2}", 
                        Adress, 
                        request.Controller,
                        ((request.Value != null && request.Value is String) 
                            ? (request.Value  as String)
                            : String.Empty));
                        
                        response = client
                            .GetAsync(url)
                            .Result;
                }
                else if (request.HttpMethod == HttpMethod.Post)
                {
                    url = String.Format("http://{0}/{1}", 
                        Adress, 
                        request.Controller);

                    response = client
                        .PostAsJsonAsync<T>(url, request.Value)
                        .Result;
                }
                else if (request.HttpMethod == HttpMethod.Put)
                {
                    url = String.Format("http://{0}/{1}", 
                        Adress,
                        request.Controller);

                    response = client
                        .PutAsJsonAsync<T>(url, request.Value)
                        .Result;
                }
                else if (request.HttpMethod == HttpMethod.Delete)
                {
                    url = String.Format("http://{0}/{1}/{2}", 
                        Adress, 
                        request.Controller, 
                        (request.Value as String));

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

                    result.Code = response
                        .StatusCode;

                    result.Response = serverResponse;

                    result.Result = true;
                }
            }
            catch (HttpRequestException hre)
            {
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
                result.Response = ex
                    .ToString();
                result
                    .Result = false;
            }

            return result;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="address"></param>
        public WebApiHttpClient(string address)
        {
            this.Adress = address;
        }
    }
}