﻿namespace Application.DTOs.Response.Point
{
    public class PointPurchaseResultResponse
    {
        public required string Status { get; set; }
        public required string Message { get; set; }
        public decimal PointAmount { get; set; }
        public decimal MoneyAmount { get; set; }
        public string? TransactionId { get; set; }
        public string? EWalletTransactionId { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
