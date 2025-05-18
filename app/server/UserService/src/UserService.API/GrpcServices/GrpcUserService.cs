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
                Address = user.Address ?? "",
                FullName = user.FullName ?? "",
                IsActive = user.IsActive,
                IsAdmin = user.Role == "Admin",
            });
        }

        return new GrpcGetSimpleUsersDTO
        {
            Users = { mapField }
        };
    }

    public override async Task<GrpcUserDetailDTO> GetUserDetailById(GrpcIdRequest request, ServerCallContext context)
    {
        if(request.Id == null)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id must not be null."));
        }

        var response = await _sender.Send(new GetUserByIdQuery
        {
            Id = Guid.Parse(request.Id)
        });

        response.ThrowIfFailure();

        return new GrpcUserDetailDTO
        {
            Id = response.Value!.Id.ToString(),
            Address = response.Value.Address ?? "",
            FullName = response.Value.FullName ?? "",
            IsActive = response.Value.IsActive,
            IsAdmin = response.Value.Role == "Admin",
            Email = response.Value.Email ?? "",
            Usrname = response.Value.Username ?? "",
        };
    }
}
