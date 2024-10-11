using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations; // 新增此命名空間以使用 IFormFile

namespace TeaTimeDemo.Models.ViewModels
{
    public class QuestionVM
    {
        public Question Question { get; set; }

        // 答案類型的下拉選單（單選、多選、填空等）
        [ValidateNever]       

        public List<SelectListItem> AnswerTypeList { get; set; }

        // 這裡加入 AnswerType 屬性
        public AnswerTypeEnum AnswerType { get; set; }

        // 這個問題的選項清單
        [ValidateNever]
        public IEnumerable<QuestionOptionVM> QuestionOptions { get; set; }


        // 這個問題的選項 ViewModel 列表
        public List<QuestionOptionVM> QuestionOptionVMs { get; set; } = new List<QuestionOptionVM>();


        // 用於接收用戶的回答
        // 使用者選擇的選項
        public List<int> SelectedOptions { get; set; } = new List<int>();

        // 單選選項
        public int? SelectedOption { get; set; }

        // 對於填空類型的答案
        [MaxLength(500, ErrorMessage = "答案不能超過500個字元")]
        public string? AnswerText { get; set; }


        public List<QuestionImage> QuestionImages { get; set; } = new List<QuestionImage>();

        // **新增屬性**：用於接收問題的圖片檔案
        [ValidateNever]
        public List<IFormFile> QuestionImageFiles { get; set; } = new List<IFormFile>();

        [ValidateNever]
        public string? MceHtml { get; set; }

    }
}
