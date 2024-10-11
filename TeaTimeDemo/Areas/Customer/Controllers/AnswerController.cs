// AnswerController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TeaTimeDemo.DataAccess.Repository.IRepository;
using TeaTimeDemo.Models;
using TeaTimeDemo.Models.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace TeaTimeDemo.Controllers
{
    [Area("Customer")] // 指定區域為 Customer
    [Authorize] // 確保只有登入的使用者能夠存取
    public class AnswerController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnswerController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 顯示所有已發布的問卷
        public IActionResult Index()
        {
            var surveys = _unitOfWork.Survey.GetAll(
                filter: s => s.IsPublished,
                includeProperties: "Questions.QuestionOptions");
            return View(surveys);
        }


        // 顯示特定問卷的詳細內容及問題
        public IActionResult TakeSurvey(int id)
        {
            var survey = _unitOfWork.Survey.GetFirstOrDefault(
                s => s.Id == id && s.IsPublished,
                includeProperties: "Questions.QuestionOptions"
            );

            if (survey == null)
            {
                return NotFound();
            }

            var surveyVM = new SurveyVM
            {
                Survey = survey,
                QuestionVMs = survey.Questions.Select(q => new QuestionVM
                {
                    Question = q,
                    QuestionOptionVMs = q.QuestionOptions.Select(o => new QuestionOptionVM
                    {
                        QuestionOption = o
                    }).ToList()
                }).ToList()
            };

            var takeSurveyVM = new TakeSurveyVM
            {
                Survey = survey,
                Questions = surveyVM.QuestionVMs,
                MceHtml = GenerateMceHtml(surveyVM) // 使用 GenerateMceHtml 生成 MceHtml
            };

            return View(takeSurveyVM);
        }




        // 處理問卷提交
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SubmitSurvey(TakeSurveyVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    foreach (var questionVM in model.Questions)
                    {
                        // 確保 Question 不為 null 並且 Id 已設置
                        if (questionVM.Question == null || questionVM.Question.Id == 0)
                        {
                            // 可能需要從數據庫中重新獲取問題
                            var questionFromDb = _unitOfWork.Question.GetFirstOrDefault(q => q.Id == questionVM.Question.Id);
                            if (questionFromDb == null)
                            {
                                ModelState.AddModelError("", "無效的問題 ID。");
                                continue; // 跳過此問題
                            }
                            questionVM.Question = questionFromDb;
                        }

                        var answer = new Answer
                        {
                            QuestionId = questionVM.Question.Id,
                            ApplicationUserId = currentUserId,
                            AnswerType = questionVM.Question.AnswerType,
                            AnswerText = questionVM.AnswerText
                        };

                        if (questionVM.Question.AnswerType == AnswerTypeEnum.SingleChoice)
                        {
                            if (questionVM.SelectedOption.HasValue)
                            {
                                var answerOption = new AnswerOption
                                {
                                    QuestionOptionId = questionVM.SelectedOption.Value,
                                    Answer = answer
                                };
                                answer.SelectedOptions.Add(answerOption);
                            }
                        }
                        else if (questionVM.Question.AnswerType == AnswerTypeEnum.MultipleChoice)
                        {
                            if (questionVM.SelectedOptions != null)
                            {
                                foreach (var selectedOptionId in questionVM.SelectedOptions)
                                {
                                    var answerOption = new AnswerOption
                                    {
                                        QuestionOptionId = selectedOptionId,
                                        Answer = answer
                                    };
                                    answer.SelectedOptions.Add(answerOption);
                                }
                            }
                        }

                        _unitOfWork.Answer.Add(answer);
                    }

                    _unitOfWork.Save();
                    TempData["success"] = "感謝您填寫問卷！";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    // 記錄例外訊息
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.InnerException?.Message);

                    ModelState.AddModelError("", "提交問卷時發生錯誤，請稍後再試。");
                }
            }

            // 如果模型無效，重新顯示問卷
            // 需要重新加載 AnswerTypeList 和 QuestionOptionVMs
            foreach (var questionVM in model.Questions)
            {
                questionVM.AnswerTypeList = GetAnswerTypeList();
                questionVM.QuestionOptionVMs = _unitOfWork.QuestionOption.GetAll(
                    qo => qo.QuestionId == questionVM.Question.Id
                ).Select(o => new QuestionOptionVM { QuestionOption = o }).ToList();
            }

            return View("TakeSurvey", model);
        }



        // 在這裡添加 GenerateMceHtml 方法
        private string GenerateMceHtml(SurveyVM surveyVM)
        {
            var survey = surveyVM.Survey;
            var questionsHtml = new System.Text.StringBuilder();

            foreach (var questionVM in surveyVM.QuestionVMs)
            {
                var question = questionVM.Question;
                var answerTypeText = GetEnumDisplayName(question.AnswerType);

                // 建立問題的標題
                var questionHtml = $"<h3>{question.QuestionText} ({answerTypeText})</h3>";

                // 添加隱藏欄位以傳遞 Question.Id
                var qIndex = surveyVM.QuestionVMs.IndexOf(questionVM);
                questionHtml += $"<input type=\"hidden\" name=\"Questions[{qIndex}].Question.Id\" value=\"{question.Id}\" />";

                var optionsHtml = new System.Text.StringBuilder();

                // 根據答案類型生成相應的輸入欄位
                if (question.AnswerType == AnswerTypeEnum.SingleChoice)
                {
                    foreach (var optionVM in questionVM.QuestionOptionVMs)
                    {
                        var option = optionVM.QuestionOption;
                        var optionIndex = questionVM.QuestionOptionVMs.IndexOf(optionVM);
                        optionsHtml.Append(
                            $"<input type=\"hidden\" name=\"Questions[{qIndex}].QuestionOptionVMs[{optionIndex}].QuestionOption.Id\" value=\"{option.Id}\" />" +
                            $"<input type=\"radio\" name=\"Questions[{qIndex}].SelectedOption\" value=\"{option.Id}\" id=\"option_{option.Id}\" required> " +
                            $"<label for=\"option_{option.Id}\">{option.OptionText}</label><br />");
                    }
                }
                else if (question.AnswerType == AnswerTypeEnum.MultipleChoice)
                {
                    foreach (var optionVM in questionVM.QuestionOptionVMs)
                    {
                        var option = optionVM.QuestionOption;
                        var optionIndex = questionVM.QuestionOptionVMs.IndexOf(optionVM);
                        optionsHtml.Append(
                            $"<input type=\"hidden\" name=\"Questions[{qIndex}].QuestionOptionVMs[{optionIndex}].QuestionOption.Id\" value=\"{option.Id}\" />" +
                            $"<input type=\"checkbox\" name=\"Questions[{qIndex}].SelectedOptions\" value=\"{option.Id}\" id=\"option_{option.Id}\"> " +
                            $"<label for=\"option_{option.Id}\">{option.OptionText}</label><br />");
                    }
                }
                else if (question.AnswerType == AnswerTypeEnum.TextAnswer)
                {
                    optionsHtml.Append($"<input type=\"text\" name=\"Questions[{qIndex}].AnswerText\" required class=\"form-control\" /><br />");
                }
                else if (question.AnswerType == AnswerTypeEnum.TextareaAnswer)
                {
                    optionsHtml.Append($"<textarea name=\"Questions[{qIndex}].AnswerText\" required class=\"form-control\"></textarea><br />");
                }

                questionHtml += "<p>" + optionsHtml.ToString() + "</p>";
                questionsHtml.Append(questionHtml);
            }

            // 建立問卷的標題和描述
            var content = $"<h1>{survey.Title}<span style=\"font-size: 0.7em; margin-left: 10px;\">站別: {survey.StationName} 頁數：{survey.QuestionNum}</span></h1>";
            content += $"<p>{survey.Description}</p>";

            // 加入所有問題的 HTML
            content += questionsHtml.ToString();

            return content;
        }



        // 輔助方法：取得答案類型的下拉選單
        private List<SelectListItem> GetAnswerTypeList()
        {
            var enumValues = Enum.GetValues(typeof(AnswerTypeEnum)).Cast<AnswerTypeEnum>();
            return enumValues.Select(e => new SelectListItem
            {
                Text = GetEnumDisplayName(e),
                Value = ((int)e).ToString()
            }).ToList();
        }

        // 輔助方法：取得枚舉的 Display 名稱
        private string GetEnumDisplayName(Enum enumValue)
        {
            var displayAttribute = enumValue.GetType().GetField(enumValue.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false).FirstOrDefault() as DisplayAttribute;

            return displayAttribute?.Name ?? enumValue.ToString();
        }
    }
}
