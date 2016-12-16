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
    internal static class Requester
    {
        // Constants
        private const string REQUEST_SIGNER_SERVER_URL = "http://localhost:50154/";

        internal static T MakePostJson<T>(string endPoint, object data)
        {
            HttpWebRequest req = createPostJsonRequest(endPoint, data);

            T responseObj = runRequest<T>(req);
            return responseObj;
        }

        // Private methods
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
            return (HttpWebRequest) WebRequest.Create(REQUEST_SIGNER_SERVER_URL + endPoint);
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
    }
}
