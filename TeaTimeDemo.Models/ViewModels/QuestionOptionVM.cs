using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http; // 新增此命名空間以使用 IFormFile

namespace TeaTimeDemo.Models.ViewModels
{
    public class QuestionOptionVM
    {
        public QuestionOption QuestionOption { get; set; }

        // 問題的下拉選單
        [ValidateNever]
        public IEnumerable<SelectListItem> QuestionList { get; set; }

        // 問題選項的下拉選單
        [ValidateNever]
        public IEnumerable<SelectListItem> QuestionOptionList { get; set; }

        public List<QuestionImage> QuestionOptionImages { get; set; } = new List<QuestionImage>();

        // 新增這一行，用於接收選項的圖片
        [ValidateNever]
        public List<IFormFile> OptionImageFiles { get; set; } = new List<IFormFile>();

        [ValidateNever]
        public bool IsDeleted { get; set; } // 用來標記是否已刪除


    }
}
