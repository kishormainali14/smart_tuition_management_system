using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SmartTuitionManagementSystem.Data;
using SmartTuitionManagementSystem.Entities;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services.Interface;

namespace SmartTuitionManagementSystem.Services;

public class FeeService : IFeeService
{
    private readonly ApplicationDbContext _context;

    public FeeService(ApplicationDbContext context)
    {
        _context = context;
    }

    private static DateTime NormalizeDate(DateTime date) =>
        date.Kind == DateTimeKind.Utc ? date : DateTime.SpecifyKind(date, DateTimeKind.Utc);

    private static DateTime? NormalizeDate(DateTime? date) =>
        date.HasValue ? NormalizeDate(date.Value) : date;

    // ===== Fee Types =====

    public async Task<List<FeeStructureViewModel>> GetAllFeeTypesAsync()
    {
        return await _context.FeeTypes
            .OrderBy(f => f.FeeTypeName)
            .Select(f => new FeeStructureViewModel
            {
                Id = f.Id,
                FeeTypeName = f.FeeTypeName,
                Description = f.Description,
                IsActive = f.IsActive,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<FeeStructureViewModel?> GetFeeTypeByIdAsync(int id)
    {
        var entity = await _context.FeeTypes.FindAsync(id);
        if (entity == null) return null;
        return new FeeStructureViewModel
        {
            Id = entity.Id,
            FeeTypeName = entity.FeeTypeName,
            Description = entity.Description,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }

    public async Task<(bool Success, string Message)> CreateFeeTypeAsync(string name, string? description)
    {
        try
        {
            var existing = await _context.FeeTypes
                .FirstOrDefaultAsync(f => f.FeeTypeName.ToLower() == name.ToLower());

            if (existing != null)
            {
                if (existing.IsActive)
                    return (false, "A fee type with this name already exists");

                existing.IsActive = true;
                existing.Description = description;
                existing.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return (true, "Fee type reactivated successfully");
            }

            var entity = new FeeTypeEntity
            {
                FeeTypeName = name,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };
            _context.FeeTypes.Add(entity);
            await _context.SaveChangesAsync();
            return (true, "Fee type created successfully");
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return (false, $"Error creating fee type: {detail}");
        }
    }

    public async Task<(bool Success, string Message)> UpdateFeeTypeAsync(int id, string name, string? description, bool isActive)
    {
        try
        {
            var entity = await _context.FeeTypes.FindAsync(id);
            if (entity == null)
                return (false, "Fee type not found");

            var duplicate = await _context.FeeTypes
                .AnyAsync(f => f.FeeTypeName.ToLower() == name.ToLower() && f.Id != id);

            if (duplicate)
                return (false, "Another fee type with this name already exists");

            entity.FeeTypeName = name;
            entity.Description = description;
            entity.IsActive = isActive;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (true, "Fee type updated successfully");
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return (false, $"Error updating fee type: {detail}");
        }
    }

    public async Task<(bool Success, string Message)> DeleteFeeTypeAsync(int id)
    {
        try
        {
            var entity = await _context.FeeTypes.FindAsync(id);
            if (entity == null)
                return (false, "Fee type not found");
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return (true, "Fee type deleted successfully");
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return (false, $"Error deleting fee type: {detail}");
        }
    }

    // ===== Fee Structures =====

    public async Task<List<FeeStructureViewModel>> GetAllFeeStructuresAsync(int? classId = null)
    {
        var query = _context.FeeStructures
            .Include(f => f.Class)
            .Include(f => f.FeeType)
            .AsQueryable();

        if (classId.HasValue && classId.Value > 0)
            query = query.Where(f => f.ClassId == classId.Value);

        return await query
            .OrderBy(f => f.Class!.ClassName)
            .ThenBy(f => f.FeeType!.FeeTypeName)
            .Select(f => new FeeStructureViewModel
            {
                Id = f.Id,
                ClassId = f.ClassId,
                FeeTypeId = f.FeeTypeId,
                Amount = f.Amount,
                DueDate = f.DueDate,
                Description = f.Description,
                IsActive = f.IsActive,
                ClassName = f.Class!.ClassName,
                FeeTypeName = f.FeeType!.FeeTypeName,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<FeeStructureViewModel?> GetFeeStructureByIdAsync(int id)
    {
        var entity = await _context.FeeStructures
            .Include(f => f.Class)
            .Include(f => f.FeeType)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (entity == null) return null;

        return new FeeStructureViewModel
        {
            Id = entity.Id,
            ClassId = entity.ClassId,
            FeeTypeId = entity.FeeTypeId,
            Amount = entity.Amount,
            DueDate = entity.DueDate,
            Description = entity.Description,
            IsActive = entity.IsActive,
            ClassName = entity.Class?.ClassName,
            FeeTypeName = entity.FeeType?.FeeTypeName,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<(bool Success, string Message)> CreateFeeStructureAsync(FeeStructureViewModel model)
    {
        try
        {
            var existing = await _context.FeeStructures
                .FirstOrDefaultAsync(f => f.ClassId == model.ClassId && f.FeeTypeId == model.FeeTypeId);

            if (existing != null)
            {
                if (existing.IsActive)
                    return (false, "This fee structure already exists for the selected class and fee type");

                existing.IsActive = true;
                existing.Amount = model.Amount;
                existing.DueDate = NormalizeDate(model.DueDate);
                existing.Description = model.Description;
                existing.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return (true, "Fee structure reactivated and updated successfully");
            }

            var entity = new FeeStructureEntity
            {
                ClassId = model.ClassId,
                FeeTypeId = model.FeeTypeId,
                Amount = model.Amount,
                DueDate = NormalizeDate(model.DueDate),
                Description = model.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.FeeStructures.Add(entity);
            await _context.SaveChangesAsync();
            return (true, "Fee structure created successfully");
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return (false, $"Error creating fee structure: {detail}");
        }
    }

    public async Task<(bool Success, string Message)> UpdateFeeStructureAsync(int id, FeeStructureViewModel model)
    {
        try
        {
            var entity = await _context.FeeStructures.FindAsync(id);
            if (entity == null)
                return (false, "Fee structure not found");

            var duplicate = await _context.FeeStructures
                .AnyAsync(f => f.ClassId == model.ClassId && f.FeeTypeId == model.FeeTypeId && f.Id != id);

            if (duplicate)
                return (false, "This fee structure already exists for the selected class and fee type");

            entity.ClassId = model.ClassId;
            entity.FeeTypeId = model.FeeTypeId;
            entity.Amount = model.Amount;
            entity.DueDate = NormalizeDate(model.DueDate);
            entity.Description = model.Description;
            entity.IsActive = model.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return (true, "Fee structure updated successfully");
        }
        catch (Exception ex)
        {
            var detail = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return (false, $"Error updating fee structure: {detail}");
        }
    }

    public async Task<bool> DeleteFeeStructureAsync(int id)
    {
        try
        {
            var entity = await _context.FeeStructures.FindAsync(id);
            if (entity == null) return false;
            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    // ===== Fee Collections =====

    public async Task<List<FeeCollectionViewModel>> GetFeeCollectionsAsync(int? classId = null, int? studentId = null, string? status = null)
    {
        var query = _context.FeeCollections
            .Include(f => f.Student)
            .Include(f => f.FeeStructure).ThenInclude(fs => fs!.FeeType)
            .Include(f => f.FeeStructure).ThenInclude(fs => fs!.Class)
            .AsQueryable();

        if (classId.HasValue && classId.Value > 0)
            query = query.Where(f => f.Student!.ClassId == classId.Value);

        if (studentId.HasValue && studentId.Value > 0)
            query = query.Where(f => f.StudentId == studentId.Value);

        if (!string.IsNullOrEmpty(status) && status != "All")
            query = query.Where(f => f.Status == status);

        return await query
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FeeCollectionViewModel
            {
                Id = f.Id,
                StudentId = f.StudentId,
                FeeStructureId = f.FeeStructureId,
                TotalAmount = f.TotalAmount,
                PaidAmount = f.PaidAmount,
                RemainingBalance = f.RemainingBalance,
                DueDate = f.DueDate,
                Status = f.Status,
                Remarks = f.Remarks,
                StudentName = f.Student!.FullName,
                RollNumber = f.Student.RollNumber,
                ClassName = f.FeeStructure!.Class!.ClassName,
                FeeTypeName = f.FeeStructure.FeeType!.FeeTypeName,
                ParentPhone = f.Student.ParentPhone,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            })
            .ToListAsync();
    }

    public async Task<FeeCollectionViewModel?> GetFeeCollectionByIdAsync(int id)
    {
        var entity = await _context.FeeCollections
            .Include(f => f.Student)
            .Include(f => f.FeeStructure).ThenInclude(fs => fs!.FeeType)
            .Include(f => f.FeeStructure).ThenInclude(fs => fs!.Class)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (entity == null) return null;

        return new FeeCollectionViewModel
        {
            Id = entity.Id,
            StudentId = entity.StudentId,
            FeeStructureId = entity.FeeStructureId,
            TotalAmount = entity.TotalAmount,
            PaidAmount = entity.PaidAmount,
            RemainingBalance = entity.RemainingBalance,
            DueDate = entity.DueDate,
            Status = entity.Status,
            Remarks = entity.Remarks,
            StudentName = entity.Student?.FullName,
            RollNumber = entity.Student?.RollNumber,
            ClassName = entity.FeeStructure?.Class?.ClassName,
            FeeTypeName = entity.FeeStructure?.FeeType?.FeeTypeName,
            ParentPhone = entity.Student?.ParentPhone,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public async Task<(bool Success, string Message)> GenerateFeeCollectionAsync(int studentId, int feeStructureId)
    {
        try
        {
            var exists = await _context.FeeCollections
                .AnyAsync(f => f.StudentId == studentId && f.FeeStructureId == feeStructureId && f.IsActive);

            if (exists)
                return (false, "Fee collection already exists for this student");

            var feeStructure = await _context.FeeStructures.FindAsync(feeStructureId);
            if (feeStructure == null)
                return (false, "Fee structure not found");

            var collection = new FeeCollectionEntity
            {
                StudentId = studentId,
                FeeStructureId = feeStructureId,
                TotalAmount = feeStructure.Amount,
                PaidAmount = 0,
                RemainingBalance = feeStructure.Amount,
                DueDate = feeStructure.DueDate ?? DateTime.UtcNow.AddDays(30),
                Status = "Pending",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.FeeCollections.Add(collection);
            await _context.SaveChangesAsync();
            return (true, "Fee collection generated successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Error generating fee collection: {ex.Message}");
        }
    }

    public async Task<(bool Success, string Message)> GenerateAllFeeCollectionsAsync(int classId, int feeStructureId)
    {
        try
        {
            var students = await _context.Students
                .Where(s => s.ClassId == classId && s.IsActive)
                .ToListAsync();

            var feeStructure = await _context.FeeStructures.FindAsync(feeStructureId);
            if (feeStructure == null)
                return (false, "Fee structure not found");

            var existingStudentIds = await _context.FeeCollections
                .Where(f => f.FeeStructureId == feeStructureId && f.IsActive)
                .Select(f => f.StudentId)
                .ToListAsync();

            var newStudents = students.Where(s => !existingStudentIds.Contains(s.Id)).ToList();

            if (!newStudents.Any())
                return (false, "All students already have this fee collection");

            foreach (var student in newStudents)
            {
                _context.FeeCollections.Add(new FeeCollectionEntity
                {
                    StudentId = student.Id,
                    FeeStructureId = feeStructureId,
                    TotalAmount = feeStructure.Amount,
                    PaidAmount = 0,
                    RemainingBalance = feeStructure.Amount,
                    DueDate = feeStructure.DueDate ?? DateTime.UtcNow.AddDays(30),
                    Status = "Pending",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return (true, $"Fee collections generated for {newStudents.Count} student(s)");
        }
        catch (Exception ex)
        {
            return (false, $"Error generating fee collections: {ex.Message}");
        }
    }

    // ===== Payments =====

    public async Task<(bool Success, string Message, int TransactionId)> MakePaymentAsync(FeePaymentViewModel model, int userId)
    {
        try
        {
            var collection = await _context.FeeCollections
                .FirstOrDefaultAsync(f => f.Id == model.FeeCollectionId && f.IsActive);

            if (collection == null)
                return (false, "Fee collection not found", 0);

            if (model.AmountPaid <= 0)
                return (false, "Payment amount must be greater than zero", 0);

            if (model.AmountPaid > collection.RemainingBalance)
                return (false, $"Amount cannot exceed remaining balance of {collection.RemainingBalance:C}", 0);

            var transaction = new PaymentTransactionEntity
            {
                FeeCollectionId = model.FeeCollectionId,
                StudentId = collection.StudentId,
                AmountPaid = model.AmountPaid,
                PaymentDate = NormalizeDate(model.PaymentDate),
                PaymentMethod = model.PaymentMethod,
                TransactionReference = model.TransactionReference,
                Remarks = model.Remarks,
                CreatedAt = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            collection.PaidAmount += model.AmountPaid;
            collection.RemainingBalance -= model.AmountPaid;
            collection.UpdatedAt = DateTime.UtcNow;

            if (collection.RemainingBalance <= 0)
                collection.Status = "Paid";
            else
                collection.Status = "Partial";

            await _context.SaveChangesAsync();
            return (true, "Payment recorded successfully", transaction.Id);
        }
        catch (Exception ex)
        {
            return (false, $"Error recording payment: {ex.Message}", 0);
        }
    }

    public async Task<List<PaymentTransactionViewModel>> GetPaymentHistoryAsync(
        int? feeCollectionId = null, int? studentId = null,
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.PaymentTransactions
            .Include(t => t.Student)
            .Include(t => t.FeeCollection).ThenInclude(fc => fc!.FeeStructure).ThenInclude(fs => fs!.FeeType)
            .Include(t => t.FeeCollection).ThenInclude(fc => fc!.FeeStructure).ThenInclude(fs => fs!.Class)
            .AsQueryable();

        if (feeCollectionId.HasValue)
            query = query.Where(t => t.FeeCollectionId == feeCollectionId.Value);

        if (studentId.HasValue && studentId.Value > 0)
            query = query.Where(t => t.StudentId == studentId.Value);

        if (fromDate.HasValue)
            query = query.Where(t => t.PaymentDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(t => t.PaymentDate <= toDate.Value.Date);

        return await query
            .OrderByDescending(t => t.PaymentDate)
            .Select(t => new PaymentTransactionViewModel
            {
                Id = t.Id,
                FeeCollectionId = t.FeeCollectionId,
                StudentId = t.StudentId,
                StudentName = t.Student!.FullName,
                RollNumber = t.Student.RollNumber,
                ClassName = t.FeeCollection!.FeeStructure!.Class!.ClassName,
                FeeTypeName = t.FeeCollection.FeeStructure.FeeType!.FeeTypeName,
                AmountPaid = t.AmountPaid,
                PaymentDate = t.PaymentDate,
                PaymentMethod = t.PaymentMethod,
                TransactionReference = t.TransactionReference,
                Remarks = t.Remarks,
                IsCancelled = t.IsCancelled,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<(bool Success, string Message)> CancelPaymentAsync(int transactionId, int userId)
    {
        try
        {
            var transaction = await _context.PaymentTransactions
                .Include(t => t.FeeCollection)
                .FirstOrDefaultAsync(t => t.Id == transactionId && !t.IsCancelled);

            if (transaction == null)
                return (false, "Transaction not found or already cancelled");

            var collection = transaction.FeeCollection;
            if (collection == null)
                return (false, "Associated fee collection not found");

            collection.PaidAmount -= transaction.AmountPaid;
            collection.RemainingBalance += transaction.AmountPaid;
            collection.UpdatedAt = DateTime.UtcNow;

            if (collection.PaidAmount <= 0)
                collection.Status = "Pending";
            else
                collection.Status = "Partial";

            transaction.IsCancelled = true;
            transaction.CancelledAt = DateTime.UtcNow;
            transaction.CancelledBy = userId;

            await _context.SaveChangesAsync();
            return (true, "Payment cancelled successfully");
        }
        catch (Exception ex)
        {
            return (false, $"Error cancelling payment: {ex.Message}");
        }
    }

    // ===== Reports =====

    public async Task<FeeReportViewModel> GetFeeReportAsync(
        string reportType, int? classId, int? studentId,
        DateTime? fromDate, DateTime? toDate)
    {
        var report = new FeeReportViewModel
        {
            ReportType = reportType,
            ClassId = classId,
            StudentId = studentId,
            FromDate = fromDate,
            ToDate = toDate,
            ClassList = await GetClassListAsync(),
            StudentList = await GetStudentListAsync(classId)
        };

        switch (reportType)
        {
            case "Daily":
            {
                var date = fromDate ?? DateTime.Today;
                report.Payments = await GetPaymentHistoryAsync(null, studentId, date, date);
                report.GrandTotalCollected = report.Payments.Where(p => !p.IsCancelled).Sum(p => p.AmountPaid);
                break;
            }
            case "Monthly":
            {
                var start = fromDate ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var end = toDate ?? start.AddMonths(1).AddDays(-1);
                report.Payments = await GetPaymentHistoryAsync(null, studentId, start, end);
                report.GrandTotalCollected = report.Payments.Where(p => !p.IsCancelled).Sum(p => p.AmountPaid);
                break;
            }
            case "Class":
            {
                report.Collections = await GetFeeCollectionsAsync(classId, null, null);
                report.GrandTotalCollected = report.Collections.Sum(c => c.PaidAmount);
                report.GrandTotalDue = report.Collections.Sum(c => c.TotalAmount);
                report.GrandTotalRemaining = report.Collections.Sum(c => c.RemainingBalance);
                break;
            }
            case "Student":
            {
                report.Collections = await GetFeeCollectionsAsync(classId, studentId, null);
                report.GrandTotalCollected = report.Collections.Sum(c => c.PaidAmount);
                report.GrandTotalDue = report.Collections.Sum(c => c.TotalAmount);
                report.GrandTotalRemaining = report.Collections.Sum(c => c.RemainingBalance);
                break;
            }
            case "Outstanding":
            {
                report.Collections = await GetFeeCollectionsAsync(classId, null, "Pending");
                var partial = await GetFeeCollectionsAsync(classId, null, "Partial");
                report.Collections.AddRange(partial);
                report.Collections = report.Collections.OrderByDescending(c => c.RemainingBalance).ToList();
                report.GrandTotalDue = report.Collections.Sum(c => c.RemainingBalance);
                report.GrandTotalRemaining = report.Collections.Sum(c => c.RemainingBalance);
                break;
            }
            case "PaymentHistory":
            {
                report.Payments = await GetPaymentHistoryAsync(null, studentId, fromDate, toDate);
                report.GrandTotalCollected = report.Payments.Where(p => !p.IsCancelled).Sum(p => p.AmountPaid);
                break;
            }
        }

        return report;
    }

    // ===== Dashboard =====

    public async Task<FeeDashboardViewModel> GetDashboardDataAsync()
    {
        var today = DateTime.Today;
        var now = DateTime.UtcNow;

        var totalCollected = await _context.PaymentTransactions
            .Where(t => !t.IsCancelled)
            .SumAsync(t => (decimal?)t.AmountPaid) ?? 0;

        var totalDue = await _context.FeeCollections
            .Where(f => f.IsActive)
            .SumAsync(f => (decimal?)f.RemainingBalance) ?? 0;

        var todayCollection = await _context.PaymentTransactions
            .Where(t => !t.IsCancelled && t.PaymentDate.Date == today.Date)
            .SumAsync(t => (decimal?)t.AmountPaid) ?? 0;

        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthlyCollection = await _context.PaymentTransactions
            .Where(t => !t.IsCancelled && t.PaymentDate >= monthStart && t.PaymentDate <= today)
            .SumAsync(t => (decimal?)t.AmountPaid) ?? 0;

        var studentsWithDue = await _context.FeeCollections
            .CountAsync(f => f.IsActive && f.RemainingBalance > 0);

        var overdueCount = await _context.FeeCollections
            .CountAsync(f => f.IsActive && f.RemainingBalance > 0 && f.DueDate < today);

        var recentPayments = await _context.PaymentTransactions
            .Include(t => t.Student)
            .Where(t => !t.IsCancelled)
            .OrderByDescending(t => t.CreatedAt)
            .Take(5)
            .Select(t => new PaymentTransactionViewModel
            {
                Id = t.Id,
                StudentName = t.Student!.FullName,
                AmountPaid = t.AmountPaid,
                PaymentDate = t.PaymentDate,
                PaymentMethod = t.PaymentMethod
            })
            .ToListAsync();

        return new FeeDashboardViewModel
        {
            TotalCollected = totalCollected,
            TotalDue = totalDue,
            TodayCollection = todayCollection,
            MonthlyCollection = monthlyCollection,
            TotalStudentsWithDue = studentsWithDue,
            OverdueCount = overdueCount,
            RecentPayments = recentPayments
        };
    }

    // ===== Receipt =====

    public async Task<FeeReceiptViewModel?> GetReceiptAsync(int transactionId)
    {
        var transaction = await _context.PaymentTransactions
            .Include(t => t.Student)
            .Include(t => t.FeeCollection).ThenInclude(fc => fc!.FeeStructure).ThenInclude(fs => fs!.FeeType)
            .Include(t => t.FeeCollection).ThenInclude(fc => fc!.FeeStructure).ThenInclude(fs => fs!.Class)
            .FirstOrDefaultAsync(t => t.Id == transactionId);

        if (transaction == null) return null;

        return new FeeReceiptViewModel
        {
            TransactionId = transaction.Id,
            ReceiptNumber = $"RCP-{transaction.Id:D6}",
            StudentName = transaction.Student?.FullName,
            RollNumber = transaction.Student?.RollNumber,
            ClassName = transaction.FeeCollection?.FeeStructure?.Class?.ClassName,
            ParentName = transaction.Student?.ParentName,
            ParentPhone = transaction.Student?.ParentPhone,
            FeeTypeName = transaction.FeeCollection?.FeeStructure?.FeeType?.FeeTypeName,
            AmountPaid = transaction.AmountPaid,
            TotalAmount = transaction.FeeCollection?.TotalAmount ?? 0,
            RemainingBalance = transaction.FeeCollection?.RemainingBalance ?? 0,
            PaymentMethod = transaction.PaymentMethod,
            TransactionReference = transaction.TransactionReference,
            PaymentDate = transaction.PaymentDate,
            GeneratedAt = DateTime.UtcNow
        };
    }

    // ===== Lookups =====

    public async Task<List<SelectListItem>> GetClassListAsync()
    {
        return await _context.Classes
            .Where(c => c.IsActive)
            .OrderBy(c => c.ClassName)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.ClassName + (string.IsNullOrEmpty(c.Section) ? "" : $" - {c.Section}")
            })
            .ToListAsync();
    }

    public async Task<List<SelectListItem>> GetFeeTypeListAsync()
    {
        return await _context.FeeTypes
            .Where(f => f.IsActive)
            .OrderBy(f => f.FeeTypeName)
            .Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.FeeTypeName
            })
            .ToListAsync();
    }

    public async Task<List<SelectListItem>> GetStudentListAsync(int? classId = null)
    {
        var query = _context.Students.Where(s => s.IsActive).AsQueryable();

        if (classId.HasValue && classId.Value > 0)
            query = query.Where(s => s.ClassId == classId.Value);

        return await query
            .OrderBy(s => s.FullName)
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.FullName} ({s.RollNumber})"
            })
            .ToListAsync();
    }

    public async Task<List<SelectListItem>> GetFeeStructureListAsync(int? classId = null)
    {
        var query = _context.FeeStructures
            .Include(f => f.FeeType)
            .Where(f => f.IsActive)
            .AsQueryable();

        if (classId.HasValue && classId.Value > 0)
            query = query.Where(f => f.ClassId == classId.Value);

        return await query
            .Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.FeeType!.FeeTypeName + $" ({f.Amount:C})"
            })
            .ToListAsync();
    }
}
