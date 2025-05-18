using System.Net;
using Contract.Common;
namespace TaskService.Domain.Errors;

public class TaskError
{
    public static Error NotFound =>
       new("TaskError.NotFound",
           StatusCode: (int)HttpStatusCode.NotFound,
           Message: "Task not found!");
    public static Error AlreadyExistTask =>
        new("TaskError.AlreadyExistTask",
            Message: "They already have the Task, abort adding addition Task",
            StatusCode: (int)HttpStatusCode.Conflict);

    public static Error TaskAlreadyInactive =>
        new("TaskError.TaskAlreadyInactive",
            Message: "This task is already inactive",
            StatusCode: (int)HttpStatusCode.BadRequest);

    public static Error TaskAlreadyActive =>
        new("TaskError.TaskAlreadyActive",
            Message: "This task is already active",
            StatusCode: (int)HttpStatusCode.BadRequest);

    public static Error NullParameters =>
        new("TaskError.NullParameters",
            Message: "Some parameter is null check the server's log for full errors",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error PermissionDenied =>
        new("TaskError.PermissionDenied",
            Message: "Permission denied.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error UpdateTaskFail =>
        new("TaskError.UpdateTaskFail",
            Message: "Update Task fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error AddTaskFail =>
        new("TaskError.AddTaskFail",
            Message: "Add Task fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error DeleteTaskFail =>
        new("TaskError.DeleteTaskFail",
            Message: "Delete Task fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error RestoreTaskFail =>
        new("TaskError.RestoreTaskFail",
            Message: "Restore Task fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);

    public static Error GetTaskFail =>
        new("TaskError.GetTaskFail",
            Message: "Get Task fail.",
            StatusCode: (int)HttpStatusCode.InternalServerError);
}
