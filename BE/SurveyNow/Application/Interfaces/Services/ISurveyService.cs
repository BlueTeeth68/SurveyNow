﻿using Application.DTOs.Request.Survey;
using Application.DTOs.Response;
using Application.DTOs.Response.Survey;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface ISurveyService
{
    Task<long> CreateSurveyAsync(SurveyRequest request);
    Task<SurveyDetailResponse> GetByIdAsync(long id);
    Task<List<SurveyResponse>> GetAllAsync();

    Task<PagingResponse<SurveyResponse>> FilterSurveyAsync(
        string? status,
        bool? isDelete,
        string? packType,
        string? title,
        string? sortTitle,
        string? sortCreatedDate,
        string? sortStartDate,
        string? sortExpiredDate,
        string? sortModifiedDate,
        int? page,
        int? size);
}