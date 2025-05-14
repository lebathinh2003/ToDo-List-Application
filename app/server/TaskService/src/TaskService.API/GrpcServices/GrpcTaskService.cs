using MediatR;
using TaskProto;
namespace UserService.API.GrpcServices;

public class GrpcTaskService : GrpcTask.GrpcTaskBase
{
    private readonly ISender _sender;

    public GrpcTaskService(ISender sender)
    {
        _sender = sender;
    }


}
