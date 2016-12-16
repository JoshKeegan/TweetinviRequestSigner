using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Injectinvi;
using Tweetinvi.Models;
using Tweetinvi.WebLogic;
using TweetinviRequestSignerCommon.ViewModels;
using HttpMethod = Tweetinvi.Models.HttpMethod;

namespace TweetinviRequestSignerClient
{
    public class TwitterQueryRemoteAuth : TwitterQuery
    {
        #region Constructors

        public TwitterQueryRemoteAuth(string queryURL, HttpMethod httpMethod) : base(queryURL, httpMethod)
        {
            // Get the credentials to use to sign this request
            ITwitterCredentials twitterCredentials = TwitterCredentials;
            if (twitterCredentials == null)
            {
                ICredentialsAccessor credentialsAccessor = TweetinviContainer.Resolve<ICredentialsAccessor>();
                twitterCredentials = credentialsAccessor.CurrentThreadCredentials;
            }

            // Send a request to the TweetinviRequestSigner server to sign this request
            VmRequestToSign signReq = new VmRequestToSign()
            {
                Url = queryURL,
                Method = httpMethod,
                ConsumerKey = twitterCredentials.ConsumerKey,
                ClientToken = twitterCredentials.AccessToken,
                ClientSecret = twitterCredentials.AccessTokenSecret
            };

            // TODO: Handle Exceptions through Tweetinvi?
            VmAuthorizationHeader vmAuthHeader = Requester.MakePostJson<VmAuthorizationHeader>("Request/Sign", signReq);
            AuthorizationHeader = vmAuthHeader.AuthorizationHeader;
        }

        #endregion

        #region Public Static Methods

        public static void Register()
        {
            TweetinviContainer.BeforeRegistrationComplete += (sender, arguments) =>
            {
                ITweetinviContainer container = arguments.TweetinviContainer;

                container.RegisterType<ITwitterQuery, TwitterQueryRemoteAuth>();
            };
        }

        #endregion
    }
}
