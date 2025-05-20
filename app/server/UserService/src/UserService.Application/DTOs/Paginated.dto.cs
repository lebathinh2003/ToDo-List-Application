using Contract.Common;
namespace UserService.Application.DTOs;
public class PaginatedGetFullUserDTO : BasePaginatedResponse<UserDetailDTO, NumberedPaginatedMetadata>;
