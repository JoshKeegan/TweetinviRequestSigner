/*
 * Tweetinvi Request Signer Demo
 * Program Entry Point
 * Authors:
 *  Josh Keegan 16/12/2016
 *  
 * Usage:
 *  - Start TweetinviRequestSigner (server)
 *  - TODO (currently constant): Add Consumer Key & Secret to Server
 *  - Set constants in demo application for client credentials & application public key
 *  - Run demo
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KLog;
using Tweetinvi;
using Tweetinvi.Core.Exceptions;
using Tweetinvi.Models;
using TweetinviRequestSignerClient;

namespace TweetinviRequestSignerDemo
{
    public static class Program
    {
        #region Constants

        private const string CONSUMER_KEY = "";
        private const string CLIENT_TOKEN = "";
        private const string CLIENT_SECRET = "";

        #endregion

        #region Public Methods

        public static void Main(string[] args)
        {
            // Init KLog
            DefaultLog.Log = new ColouredConsoleLog(LogLevel.All);

            DefaultLog.Info("Tweetinvi Request Signer Demo Application");

            // Set up Remote Authorisation
            TwitterQueryRemoteAuth.Register();
            Requester.ServerUrl = "http://localhost:50154/";

            // Set up credentials as you usually would for Tweetinvi, just using null for the consumer secret
            Auth.SetUserCredentials(CONSUMER_KEY, null,
                CLIENT_TOKEN, CLIENT_SECRET);

            // Send a request to Twitter
            IEnumerable<ITweet> tweets = Search.SearchTweets("test");

            // Check response
            if (tweets == null)
            {
                DefaultLog.Error("An error occurred whilst running the search");

                IEnumerable<ITwitterException> exceptions = ExceptionHandler.GetExceptions();
                foreach (ITwitterException e in exceptions)
                {
                    DefaultLog.Error("Tweetinvi Twitter Exception:\n{0}", e);
                }
            }
            else
            {
                DefaultLog.Info("Successfully fetched {0} Tweets", tweets.Count());
            }
        }

        #endregion
    }
}
