using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http; // 新增此命名空間以使用 IFormFile

namespace TeaTimeDemo.Models.ViewModels
{
    public class QuestionVM
    {
        public Question Question { get; set; }

        // 答案類型的下拉選單（單選、多選、填空等）
        [ValidateNever]
        public IEnumerable<SelectListItem> AnswerTypeList { get; set; }

        // 這個問題的選項清單
        [ValidateNever]
        public IEnumerable<QuestionOptionVM> QuestionOptions { get; set; }

        // 這個問題的選項 ViewModel 列表
        public List<QuestionOptionVM> QuestionOptionVMs { get; set; } = new List<QuestionOptionVM>();

        public List<QuestionImage> QuestionImages { get; set; } = new List<QuestionImage>();

        // **新增屬性**：用於接收問題的圖片檔案
        [ValidateNever]
        public List<IFormFile> QuestionImageFiles { get; set; } = new List<IFormFile>();

        [ValidateNever]
        public string? MceHtml { get; set; }

    }
}
