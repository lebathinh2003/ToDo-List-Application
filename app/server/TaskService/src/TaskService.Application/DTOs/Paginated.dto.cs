using Contract.Common;
namespace TaskService.Application.DTOs;
public class PaginatedGetFullTaskDTO : BasePaginatedResponse<TaskDTO, NumberedPaginatedMetadata>;
