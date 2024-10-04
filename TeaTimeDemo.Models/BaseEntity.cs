using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace TeaTimeDemo.Models
{
    public abstract class BaseEntity
    {
        // 主鍵，唯一標識
        [Key]
        public int Id { get; set; }

        // 創建時間，設置預設值為當前時間並將其設定為只讀
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // 自動生成的欄位
        public DateTime? CreateTime { get; set; } = DateTime.Now;

        // 完成時間，預設為空
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        [DataType(DataType.DateTime)]
        public DateTime? CompleteTime { get; set; }

        // 備註，選填欄位，並設置資料庫中的欄位類型為 nvarchar(max)
        [ValidateNever]
        [Column(TypeName = "nvarchar(max)")]
        public string? Remark { get; set; }
    }
}
