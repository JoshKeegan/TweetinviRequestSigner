using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TweetinviRequestSignerClient.Exceptions
{
    [Serializable]
    public class TweetInviRequestSignerServerException : Exception
    {
        #region Public Variables

        public readonly HttpStatusCode StatusCode;
        public readonly string StatusDescription;

        #endregion

        #region Constructors

        public TweetInviRequestSignerServerException(HttpStatusCode statusCode, string statusDescription, WebException innerException = null)
            : this(
                statusCode, statusDescription, String.Format("Server Error {0}: {1}", statusCode, statusDescription),
                innerException)
        { }

        public TweetInviRequestSignerServerException(HttpStatusCode statusCode, string statusDescription, string message,
            WebException innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }

        public TweetInviRequestSignerServerException(string message)
            : this(HttpStatusCode.OK, "OK", message) { }

        #endregion
    }
}
