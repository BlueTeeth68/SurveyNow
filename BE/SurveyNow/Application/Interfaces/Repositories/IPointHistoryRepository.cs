﻿using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPointHistoryRepository: IBaseRepository<PointHistory>
{
    Task<PointHistory> GetPointPurchaseDetailAsync(long id);
}