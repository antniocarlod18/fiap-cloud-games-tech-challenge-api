using FiapCloudGamesTechChallenge.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FiapCloudGamesTechChallenge.Domain.Exceptions
{
    [Serializable]
    public class ResourceNotFoundException<T> : BaseException
    {
        private static string _customMessage = "Oops! {} not found or may have been removed.";
        public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.NotFound;
        public override LogLevel LogLevel { get; set; } = LogLevel.Warning;

        public ResourceNotFoundException() : base(string.Format(_customMessage, nameof(T)))
        {
        }
    }
}