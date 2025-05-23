﻿using System.Net;
using Contract.Common;
namespace UserService.Domain.Errors;

public class UserError
{
    public static Error NotFound =>
       new("UserError.NotFound",
           StatusCode: (int)HttpStatusCode.NotFound,
           Message: "User not found!");
    public static Error AlreadyExistUser =>
        new("UserError.AlreadyExistUser",
            Message: "They already have the user, abort adding addition user",
            StatusCode: (int)HttpStatusCode.Conflict);

    public static Error UserAlreadyInactive =>
        new("UserError.UserAlreadyInactive",
            Message: "This user is already inactive",
            StatusCode: (int)HttpStatusCode.BadRequest);

    public static Error UserAlreadyActive =>
        new("UserError.UserAlreadyActive",
            Message: "This user is already active",
            StatusCode: (int)HttpStatusCode.BadRequest);

    public static Error NullParameters =>
        new("UserError.NullParameters",
            Message: "Some parameter is null check the server's log for full errors",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error PermissionDenied =>
        new("UserError.PermissionDenied",
            Message: "Permission denied.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error UpdateUserFail =>
        new("UserError.UpdateUserFail",
            Message: "Update user fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error AddUserFail =>
        new("UserError.AddUserFail",
            Message: "Add user fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error DeleteUserFail =>
        new("UserError.DeleteUserFail",
            Message: "Delete user fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error RestoreUserFail =>
        new("UserError.RestoreUserFail",
            Message: "Restore user fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error GetUserFail =>
        new("UserError.GetUserFail",
            Message: "Get user fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);
}
