﻿using Contract.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;

namespace Contract.Extension;

public static class ErrorValidationExtension
{
    public static IServiceCollection AddErrorValidation(this IServiceCollection services)
    {
        services.PostConfigure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = (context) =>
            {
                var errors = context.ModelState
                                    .Where(x => (x.Value?.Errors ?? []).Count > 0)
                                    .Select(x => new
                                    {
                                        Field = x.Key,
                                        Errors = (x.Value?.Errors ?? []).Select(e => e.ErrorMessage).ToArray()
                                    });

                var jsonErrorMessage = JsonConvert.SerializeObject(errors);

                var errorResponseDTO = new ErrorResponseDTO
                {
                    Code = "ValidationError",
                    Message = jsonErrorMessage,
                    Status = (int)HttpStatusCode.BadRequest,
                };

                var jsonResponse = JsonConvert.SerializeObject(errorResponseDTO);

                return new ContentResult
                {
                    Content = jsonResponse,
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.BadRequest,
                };
            };
        });
        return services;
    }
}
