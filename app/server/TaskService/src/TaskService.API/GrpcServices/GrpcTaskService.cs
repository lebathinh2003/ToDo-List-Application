using MediatR;
using TaskProto;
namespace TaskService.API.GrpcServices;

public class GrpcTaskService : GrpcTask.GrpcTaskBase
{
    private readonly ISender _sender;

    public GrpcTaskService(ISender sender)
    {
        _sender = sender;
    }


}
