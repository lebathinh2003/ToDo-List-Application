using Contract.Common;
using TaskService.Application.DTOs;
namespace TaskService.Domain.Responses;
public class PaginatedGetFullTaskDTO : BasePaginatedResponse<TaskDTO, NumberedPaginatedMetadata>;
