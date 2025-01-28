using System.Net;

namespace Questao5.Application.Helpers;

public class Result<T>
{
    public bool IsValid { get; }
    public T Data { get; }
    public string ErrorMessage { get; }
    public string ErrorType { get; }
    public HttpStatusCode StatusCode { get; }

    private Result(bool isValid, T data, string errorMessage, string errorType)
    {
        IsValid = isValid;
        Data = data;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
        StatusCode = HttpStatusCode.BadRequest;
    }

    public static Result<T> Success(T data) => new(true, data, null, null);
    public static Result<T> Failure(string errorMessage, string errorType) => new(false, default, errorMessage, errorType);
}
