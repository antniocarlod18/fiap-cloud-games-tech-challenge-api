using FiapCloudGamesTechChallenge.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Net;

namespace FiapCloudGamesTechChallenge.Domain.Exceptions;

[Serializable]
public class InvalidOrderStatusException : BaseException
{
    private static string _customMessage = "Oops! You can only complete orders that are waiting for payment. Your order is {}!";
    public override HttpStatusCode StatusCode { get; set; } = HttpStatusCode.UnprocessableContent;
    public override LogLevel LogLevel { get; set; } = LogLevel.Warning;

    public InvalidOrderStatusException(OrderStatusEnum statusEnum) : base(string.Format(_customMessage, nameof(statusEnum)))
    {
    }
}