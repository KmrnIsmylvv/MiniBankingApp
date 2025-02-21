namespace Shared.Common;

public class ResponseDTO
{
    public object? Data { get; set; }
    public string? Message { get; set; }
    public bool Success { get; set; }

    public ResponseDTO(object data, string? message = null, bool success = true)
    {
        Data = data;
        Message = message;
        Success = success;
    }

    public static ResponseDTO CreateResponse(object? data = null, string? message = null, bool? success = true) =>
        new ResponseDTO(data, message);
}