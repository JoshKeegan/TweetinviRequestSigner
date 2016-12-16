using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Tweetinvi;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;
using TweetinviRequestSignerCommon.ViewModels;

namespace TweetinviRequestSigner.Controllers
{
    /// <summary>
    /// Provides endpoints for signing Twitter requests
    /// </summary>
    public class RequestController : ApiController
    {
        // Constants
        private const string CONSUMER_KEY = "";
        private const string CONSUMER_SECRET = "";

        /// <summary>
        /// Sign a Twitter API request with the consumer secret (held server-side)
        /// </summary>
        /// <param name="req">Information from the Twitter Request required to sign it</param>
        /// <returns>Authorization Header</returns>
        [HttpPost]
        public VmAuthorizationHeader Sign([FromBody] VmRequestToSign req)
        {
            // Check that the Consumer Key in the request matches the one that we have
            if (req.ConsumerKey != CONSUMER_KEY)
            {
                throw new ArgumentException(
                    "Request must be for a Twitter Application that we have the credentials for", nameof(req));
            }

            Uri uri = new Uri(req.Url);
            TwitterCredentials twitterCredentials = new TwitterCredentials(req.ConsumerKey, CONSUMER_SECRET,
                req.ClientToken, req.ClientSecret);

            IOAuthWebRequestGenerator oAuthWebReqGenerator = TweetinviContainer.Resolve<IOAuthWebRequestGenerator>();
            IEnumerable<IOAuthQueryParameter> credentialsParams =
                oAuthWebReqGenerator.GenerateParameters(twitterCredentials);
            string authHeader = oAuthWebReqGenerator.GenerateAuthorizationHeader(uri, req.Method, credentialsParams);

            return new VmAuthorizationHeader()
            {
                AuthorizationHeader = authHeader
            };
        }
    }
}
