using Google.Protobuf.Collections;
using Grpc.Core;
using MediatR;
using UserProto;
using UserService.Application.Users.Queries;
namespace UserService.API.GrpcServices;

public class GrpcUserService : GrpcUser.GrpcUserBase
{
    private readonly ISender _sender;

    public GrpcUserService(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<GrpcGetSimpleUsersDTO> GetSimpleUser(GrpcIdsSetRequest request, ServerCallContext context)
    {
        if (request.Ids == null || request.Ids.Count == 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Ids must not be null or empty."));
        }

        var accountIdSets = request.Ids.Select(Guid.Parse).ToHashSet();

        var response = await _sender.Send(new GetSimpleUsersByIdSetQuery
        {
            Ids = accountIdSets,
        });

        response.ThrowIfFailure();

        var users = response.Value!;

        var mapField = new MapField<string, GrpcSimpleUser>();
        foreach (var user in users)
        {
            mapField.Add(user.Id.ToString(), new GrpcSimpleUser
            {
                Id = user.Id.ToString(),
                Address = user.Address,
                FullName = user.FullName,
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin,
            });
        }

        var grpcResult = new GrpcGetSimpleUsersDTO
        {
            Users = { mapField }
        };
        return grpcResult;
    }
}
