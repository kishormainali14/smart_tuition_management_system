using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Models;

namespace SmartTuitionManagementSystem.Services.Interface;

public interface IFeeService
{
    // Fee Types
    Task<List<FeeStructureViewModel>> GetAllFeeTypesAsync();
    Task<FeeStructureViewModel?> GetFeeTypeByIdAsync(int id);
    Task<(bool Success, string Message)> CreateFeeTypeAsync(string name, string? description);
    Task<(bool Success, string Message)> UpdateFeeTypeAsync(int id, string name, string? description, bool isActive);
    Task<(bool Success, string Message)> DeleteFeeTypeAsync(int id);

    // Fee Structures
    Task<List<FeeStructureViewModel>> GetAllFeeStructuresAsync(int? classId = null);
    Task<FeeStructureViewModel?> GetFeeStructureByIdAsync(int id);
    Task<(bool Success, string Message)> CreateFeeStructureAsync(FeeStructureViewModel model);
    Task<(bool Success, string Message)> UpdateFeeStructureAsync(int id, FeeStructureViewModel model);
    Task<bool> DeleteFeeStructureAsync(int id);

    // Fee Collections
    Task<List<FeeCollectionViewModel>> GetFeeCollectionsAsync(int? classId = null, int? studentId = null, string? status = null);
    Task<FeeCollectionViewModel?> GetFeeCollectionByIdAsync(int id);
    Task<(bool Success, string Message)> GenerateFeeCollectionAsync(int studentId, int feeStructureId);
    Task<(bool Success, string Message)> GenerateAllFeeCollectionsAsync(int classId, int feeStructureId);

    // Payments
    Task<(bool Success, string Message, int TransactionId)> MakePaymentAsync(FeePaymentViewModel model, int userId);
    Task<List<PaymentTransactionViewModel>> GetPaymentHistoryAsync(int? feeCollectionId = null, int? studentId = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<(bool Success, string Message)> CancelPaymentAsync(int transactionId, int userId);

    // Reports
    Task<FeeReportViewModel> GetFeeReportAsync(string reportType, int? classId, int? studentId, DateTime? fromDate, DateTime? toDate);

    // Dashboard
    Task<FeeDashboardViewModel> GetDashboardDataAsync();

    // Receipt
    Task<FeeReceiptViewModel?> GetReceiptAsync(int transactionId);

    // Lookups
    Task<List<SelectListItem>> GetClassListAsync();
    Task<List<SelectListItem>> GetFeeTypeListAsync();
    Task<List<SelectListItem>> GetStudentListAsync(int? classId = null);
    Task<List<SelectListItem>> GetFeeStructureListAsync(int? classId = null);
}
