﻿using Application.DTOs.Request;
using Application.DTOs.Request.Point;
using Application.DTOs.Response;
using Application.DTOs.Response.Momo;
using Application.DTOs.Response.Point.History;
using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Services
{
    public interface IPointService
    {
        Task<bool> AddDoSurveyPointAsync(long userId, long surveyId, decimal pointAmount);
        Task<MomoPaymentMethodResponse?> CreateMomoPurchasePointOrder(User? user, PointPurchaseRequest purchaseRequest);
        Task<BasePointHistoryResponse?> GetPointHistoryDetailAsync(long id);
        Task<PagingResponse<ShortPointHistoryResponse>?> GetPaginatedPointHistoryListAsync(long userId, PointHistoryType type, PointDateFilterRequest dateFilter, PointValueFilterRequest valueFilter, PointSortOrderRequest sortOrder, PagingRequest pagingRequest);
    }
}
