using Google.Protobuf.Collections;
using Grpc.Core;
using IdentityProto;
using IdentityService.Application.ApplicationUsers.Commands;
using IdentityService.Application.ApplicationUsers.Queries;
using MediatR;
namespace IdentityService.API.GrpcServices;

public class GrpcIdentityService : GrpcIdentity.GrpcIdentityBase
{
    private readonly ISender _sender;

    public GrpcIdentityService(ISender sender)
    {
        _sender = sender;
    }

    public override async Task<GrpcListSimpleAccountsDTO> GetSimpleAccounts(GrpcIdsSetRequest request, ServerCallContext context)
    {
        if (request.Ids == null || request.Ids.Count == 0)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Ids must not be null or empty."));
        }

        var accountIdSets = request.Ids.Select(Guid.Parse).ToHashSet();

        var response = await _sender.Send(new GetSimpleAccountsByIdSetQuery
        {
            Ids = accountIdSets,
        });

        response.ThrowIfFailure();

        var users = response.Value!;

        var mapField = new MapField<string, GrpcSimpleAccountDTO>();
        foreach (var user in users)
        {
            mapField.Add(user.Id.ToString(), new GrpcSimpleAccountDTO
            {
                Id = user.Id.ToString() ?? "",
                Email = user.Email ?? "",
                Username = user.Username ?? "",
                Role = user.Role ?? "",
            });
        }

        var grpcResult = new GrpcListSimpleAccountsDTO
        {
            Accounts = { mapField }
        };
        return grpcResult;
    }
    public override async Task<GrpcAccountDetailDTO> GetAccountDetail(GrpcIdRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id must not be null or empty."));
        }

        var response = await _sender.Send(new GetAccountDetailQuery
        {
            Id = Guid.Parse(request.Id),
        });
        response.ThrowIfFailure();

        return new GrpcAccountDetailDTO
        {
            Id = response.Value!.Id.ToString() ?? "",
            Email = response.Value.Email ?? "",
            Username = response.Value.Username ?? "",
            Role = response.Value.Role ?? "",
        };
    }

    public override async Task<GrpcStatusResponse> UpdateAccount(GrpcUpdateAccountRequest request, ServerCallContext context)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, "Id must not be null or empty."));
        }

        var response = await _sender.Send(new UpdateAccountCommand
        {
            Id = Guid.Parse(request.Id),
            Email = request.Email,
            Username = request.Username,
            IsActive = request.IsActive,
        });
        response.ThrowIfFailure();
        return new GrpcStatusResponse{ IsSuccess = true};
    }
}
