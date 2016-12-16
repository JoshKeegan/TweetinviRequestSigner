using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TweetinviRequestSignerClient.Exceptions;

namespace TweetinviRequestSignerClient
{
    public static class Requester
    {
        #region Settings

        public static string ServerUrl;

        [ThreadStatic]
        public static string CurrentThreadServerUrl;

        private static string threadAwareServerUrl => CurrentThreadServerUrl ?? ServerUrl;

        #endregion

        #region Internal Methods

        internal static T MakePostJson<T>(string endPoint, object data)
        {
            HttpWebRequest req = createPostJsonRequest(endPoint, data);

            T responseObj = runRequest<T>(req);
            return responseObj;
        }

        #endregion

        #region Private Methods

        private static HttpWebRequest createPostJsonRequest(string endPoint, object data)
        {
            HttpWebRequest req = createBlankRequest(endPoint);
            req.Method = "POST";

            // Convert the POST data to the raw bytes
            string jsonData = JsonConvert.SerializeObject(data);
            byte[] bytesData = Encoding.UTF8.GetBytes(jsonData);

            // Set content type & length
            req.ContentType = "application/json; charset=UTF-8";
            req.ContentLength = bytesData.Length;

            // Write the data to the stream
            using (Stream s = req.GetRequestStream())
            {
                s.Write(bytesData, 0, bytesData.Length);
            }

            return req;
        }

        private static HttpWebRequest createBlankRequest(string endPoint)
        {
            // Validation
            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            // Check that a Server URL has been set
            string serverUrl = threadAwareServerUrl;
            if (serverUrl == null)
            {
                throw new InvalidOperationException("Must set a Server URL before making a request");
            }

            // Ensure that the server URL has a trailing / before concatenating with the end point
            string epUrl = serverUrl + (serverUrl[serverUrl.Length - 1] != '/' ? "/" : "") + endPoint;

            return (HttpWebRequest) WebRequest.Create(epUrl);
        }

        private static string runRequest(HttpWebRequest req)
        {
            // Get the response
            try
            {
                using (HttpWebResponse response = (HttpWebResponse) req.GetResponse())
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new TweetInviRequestSignerServerException(response.StatusCode, response.StatusDescription);
                    }

                    StreamReader reader = new StreamReader(response.GetResponseStream());

                    string rawResponseJson = reader.ReadToEnd();
                    return rawResponseJson;
                }
            }
            catch (WebException e)
            {
                // Should this be changed out for an AlertApiException??
                HttpWebResponse webResponse = e.Response as HttpWebResponse;
                if (webResponse != null)
                {
                    // Try and read the response
                    Stream stream = webResponse.GetResponseStream();
                    string strResponseContent = null;
                    if (stream != null)
                    {
                        StreamReader reader = new StreamReader(stream);
                        strResponseContent = reader.ReadToEnd();

                        reader.Dispose();
                    }

                    throw new TweetInviRequestSignerServerException(webResponse.StatusCode, webResponse.StatusDescription,
                        strResponseContent, e);
                }
                else
                {
                    throw;
                }
            }
        }

        private static T runRequest<T>(HttpWebRequest req)
        {
            string rawResponseJson = runRequest(req);

            T responseObj = JsonConvert.DeserializeObject<T>(rawResponseJson);
            return responseObj;
        }

        #endregion
    }
}
