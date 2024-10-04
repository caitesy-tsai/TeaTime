using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;


namespace TeaTimeDemo.Models
{
    public class Survey : BaseEntity
    {
        // 外鍵對應到 ApplicationUser 資料表
        public string? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }  // 導航屬性，關聯到 ApplicationUser

        // 儲存建立人姓名，設限 100 字元長度
        [MaxLength(100)]
        public string? JobName { get; set; }

        // 儲存建立人工號，設限 50 字元長度
        [MaxLength(50)]
        public string? JobNum { get; set; }

        // 類別名稱
        [Required(ErrorMessage = "類別是必填欄位")]
        public string CategoryName { get; set; }

        // 問卷標題，必填，並設限 200 字元長度
        [Required(ErrorMessage = "分類標題是必填欄位")]
        [MaxLength(200, ErrorMessage = "分類標題不能超過 200 個字元")]
        public string Title { get; set; }

        // 問卷描述，選填
        public string? Description { get; set; }

        // 儲存站別名稱
        [Required(ErrorMessage = "站別名稱是必填欄位")]
        public string StationName { get; set; }  // 直接存儲站別名稱

        // 問卷頁數或問題順序，選填
        public int? QuestionNum { get; set; }

        // 是否發布問卷
        public bool IsPublished { get; set; }

        // 儲存 TinyMCE 富文本編輯器中的 HTML 內容，非必填
        [ValidateNever]
        [Column(TypeName = "nvarchar(max)")] // 設定欄位類型為 nvarchar(max)
        public string? MceHtml { get; set; }


        // 關聯到該問題的圖片集合
        // 使用 virtual 可支援延遲加載，並初始化為一個空的 List
        public virtual ICollection<QuestionImage> QuestionImages { get; set; } = new List<QuestionImage>();

        // 關聯到問題的集合
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
