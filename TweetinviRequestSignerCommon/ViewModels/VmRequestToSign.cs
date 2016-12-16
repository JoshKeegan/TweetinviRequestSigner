using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using HttpMethod = Tweetinvi.Models.HttpMethod;

namespace TweetinviRequestSignerCommon.ViewModels
{
    public class VmRequestToSign
    {
        public string Url { get; set; }
        public HttpMethod Method { get; set; }
        public string ConsumerKey { get; set; }
        public string ClientToken { get; set; }
        public string ClientSecret { get; set; }
    }
}
