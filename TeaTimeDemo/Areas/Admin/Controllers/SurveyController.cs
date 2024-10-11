using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TeaTimeDemo.DataAccess.Repository.IRepository;
using TeaTimeDemo.Models;
using TeaTimeDemo.Models.ViewModels;
using TeaTimeDemo.Utility;
using System.Linq;
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Hosting; // 用來處理圖片上傳
using System.IO; // 處理文件操作
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel; // 新增此命名空間以使用 IFormFile
using AutoMapper;
using TeaTimeDemo.DTOs; // 引用 SurveyDTO 的命名空間

namespace TeaTimeDemo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Manager)] // 只有 Admin 和 Manager 角色能進入
    public class SurveyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment; // 用於處理圖片上傳的環境變數
        private readonly IImageService _imageService; // 使用 IImageService 處理圖片
        private readonly IMapper _mapper; // 注入 IMapper


        /// <summary>
        /// 建構子，注入 IUnitOfWork、IWebHostEnvironment 與 IImageService
        /// </summary>
        /// <param name="unitOfWork">單元工作介面</param>
        /// <param name="hostEnvironment">網站宿主環境</param>
        /// <param name="imageService">圖片服務介面</param>
        public SurveyController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, IImageService imageService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _imageService = imageService;
            _mapper = mapper; // 賦值
        }

        /// <summary>
        /// 問卷列表頁面
        /// </summary>
        /// <returns>問卷列表視圖</returns>
        public IActionResult Index()
        {
            return View(); // 不再傳遞 surveyList，讓 DataTables 使用 AJAX 獲取資料
        }



        /// <summary>
        /// 問卷的 Upsert 方法，用來處理創建和更新問卷 (GET)
        /// </summary>
        /// <param name="id">問卷 ID，若為 null 則為新增問卷</param>
        /// <returns>Upsert 視圖</returns>
        public IActionResult Upsert(int? id)
        {
            // 初始化 SurveyVM 並準備站別、類別、問題類型的下拉選單資料
            SurveyVM surveyVM = new()
            {
                Survey = new Survey(),
                StationList = _unitOfWork.Station.GetAll().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() }),
                CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }),
                QuestionTypeList = GetQuestionTypeList(),
                MceHtml = id != null && id > 0
                            ? _unitOfWork.Survey.Get(s => s.Id == id, includeProperties: "Questions,Questions.QuestionOptions,Category")?.MceHtml
                            : string.Empty
            };

            // 如果有傳入 ID，表示是編輯問卷
            if (id != null && id > 0)
            {
                // 從資料庫中取得指定 ID 的問卷資料，不包括圖片
                surveyVM.Survey = _unitOfWork.Survey.Get(
                    s => s.Id == id,
                    includeProperties: "Questions,Questions.QuestionOptions,Category");
                if (surveyVM.Survey == null)
                {
                    return NotFound();
                }

                // 單獨獲取圖片，確保圖片正確關聯

                // 獲取問卷圖片（只包含 SurveyId，不包含 QuestionId 和 QuestionOptionId）
                surveyVM.Survey.QuestionImages = _unitOfWork.QuestionImage.GetAll(
                    qi => qi.SurveyId == id && qi.QuestionId == null && qi.QuestionOptionId == null).ToList();

                // 獲取每個問題的圖片
                foreach (var question in surveyVM.Survey.Questions)
                {
                    question.QuestionImages = _unitOfWork.QuestionImage.GetAll(
                        qi => qi.QuestionId == question.Id && qi.QuestionOptionId == null).ToList();

                    // 獲取每個選項的圖片
                    foreach (var option in question.QuestionOptions)
                    {
                        option.QuestionImages = _unitOfWork.QuestionImage.GetAll(
                            qi => qi.QuestionOptionId == option.Id).ToList();
                    }
                }

                // 將每個問題和選項的圖片資料封裝進 ViewModel 傳遞給前端
                surveyVM.QuestionVMs = surveyVM.Survey.Questions.Select(q => new QuestionVM
                {
                    Question = q,
                    QuestionOptionVMs = q.QuestionOptions.Select(o => new QuestionOptionVM
                    {
                        QuestionOption = o,
                        QuestionOptionImages = o.QuestionImages.ToList()
                    }).ToList(),
                    QuestionImages = q.QuestionImages.ToList()
                }).ToList();
            }

            return View(surveyVM); // 返回帶有問卷資料的視圖
        }

        /// <summary>
        /// 問卷的 Upsert 方法，處理表單提交，用來新增或更新問卷資料與圖片 (POST)
        /// </summary>
        /// <param name="surveyVM">問卷的 ViewModel</param>
        /// <param name="MceHtml">TinyMCE 編輯器的 HTML 內容</param>
        /// <returns>重定向到 Index 或返回 Upsert 視圖</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(SurveyVM surveyVM, string MceHtml)
        {
            // 檢查表單提交的資料是否有效
            if (ModelState.IsValid)
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // 取得當前使用者 ID
                var currentUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == currentUserId);

                // 處理新增或編輯問卷
                if (surveyVM.Survey.Id == 0)
                {
                    // 設定問卷的建立人資訊
                    surveyVM.Survey.JobName = currentUser?.Name;  // 設定問卷的建立人名稱
                    surveyVM.Survey.JobNum = currentUser?.Address;  // 設定問卷的建立人地址
                    surveyVM.Survey.ApplicationUserId = currentUserId;// 設定建立人的 ID
                    surveyVM.Survey.CompleteTime = surveyVM.Survey.IsPublished ? DateTime.Now : null;// 設定完成時間
                    surveyVM.Survey.MceHtml = MceHtml; // 將 MceHtml 賦值給 Survey 的 MceHtml 欄位
                    _unitOfWork.Survey.Add(surveyVM.Survey);
                    _unitOfWork.Save(); // 先保存問卷，取得 ID
                    TempData["success"] = "問卷新增成功!";
                }
                else
                {
                    // 編輯問卷
                    var existingSurvey = _unitOfWork.Survey.GetFirstOrDefault(s => s.Id == surveyVM.Survey.Id);
                    if (existingSurvey != null)
                    {
                        existingSurvey.CategoryId = surveyVM.Survey.CategoryId; // 設置 CategoryId
                        existingSurvey.Title = surveyVM.Survey.Title;
                        existingSurvey.Description = surveyVM.Survey.Description;
                        existingSurvey.StationName = surveyVM.Survey.StationName;
                        existingSurvey.QuestionNum = surveyVM.Survey.QuestionNum;
                        existingSurvey.IsPublished = surveyVM.Survey.IsPublished;

                        if (surveyVM.Survey.IsPublished && existingSurvey.CompleteTime == null)
                        {
                            existingSurvey.CompleteTime = DateTime.Now;
                        }
                        else if (!surveyVM.Survey.IsPublished)
                        {
                            existingSurvey.CompleteTime = null;
                        }

                        existingSurvey.MceHtml = MceHtml; // 更新 MceHtml

                        _unitOfWork.Survey.Update(existingSurvey); // 更新操作
                        _unitOfWork.Save(); // 先更新問卷
                        TempData["success"] = "問卷編輯成功!";
                    }
                }

                // **處理問卷圖片上傳與驗證**
                if (surveyVM.SurveyImageFiles != null && surveyVM.SurveyImageFiles.Count > 0)
                {
                    foreach (var image in surveyVM.SurveyImageFiles)
                    {
                        if (image != null && image.Length > 0)
                        {
                            // 後端驗證圖片類型
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif",".webp" };
                            var extension = Path.GetExtension(image.FileName).ToLower();
                            if (!allowedExtensions.Contains(extension))
                            {
                                ModelState.AddModelError("SurveyImageFiles", "僅允許上傳 JPG、PNG 或 GIF 格式的圖片。");
                                continue; // 跳過無效的圖片
                            }

                            // 後端驗證圖片大小
                            if (image.Length > 5 * 1024 * 1024) // 5MB
                            {
                                ModelState.AddModelError("SurveyImageFiles", "圖片大小不得超過 5MB。");
                                continue; // 跳過超大圖片
                            }

                            // 使用 ImageService 儲存圖片
                            var imageUrl = _imageService.SaveImage(image, "survey"); // 儲存問卷圖片
                            if (imageUrl != null)
                            {
                                var questionImage = new QuestionImage
                                {
                                    SurveyId = surveyVM.Survey.Id, // 只設置 SurveyId
                                    ImageUrl = imageUrl
                                };
                                _unitOfWork.QuestionImage.Add(questionImage); // 新增圖片記錄
                            }
                        }
                    }
                }

                // **處理每個問題**
                foreach (var questionVM in surveyVM.QuestionVMs)
                {
                    var question = questionVM.Question;

                    // 新增或更新問題
                    if (question.Id == 0)
                    {
                        question.SurveyId = surveyVM.Survey.Id;
                        _unitOfWork.Question.Add(question);
                        _unitOfWork.Save();  // 保存以生成問題的 ID
                    }
                    else
                    {
                        _unitOfWork.Question.Update(question);
                        _unitOfWork.Save();  // 更新後保存
                    }

                    // **處理問題圖片上傳與驗證**
                    if (questionVM.QuestionImageFiles != null && questionVM.QuestionImageFiles.Count > 0)
                    {
                        foreach (var image in questionVM.QuestionImageFiles)
                        {
                            if (image != null && image.Length > 0)
                            {
                                // 後端驗證圖片類型
                                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif",".webp" };
                                var extension = Path.GetExtension(image.FileName).ToLower();
                                if (!allowedExtensions.Contains(extension))
                                {
                                    ModelState.AddModelError("QuestionImageFiles", "僅允許上傳 JPG、PNG 或 GIF 格式的圖片。");
                                    continue; // 跳過無效的圖片
                                }

                                // 後端驗證圖片大小
                                if (image.Length > 5 * 1024 * 1024) // 5MB
                                {
                                    ModelState.AddModelError("QuestionImageFiles", "圖片大小不得超過 5MB。");
                                    continue; // 跳過超大圖片
                                }

                                // 使用 ImageService 儲存圖片
                                var imageUrl = _imageService.SaveImage(image, "question"); // 儲存問題圖片
                                if (imageUrl != null)
                                {
                                    var questionImage = new QuestionImage
                                    {
                                        QuestionId = question.Id, // 只設置 QuestionId
                                        ImageUrl = imageUrl
                                    };
                                    _unitOfWork.QuestionImage.Add(questionImage); // 新增問題圖片記錄
                                }
                            }
                        }
                    }

                    // **處理每個選項及其圖片**
                    foreach (var optionVM in questionVM.QuestionOptionVMs)
                    {
                        var option = optionVM.QuestionOption;
                        option.QuestionId = question.Id;

                        if (option.Id == 0)
                        {
                            _unitOfWork.QuestionOption.Add(option); // 新增選項
                            _unitOfWork.Save(); // 保存以生成選項的 ID
                        }
                        else
                        {
                            _unitOfWork.QuestionOption.Update(option); // 更新選項
                            _unitOfWork.Save();
                        }

                        // **處理選項圖片上傳與驗證**
                        if (optionVM.OptionImageFiles != null && optionVM.OptionImageFiles.Count > 0)
                        {
                            foreach (var image in optionVM.OptionImageFiles)
                            {
                                if (image != null && image.Length > 0)
                                {
                                    // 後端驗證圖片類型
                                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif",".webp" };
                                    var extension = Path.GetExtension(image.FileName).ToLower();
                                    if (!allowedExtensions.Contains(extension))
                                    {
                                        ModelState.AddModelError("OptionImageFiles", "僅允許上傳 JPG、PNG 或 GIF 格式的圖片。");
                                        continue; // 跳過無效的圖片
                                    }

                                    // 後端驗證圖片大小
                                    if (image.Length > 5 * 1024 * 1024) // 5MB
                                    {
                                        ModelState.AddModelError("OptionImageFiles", "圖片大小不得超過 5MB。");
                                        continue; // 跳過超大圖片
                                    }

                                    // 使用 ImageService 儲存圖片
                                    var imageUrl = _imageService.SaveImage(image, "option"); // 儲存選項圖片
                                    if (imageUrl != null)
                                    {
                                        var optionImage = new QuestionImage
                                        {
                                            QuestionOptionId = option.Id, // 只設置 QuestionOptionId
                                            ImageUrl = imageUrl
                                        };
                                        _unitOfWork.QuestionImage.Add(optionImage); // 新增選項圖片記錄
                                    }
                                }
                            }
                        }
                    }
                }

                // 儲存所有更改
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index)); // 保存後重定向到問卷列表頁面
            }

            // 如果表單驗證失敗，重新加載站別與分類下拉選單數據            
            surveyVM.StationList = _unitOfWork.Station.GetAll().Select(s => new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            surveyVM.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            surveyVM.QuestionTypeList = GetQuestionTypeList();
            return View(surveyVM); // 返回帶有錯誤提示的視圖
        }


        /// <summary>
        /// 刪除圖片的功能 (AJAX)
        /// </summary>
        /// <param name="id">圖片的 ID</param>
        /// <returns>JSON 結果表示刪除是否成功</returns>
        [HttpPost]
        public IActionResult RemoveImage(int id)
        {
            var image = _unitOfWork.QuestionImage.GetFirstOrDefault(i => i.Id == id);
            if (image != null)
            {
                // 檢查圖片檔案是否存在
                var filePath = Path.Combine(_hostEnvironment.WebRootPath, image.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath))
                {
                    // 使用 ImageService 刪除圖片文件
                    if (!_imageService.DeleteImage(image.ImageUrl))
                    {
                        return Json(new { success = false, message = "圖片刪除失敗" });
                    }
                }
                else
                {
                    // 圖片檔案不存在，也刪除資料庫中的記錄
                    _unitOfWork.QuestionImage.Remove(image);
                    _unitOfWork.Save();
                    // 如果圖片文件不存在，也允許刪除資料庫中的記錄
                    return Json(new { success = true, message = "圖片刪除成功" });
                }

                // 刪除數據庫中的圖片記錄
                _unitOfWork.QuestionImage.Remove(image);
                _unitOfWork.Save();
                return Json(new { success = true, message = "圖片刪除成功" });
            }
            return Json(new { success = false, message = "圖片刪除失敗" });
        }


        /// <summary>
        /// 刪除問題的邏輯 (AJAX)
        /// </summary>
        /// <param name="id">問題的 ID</param>
        /// <returns>JSON 結果表示刪除是否成功</returns>
        [HttpPost]
        public IActionResult RemoveQuestion(int id)
        {
            var question = _unitOfWork.Question.GetFirstOrDefault(q => q.Id == id);
            if (question != null)
            {
                // 刪除問題相關的圖片
                var questionImages = _unitOfWork.QuestionImage.GetAll(qi => qi.QuestionId == id).ToList();
                foreach (var image in questionImages)
                {
                    _imageService.DeleteImage(image.ImageUrl); // 使用 ImageService 刪除圖片文件
                    _unitOfWork.QuestionImage.Remove(image); // 刪除數據庫中的圖片記錄
                }

                // 刪除問題選項及其相關圖片
                var options = _unitOfWork.QuestionOption.GetAll(o => o.QuestionId == id).ToList();
                foreach (var option in options)
                {
                    // 刪除選項相關的圖片
                    var optionImages = _unitOfWork.QuestionImage.GetAll(qi => qi.QuestionOptionId == option.Id).ToList();
                    foreach (var image in optionImages)
                    {
                        _imageService.DeleteImage(image.ImageUrl); // 使用 ImageService 刪除圖片文件
                        _unitOfWork.QuestionImage.Remove(image); // 刪除數據庫中的圖片記錄
                    }

                    _unitOfWork.QuestionOption.Remove(option); // 刪除選項
                }

                // 刪除問題
                _unitOfWork.Question.Remove(question);
                _unitOfWork.Save();
                return Json(new { success = true, message = "問題刪除成功" });
            }
            return Json(new { success = false, message = "問題刪除失敗" });
        }


        /// <summary>
        /// 刪除選項的邏輯 (AJAX)
        /// </summary>
        /// <param name="id">選項的 ID</param>
        /// <returns>JSON 結果表示刪除是否成功</returns>
        [HttpPost]
        public IActionResult RemoveOption(int id)
        {
            var option = _unitOfWork.QuestionOption.GetFirstOrDefault(o => o.Id == id);
            if (option != null)
            {
                // 刪除選項相關的圖片
                var optionImages = _unitOfWork.QuestionImage.GetAll(qi => qi.QuestionOptionId == option.Id).ToList();
                foreach (var image in optionImages)
                {
                    _imageService.DeleteImage(image.ImageUrl); // 使用 ImageService 刪除圖片文件
                    _unitOfWork.QuestionImage.Remove(image); // 刪除數據庫中的圖片記錄
                }

                _unitOfWork.QuestionOption.Remove(option); // 刪除選項
                _unitOfWork.Save(); // 保存更改
                return Json(new { success = true, message = "選項刪除成功" });
            }

            return Json(new { success = false, message = "選項刪除失敗" });
        }


        /// <summary>
        /// 動態生成問題類型的下拉選單
        /// </summary>
        /// <returns>問題類型的下拉選單列表</returns>
        private List<SelectListItem> GetQuestionTypeList()
        {
            var enumValues = Enum.GetValues(typeof(AnswerTypeEnum)).Cast<AnswerTypeEnum>();
            return enumValues.Select(e => new SelectListItem
            {
                Text = GetEnumDisplayName(e),
                Value = ((int)e).ToString()
            }).ToList();
        }

        /// <summary>
        /// 輔助方法：取得枚舉的 Display 名稱
        /// </summary>
        /// <param name="enumValue">枚舉值</param>
        /// <returns>枚舉的顯示名稱</returns>
        private string GetEnumDisplayName(Enum enumValue)
        {
            var displayAttribute = enumValue.GetType().GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? enumValue.ToString();
        }


        /// <summary>
        /// 切換問卷的發佈狀態 (AJAX)
        /// </summary>
        /// <param name="id">問卷的 ID</param>
        /// <returns>JSON 結果表示切換是否成功</returns>
        [HttpPost]
        public IActionResult TogglePublish(int id)
        {
            var survey = _unitOfWork.Survey.Get(s => s.Id == id); // 根據 ID 查詢問卷
            if (survey == null)
            {
                return Json(new { success = false, message = "問卷未找到" });
            }

            // 切換發佈狀態
            survey.IsPublished = !survey.IsPublished;

            // 如果發佈，更新完成時間；如果取消發佈，清空完成時間
            survey.CompleteTime = survey.IsPublished ? DateTime.Now : null;

            _unitOfWork.Survey.Update(survey); // 更新問卷狀態
            _unitOfWork.Save(); // 儲存更改
            return Json(new { success = true, message = "狀態已更新" });
        }


        /// <summary>
        /// 刪除問卷及其相關資料的邏輯 (AJAX)
        /// </summary>
        /// <param name="id">問卷的 ID</param>
        /// <returns>JSON 結果表示刪除是否成功</returns>
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var survey = _unitOfWork.Survey.Get(s => s.Id == id, includeProperties: "Questions.QuestionOptions.QuestionImages");
            if (survey == null)
            {
                return Json(new { success = false, message = "刪除失敗" });
            }

            // 手動刪除與 SurveyId 相關的圖片
            var surveyImages = _unitOfWork.QuestionImage.GetAll(qi => qi.SurveyId == id && qi.QuestionId == null && qi.QuestionOptionId == null).ToList();
            foreach (var image in surveyImages)
            {
                _imageService.DeleteImage(image.ImageUrl); // 使用 ImageService 刪除圖片文件
                _unitOfWork.QuestionImage.Remove(image); // 刪除數據庫中的圖片記錄
            }

            // 手動刪除問題和選項相關的圖片
            foreach (var question in survey.Questions)
            {
                // 刪除問題圖片
                foreach (var image in question.QuestionImages)
                {
                    _imageService.DeleteImage(image.ImageUrl); // 使用 ImageService 刪除圖片文件
                    _unitOfWork.QuestionImage.Remove(image); // 刪除數據庫中的圖片記錄
                }

                // 刪除每個問題選項相關的圖片
                foreach (var option in question.QuestionOptions)
                {
                    foreach (var image in option.QuestionImages)
                    {
                        _imageService.DeleteImage(image.ImageUrl); // 使用 ImageService 刪除圖片文件
                        _unitOfWork.QuestionImage.Remove(image); // 刪除數據庫中的圖片記錄
                    }
                }
            }

            // 刪除問卷及其關聯的問題
            _unitOfWork.Survey.Remove(survey);
            _unitOfWork.Save(); // 保存所有刪除操作

            return Json(new { success = true, message = "問卷及相關圖片刪除成功" });
        }

        /// <summary>
        /// 取得所有問卷資料並轉為 JSON 格式，用於前端 DataTable 顯示
        /// </summary>
        /// <returns>JSON 格式的問卷資料</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var surveyList = _unitOfWork.Survey.GetAll(includeProperties: "ApplicationUser,Category");
            var surveyDTOList = _mapper.Map<IEnumerable<SurveyDTO>>(surveyList);

            // 調試輸出
            foreach (var survey in surveyDTOList)
            {
                Console.WriteLine($"Survey ID: {survey.Id}, CategoryName: {survey.CategoryName}");
            }

            return Json(new { data = surveyDTOList });
        }



        /// <summary>
        /// 匯出所有相關資料表到 Excel，包含空表格
        /// </summary>
        /// <returns>Excel 檔案下載</returns>
        public IActionResult ExportToExcel()
        {
            var surveys = _unitOfWork.Survey.GetAll(includeProperties: "ApplicationUser,Questions.QuestionOptions,Questions.QuestionImages,QuestionImages").ToList();
            var questions = _unitOfWork.Question.GetAll().ToList();
            var questionOptions = _unitOfWork.QuestionOption.GetAll().ToList();
            var questionImages = _unitOfWork.QuestionImage.GetAll().ToList();
            const int maxCellLength = 32767;

            using (var workbook = new XLWorkbook())
            {
                // 1. Surveys 工作表
                var surveysSheet = workbook.Worksheets.Add("Surveys");
                surveysSheet.Cell(1, 1).Value = "問卷ID";
                surveysSheet.Cell(1, 2).Value = "建立人ID";
                surveysSheet.Cell(1, 3).Value = "建立人姓名";
                surveysSheet.Cell(1, 4).Value = "建立人工號";
                surveysSheet.Cell(1, 5).Value = "類別名稱";
                surveysSheet.Cell(1, 6).Value = "標題";
                surveysSheet.Cell(1, 7).Value = "描述";
                surveysSheet.Cell(1, 8).Value = "站別名稱";
                surveysSheet.Cell(1, 9).Value = "問題數量";
                surveysSheet.Cell(1, 10).Value = "是否發布";
                surveysSheet.Cell(1, 11).Value = "完成時間";
                surveysSheet.Cell(1, 12).Value = "TinyMCE HTML";
                surveysSheet.Cell(1, 13).Value = "建立時間";
                surveysSheet.Cell(1, 14).Value = "完成時間";
                surveysSheet.Cell(1, 15).Value = "備註";

                for (int i = 0; i < surveys.Count; i++)
                {
                    var survey = surveys[i];
                    string truncatedMceHtml = survey.MceHtml?.Length > maxCellLength ? survey.MceHtml.Substring(0, maxCellLength) : survey.MceHtml;

                    surveysSheet.Cell(i + 2, 1).Value = survey.Id;
                    surveysSheet.Cell(i + 2, 2).Value = survey.ApplicationUserId;
                    surveysSheet.Cell(i + 2, 3).Value = survey.ApplicationUser?.Name;
                    surveysSheet.Cell(i + 2, 4).Value = survey.JobNum;
                    surveysSheet.Cell(i + 2, 5).Value = survey.CategoryName;
                    surveysSheet.Cell(i + 2, 6).Value = survey.Title;
                    surveysSheet.Cell(i + 2, 7).Value = survey.Description;
                    surveysSheet.Cell(i + 2, 8).Value = survey.StationName;
                    surveysSheet.Cell(i + 2, 9).Value = survey.QuestionNum;
                    surveysSheet.Cell(i + 2, 10).Value = survey.IsPublished ? "是" : "否";
                    surveysSheet.Cell(i + 2, 11).Value = survey.CompleteTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "未完成";
                    surveysSheet.Cell(i + 2, 12).Value = truncatedMceHtml;
                    surveysSheet.Cell(i + 2, 13).Value = survey.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss");
                    surveysSheet.Cell(i + 2, 14).Value = survey.CompleteTime?.ToString("yyyy-MM-dd HH:mm:ss");
                    surveysSheet.Cell(i + 2, 15).Value = survey.Remark;
                }

                // 如果沒有資料，填入「無資料」
                if (surveys.Count == 0)
                {
                    surveysSheet.Cell(2, 1).Value = "無資料";
                }

                // 2. Questions 工作表
                var questionsSheet = workbook.Worksheets.Add("Questions");
                questionsSheet.Cell(1, 1).Value = "問題ID";
                questionsSheet.Cell(1, 2).Value = "問卷ID";
                questionsSheet.Cell(1, 3).Value = "問題描述";
                questionsSheet.Cell(1, 4).Value = "答案類型";
                questionsSheet.Cell(1, 5).Value = "TinyMCE HTML";
                questionsSheet.Cell(1, 6).Value = "建立時間";
                questionsSheet.Cell(1, 7).Value = "備註";

                for (int i = 0; i < questions.Count; i++)
                {
                    var question = questions[i];
                    string truncatedQuestionMceHtml = question.MceHtml?.Length > maxCellLength ? question.MceHtml.Substring(0, maxCellLength) : question.MceHtml;

                    questionsSheet.Cell(i + 2, 1).Value = question.Id;
                    questionsSheet.Cell(i + 2, 2).Value = question.SurveyId;
                    questionsSheet.Cell(i + 2, 3).Value = question.QuestionText;
                    questionsSheet.Cell(i + 2, 4).Value = question.AnswerType.ToString();
                    questionsSheet.Cell(i + 2, 5).Value = truncatedQuestionMceHtml;
                    questionsSheet.Cell(i + 2, 6).Value = question.CreateTime?.ToString("yyyy-MM-dd HH:mm:ss");
                    questionsSheet.Cell(i + 2, 7).Value = question.Remark;
                }

                // 如果沒有資料，填入「無資料」
                if (questions.Count == 0)
                {
                    questionsSheet.Cell(2, 1).Value = "無資料";
                }

                // 3. QuestionOptions 工作表
                var optionsSheet = workbook.Worksheets.Add("QuestionOptions");
                optionsSheet.Cell(1, 1).Value = "選項ID";
                optionsSheet.Cell(1, 2).Value = "問題ID";
                optionsSheet.Cell(1, 3).Value = "選項描述";
                optionsSheet.Cell(1, 4).Value = "是否正確答案";
                optionsSheet.Cell(1, 5).Value = "是否為其他選項";
                optionsSheet.Cell(1, 6).Value = "排序順序";
                optionsSheet.Cell(1, 7).Value = "描述";               

                for (int i = 0; i < questionOptions.Count; i++)
                {
                    var option = questionOptions[i];
                    optionsSheet.Cell(i + 2, 1).Value = option.Id;
                    optionsSheet.Cell(i + 2, 2).Value = option.QuestionId;
                    optionsSheet.Cell(i + 2, 3).Value = option.OptionText;
                    optionsSheet.Cell(i + 2, 4).Value = option.IsCorrect ? "是" : "否";
                    optionsSheet.Cell(i + 2, 5).Value = option.IsOther ? "是" : "否";
                    optionsSheet.Cell(i + 2, 6).Value = option.SortOrder;
                    optionsSheet.Cell(i + 2, 7).Value = option.Description;                 
                }

                // 如果沒有資料，填入「無資料」
                if (questionOptions.Count == 0)
                {
                    optionsSheet.Cell(2, 1).Value = "無資料";
                }

                // 4. QuestionImages 工作表
                var imagesSheet = workbook.Worksheets.Add("QuestionImages");
                imagesSheet.Cell(1, 1).Value = "圖片ID";
                imagesSheet.Cell(1, 2).Value = "問卷ID";
                imagesSheet.Cell(1, 3).Value = "問題ID";
                imagesSheet.Cell(1, 4).Value = "選項ID";
                imagesSheet.Cell(1, 5).Value = "圖片URL";
                imagesSheet.Cell(1, 6).Value = "替代文字";
                imagesSheet.Cell(1, 7).Value = "上傳時間";
                imagesSheet.Cell(1, 8).Value = "排序順序";               

                for (int i = 0; i < questionImages.Count; i++)
                {
                    var image = questionImages[i];
                    imagesSheet.Cell(i + 2, 1).Value = image.Id;
                    imagesSheet.Cell(i + 2, 2).Value = image.SurveyId;
                    imagesSheet.Cell(i + 2, 3).Value = image.QuestionId;
                    imagesSheet.Cell(i + 2, 4).Value = image.QuestionOptionId;
                    imagesSheet.Cell(i + 2, 5).Value = image.ImageUrl;
                    imagesSheet.Cell(i + 2, 6).Value = image.AltText;
                    imagesSheet.Cell(i + 2, 7).Value = image.UploadTime?.ToString("yyyy-MM-dd HH:mm:ss");
                    imagesSheet.Cell(i + 2, 8).Value = image.SortOrder;                  
                }

                // 如果沒有資料，填入「無資料」
                if (questionImages.Count == 0)
                {
                    imagesSheet.Cell(2, 1).Value = "無資料";
                }

                // 設定欄寬自動調整
                foreach (var sheet in workbook.Worksheets)
                {
                    sheet.Columns().AdjustToContents();
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "SurveyData.xlsx");
                }
            }
        }



        /// <summary>
        /// 匯入 Excel 資料到資料庫，包含 Survey, Question, QuestionOption 三個資料表
        /// </summary>
        /// <param name="file">上傳的 Excel 文件</param>
        /// <param name="replaceExistingData">是否替換現有資料</param>
        /// <returns>重定向到 Index 或返回錯誤訊息</returns>

        /// <summary>
        /// 匯入 Excel 資料到資料庫，包含 Survey, Question, QuestionOption 三個資料表
        /// </summary>
        /// <param name="file">上傳的 Excel 文件</param>
        /// <param name="replaceExistingData">是否替換現有資料</param>
        /// <returns>重定向到 Index 或返回錯誤訊息</returns>
        [HttpPost]
        public async Task<IActionResult> ImportExcel(IFormFile file, bool replaceExistingData)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ERROR"] = "請選擇一個有效的 Excel 文件!";
                return RedirectToAction("Index");
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var workbook = new XLWorkbook(stream))
                {
                    // 1. 讀取 Surveys 工作表
                    var surveySheet = workbook.Worksheet("Surveys");
                    var surveyRows = surveySheet.RowsUsed().Skip(1); // 跳過標題行

                    // 建立一個字典來映射舊 SurveyID 到新 SurveyID
                    var surveyIdMapping = new Dictionary<int, int>();

                    if (replaceExistingData)
                    {
                        // 刪除所有現有資料
                        _unitOfWork.QuestionOption.RemoveRange(_unitOfWork.QuestionOption.GetAll());
                        _unitOfWork.Question.RemoveRange(_unitOfWork.Question.GetAll());
                        _unitOfWork.QuestionImage.RemoveRange(_unitOfWork.QuestionImage.GetAll());
                        _unitOfWork.Survey.RemoveRange(_unitOfWork.Survey.GetAll());
                        _unitOfWork.Save();
                    }

                    foreach (var row in surveyRows)
                    {
                        // 檢查是否有資料
                        if (row.Cell(1).IsEmpty() && row.Cell(2).IsEmpty())
                        {
                            continue; // 跳過空行
                        }

                        // 處理可能的日期格式錯誤
                        DateTime? createTime = null;
                        if (row.Cell(13).TryGetValue(out DateTime parsedDateTime))
                        {
                            createTime = parsedDateTime;
                        }
                        else
                        {
                            createTime = DateTime.Now; // 如果沒有日期，使用當前時間
                        }

                        var survey = new Survey
                        {
                            ApplicationUserId = row.Cell(2).GetString(), // 假設建立人ID在第二欄
                            JobName = row.Cell(3).GetString(),
                            JobNum = row.Cell(4).GetString(),
                            CategoryName = row.Cell(5).GetString(),
                            Title = row.Cell(6).GetString(),
                            Description = row.Cell(7).GetString(),
                            StationName = row.Cell(8).GetString(),
                            QuestionNum = row.Cell(9).GetValue<int?>(),
                            IsPublished = row.Cell(10).GetString().Trim() == "是",
                            CompleteTime = row.Cell(11).GetString() != "未完成" ? row.Cell(11).GetDateTime() : (DateTime?)null,
                            MceHtml = row.Cell(12).GetString(),

                            // 匯入 CreateTime 和 Remark，如果日期無效則使用 null
                            CreateTime = createTime,
                            Remark = row.Cell(14).GetString()
                        };
                        _unitOfWork.Survey.Add(survey);
                        _unitOfWork.Save(); // 保存以生成 Survey ID
                        surveyIdMapping.Add(row.Cell(1).GetValue<int>(), survey.Id); // 假設原始問卷ID在第一欄
                    }

                    // 2. 讀取 Questions 工作表
                    var questionSheet = workbook.Worksheet("Questions");
                    var questionRows = questionSheet.RowsUsed().Skip(1);

                    var questionIdMapping = new Dictionary<int, int>();

                    foreach (var row in questionRows)
                    {
                        if (row.Cell(1).IsEmpty() && row.Cell(2).IsEmpty())
                        {
                            continue; // 跳過空行
                        }

                        var originalSurveyId = row.Cell(2).GetValue<int>();
                        if (!surveyIdMapping.ContainsKey(originalSurveyId))
                        {
                            // 無法找到對應的 Survey，跳過此問題
                            continue;
                        }

                        // 處理可能的日期格式錯誤
                        DateTime? createTime = null;
                        if (row.Cell(13).TryGetValue(out DateTime parsedDateTime))
                        {
                            createTime = parsedDateTime;
                        }
                        else
                        {
                            createTime = DateTime.Now; // 如果沒有日期，使用當前時間
                        }

                        var question = new Question
                        {
                            SurveyId = surveyIdMapping[originalSurveyId],
                            QuestionText = row.Cell(3).GetString(),
                            AnswerType = Enum.TryParse(row.Cell(4).GetString(), out AnswerTypeEnum answerType) ? answerType : AnswerTypeEnum.SingleChoice,
                            MceHtml = row.Cell(5).GetString(),

                            // 匯入 CreateTime 和 Remark
                            CreateTime = createTime,
                            Remark = row.Cell(7).GetString()
                        };
                        _unitOfWork.Question.Add(question);
                        _unitOfWork.Save(); // 保存以生成 Question ID
                        questionIdMapping.Add(row.Cell(1).GetValue<int>(), question.Id); // 假設原始問題ID在第一欄
                    }

                    // 3. 讀取 QuestionOptions 工作表
                    var optionSheet = workbook.Worksheet("QuestionOptions");
                    var optionRows = optionSheet.RowsUsed().Skip(1);

                    foreach (var row in optionRows)
                    {
                        if (row.Cell(1).IsEmpty() && row.Cell(2).IsEmpty())
                        {
                            continue; // 跳過空行
                        }

                        var originalQuestionId = row.Cell(2).GetValue<int>();
                        if (!questionIdMapping.ContainsKey(originalQuestionId))
                        {
                            // 無法找到對應的 Question，跳過此選項
                            continue;
                        }

                        var option = new QuestionOption
                        {
                            QuestionId = questionIdMapping[originalQuestionId],
                            OptionText = row.Cell(3).GetString(),
                            IsCorrect = row.Cell(4).GetString().Trim() == "是",
                            IsOther = row.Cell(5).GetString().Trim() == "是",
                            SortOrder = row.Cell(6).GetValue<int?>(),
                            Description = row.Cell(7).GetString(),
                        };
                        _unitOfWork.QuestionOption.Add(option);
                    }

                    _unitOfWork.Save(); // 儲存所有選項

                    // 4. 讀取 QuestionImages 工作表
                    var imageSheet = workbook.Worksheet("QuestionImages");
                    var imageRows = imageSheet.RowsUsed().Skip(1);

                    foreach (var row in imageRows)
                    {
                        if (row.Cell(1).IsEmpty() && row.Cell(2).IsEmpty())
                        {
                            continue; // 跳過空行
                        }

                        // 確保 SurveyId 存在於映射中
                        var originalSurveyId = row.Cell(2).GetValue<int?>();
                        if (!originalSurveyId.HasValue || !surveyIdMapping.ContainsKey(originalSurveyId.Value))
                        {
                            // 無法找到對應的 Survey，跳過此圖片
                            continue;
                        }

                        // 處理圖片上傳時間（加入 TryGetValue 檢查避免日期解析錯誤）
                        DateTime? uploadTime = null;
                        if (row.Cell(7).TryGetValue(out DateTime parsedUploadTime))
                        {
                            uploadTime = parsedUploadTime;
                        }
                        else
                        {
                            // 如果不是有效的日期，賦予當前時間
                            uploadTime = DateTime.Now;
                        }

                        // 處理圖片資料
                        var questionImage = new QuestionImage
                        {
                            SurveyId = surveyIdMapping[originalSurveyId.Value], // 確保使用正確映射後的 SurveyId
                            QuestionId = row.Cell(3).GetValue<int?>(),
                            QuestionOptionId = row.Cell(4).GetValue<int?>(),
                            ImageUrl = row.Cell(5).GetString(),
                            AltText = row.Cell(6).GetString(),
                            UploadTime = uploadTime, // 使用解析的日期值或預設值
                            SortOrder = row.Cell(8).GetValue<int?>()
                        };
                        _unitOfWork.QuestionImage.Add(questionImage);
                    }

                    _unitOfWork.Save(); // 儲存所有圖片

                }
            }

            TempData["SUCCESS"] = replaceExistingData ? "資料取代成功!" : "資料新增成功!";
            return RedirectToAction("Index");
        }







    }
}
