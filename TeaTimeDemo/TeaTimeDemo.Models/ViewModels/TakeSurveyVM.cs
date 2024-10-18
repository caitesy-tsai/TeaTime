// TakeSurveyVM.cs
using System.Collections.Generic;
using TeaTimeDemo.Models;

namespace TeaTimeDemo.Models.ViewModels
{
    public class TakeSurveyVM
    {
        public virtual Survey Survey { get; set; }
        public List<QuestionVM> Questions { get; set; }

        public string MceHtml { get; set; } // 新增 MceHtml 屬性
    }
}
