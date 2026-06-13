using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SmartTuitionManagementSystem.Models;
using SmartTuitionManagementSystem.Services.Interface;

namespace SmartTuitionManagementSystem.Controllers;

public class FeesController : Controller
{
    private readonly ILogger<FeesController> _logger;
    private readonly IFeeService _feeService;

    public FeesController(ILogger<FeesController> logger, IFeeService feeService)
    {
        _logger = logger;
        _feeService = feeService;
    }

    private bool IsUserAuthenticated()
    {
        return HttpContext.Session.GetString("UserId") != null;
    }

    private IActionResult RedirectToLogin()
    {
        TempData["ErrorMessage"] = "Please login to access this page";
        return RedirectToAction("Login", "Account");
    }

    // ===== Fee Management (Dashboard + CRUD) =====

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        try
        {
            var dashboard = await _feeService.GetDashboardDataAsync();
            var feeStructures = await _feeService.GetAllFeeStructuresAsync();
            var feeTypes = await _feeService.GetAllFeeTypesAsync();

            ViewBag.Dashboard = dashboard;
            ViewBag.FeeTypes = feeTypes;
            ViewBag.ClassList = await _feeService.GetClassListAsync();
            ViewBag.FeeTypeList = await _feeService.GetFeeTypeListAsync();

            return View(feeStructures);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Fee Management page");
            ViewBag.PageError = "An error occurred while loading the page";
            return View(new List<FeeStructureViewModel>());
        }
    }

    // ===== Fee Structure CRUD =====

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        var model = new FeeStructureViewModel
        {
            ClassList = await _feeService.GetClassListAsync(),
            FeeTypeList = await _feeService.GetFeeTypeListAsync()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FeeStructureViewModel model)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        if (!ModelState.IsValid)
        {
            model.ClassList = await _feeService.GetClassListAsync();
            model.FeeTypeList = await _feeService.GetFeeTypeListAsync();
            return View(model);
        }

        var result = await _feeService.CreateFeeStructureAsync(model);
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", result.Message);
        model.ClassList = await _feeService.GetClassListAsync();
        model.FeeTypeList = await _feeService.GetFeeTypeListAsync();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        var feeStructure = await _feeService.GetFeeStructureByIdAsync(id);
        if (feeStructure == null)
        {
            TempData["ErrorMessage"] = "Fee structure not found";
            return RedirectToAction(nameof(Index));
        }

        feeStructure.ClassList = await _feeService.GetClassListAsync();
        feeStructure.FeeTypeList = await _feeService.GetFeeTypeListAsync();
        return View(feeStructure);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FeeStructureViewModel model)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        if (!ModelState.IsValid)
        {
            model.ClassList = await _feeService.GetClassListAsync();
            model.FeeTypeList = await _feeService.GetFeeTypeListAsync();
            return View(model);
        }

        var result = await _feeService.UpdateFeeStructureAsync(id, model);
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", result.Message);
        model.ClassList = await _feeService.GetClassListAsync();
        model.FeeTypeList = await _feeService.GetFeeTypeListAsync();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsUserAuthenticated())
            return Json(new { success = false, message = "Please login" });

        var result = await _feeService.DeleteFeeStructureAsync(id);
        return Json(new { success = result, message = result ? "Fee structure deleted" : "Delete failed" });
    }

    // ===== Fee Type CRUD (via modals from Index) =====

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateFeeType(string feeTypeName, string? description)
    {
        if (!IsUserAuthenticated())
            return Json(new { success = false, message = "Please login" });

        if (string.IsNullOrWhiteSpace(feeTypeName))
            return Json(new { success = false, message = "Fee type name is required" });

        var result = await _feeService.CreateFeeTypeAsync(feeTypeName, description);
        return Json(new { success = result.Success, message = result.Message });
    }

    // ===== Collections =====

    [HttpGet]
    public async Task<IActionResult> Collections(int? classId, int? studentId, string? status)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        try
        {
            var collections = await _feeService.GetFeeCollectionsAsync(classId, studentId, status);
            ViewBag.ClassList = await _feeService.GetClassListAsync();
            ViewBag.StudentList = await _feeService.GetStudentListAsync(classId);
            ViewBag.SelectedClassId = classId;
            ViewBag.SelectedStudentId = studentId;
            ViewBag.SelectedStatus = status;

            return View(collections);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Collections page");
            TempData["ErrorMessage"] = "An error occurred";
            return View(new List<FeeCollectionViewModel>());
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateCollection(int studentId, int feeStructureId)
    {
        if (!IsUserAuthenticated())
            return Json(new { success = false, message = "Please login" });

        var result = await _feeService.GenerateFeeCollectionAsync(studentId, feeStructureId);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateBulkCollection(int classId, int feeStructureId)
    {
        if (!IsUserAuthenticated())
            return Json(new { success = false, message = "Please login" });

        var result = await _feeService.GenerateAllFeeCollectionsAsync(classId, feeStructureId);
        return Json(new { success = result.Success, message = result.Message });
    }

    [HttpGet]
    public async Task<IActionResult> Payment(int collectionId)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        var collection = await _feeService.GetFeeCollectionByIdAsync(collectionId);
        if (collection == null)
        {
            TempData["ErrorMessage"] = "Fee collection not found";
            return RedirectToAction(nameof(Collections));
        }

        var model = new FeePaymentViewModel
        {
            FeeCollectionId = collection.Id,
            StudentId = collection.StudentId,
            RemainingBalance = collection.RemainingBalance,
            PaymentDate = DateTime.Now
        };

        ViewBag.Collection = collection;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Payment(FeePaymentViewModel model)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");

        if (!ModelState.IsValid)
        {
            var collection = await _feeService.GetFeeCollectionByIdAsync(model.FeeCollectionId);
            ViewBag.Collection = collection;
            return View(model);
        }

        var result = await _feeService.MakePaymentAsync(model, userId);
        if (result.Success)
        {
            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(Receipt), new { transactionId = result.TransactionId });
        }

        ModelState.AddModelError("", result.Message);
        var coll = await _feeService.GetFeeCollectionByIdAsync(model.FeeCollectionId);
        ViewBag.Collection = coll;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Receipt(int transactionId)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        var receipt = await _feeService.GetReceiptAsync(transactionId);
        if (receipt == null)
        {
            TempData["ErrorMessage"] = "Receipt not found";

            if (transactionId > 0)
            {
                var transactions = await _feeService.GetPaymentHistoryAsync();
                var last = transactions.OrderByDescending(t => t.Id).FirstOrDefault();
                if (last != null)
                {
                    receipt = await _feeService.GetReceiptAsync(last.Id);
                    if (receipt != null)
                        return View(receipt);
                }
            }

            return RedirectToAction(nameof(Collections));
        }

        return View(receipt);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelPayment(int transactionId)
    {
        if (!IsUserAuthenticated())
            return Json(new { success = false, message = "Please login" });

        var userId = int.Parse(HttpContext.Session.GetString("UserId") ?? "0");
        var result = await _feeService.CancelPaymentAsync(transactionId, userId);
        return Json(new { success = result.Success, message = result.Message });
    }

    // ===== Reports =====

    [HttpGet]
    public async Task<IActionResult> Reports(string reportType = "Class", int? classId = null, int? studentId = null,
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        if (!IsUserAuthenticated())
            return RedirectToLogin();

        try
        {
            var report = await _feeService.GetFeeReportAsync(reportType, classId, studentId, fromDate, toDate);
            return View(report);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading Reports page");
            TempData["ErrorMessage"] = "An error occurred";
            return View(new FeeReportViewModel
            {
                ClassList = await _feeService.GetClassListAsync(),
                StudentList = await _feeService.GetStudentListAsync()
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsByClass(int classId)
    {
        var students = await _feeService.GetStudentListAsync(classId);
        return Json(students);
    }

    [HttpGet]
    public async Task<IActionResult> GetFeeStructuresByClass(int classId)
    {
        var structures = await _feeService.GetFeeStructureListAsync(classId);
        return Json(structures);
    }
}
