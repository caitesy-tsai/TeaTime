using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http; // 新增此命名空間以使用 IFormFile

namespace TeaTimeDemo.Models.ViewModels
{
    public class SurveyVM
    {
        public Survey Survey { get; set; }  // 問卷實體

        // 類別的下拉選單
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }

        // 站別的下拉選單
        [ValidateNever]
        public IEnumerable<SelectListItem> StationList { get; set; }

        // 問題類型的下拉選單（單選、多選、填空等）
        [ValidateNever]
        public IEnumerable<SelectListItem> QuestionTypeList { get; set; }

        // 問卷中包含的問題 ViewModel 列表
        public List<QuestionVM> QuestionVMs { get; set; } = new List<QuestionVM>();


        // **新增屬性**：用於接收問卷的圖片檔案
        [ValidateNever]
        public List<IFormFile> SurveyImageFiles { get; set; } = new List<IFormFile>();

        [ValidateNever]
        public string? MceHtml { get; set; }
    }
}
