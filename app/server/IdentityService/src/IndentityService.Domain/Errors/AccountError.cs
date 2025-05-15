using System.Net;
using Contract.Common;
namespace IndentityService.Domain.Errors;

public class AccountError
{
    public static Error NotFound =>
       new("AccountError.NotFound",
           StatusCode: (int)HttpStatusCode.NotFound,
           Message: "Account not found!");
    public static Error AlreadyExistAccount =>
        new("AccountError.AlreadyExistAccount",
            Message: "They already have the Account, abort adding addition Account",
            StatusCode: (int)HttpStatusCode.Conflict);

    public static Error AccountAlreadyInactive =>
        new("AccountError.AccountAlreadyInactive",
            Message: "This Account is already inactive",
            StatusCode: (int)HttpStatusCode.BadRequest);

    public static Error AccountAlreadyActive =>
        new("AccountError.AccountAlreadyActive",
            Message: "This Account is already active",
            StatusCode: (int)HttpStatusCode.BadRequest);

    public static Error NullParameters =>
        new("AccountError.NullParameters",
            Message: "Some parameter is null check the server's log for full errors",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error PermissionDenied =>
        new("AccountError.PermissionDenied",
            Message: "Permission denied.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error UpdateAccountFail =>
        new("AccountError.UpdateAccountFail",
            Message: "Update Account fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error AddAccountFail =>
        new("AccountError.AddAccountFail",
            Message: "Add Account fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error DeleteAccountFail =>
        new("AccountError.DeleteAccountFail",
            Message: "Delete Account fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error GetAccountFail =>
        new("AccountError.GetAccountFail",
            Message: "Get Account fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);
}
