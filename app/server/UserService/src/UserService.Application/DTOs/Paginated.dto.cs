using Contract.Common;
using UserService.Application.DTOs;
namespace UserService.Domain.Responses;
public class PaginatedGetFullUserDTO : BasePaginatedResponse<UserDetailDTO, NumberedPaginatedMetadata>;
