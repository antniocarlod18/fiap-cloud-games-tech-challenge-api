using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FiapCloudGamesTechChallenge.Domain.Exceptions
{
    public class CannotRefundOrderException : BaseException
    {
        public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.Conflict;
        public override LogLevel LogLevel { get; set; } = LogLevel.Warning;

        public CannotRefundOrderException() : base("Oops! We couldn’t process your refund — it seems the refund window has closed.")
        {
        }
    }
}
