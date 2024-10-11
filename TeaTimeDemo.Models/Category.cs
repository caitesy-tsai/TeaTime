﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TeaTimeDemo.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        [DisplayName("類別名稱")]         
        public string Name { get; set; }
        [DisplayName("顯示順序")]
        [Range(1,100,ErrorMessage="輸入範圍應該要1-100之間")]
        public int DisplayOrder { get; set; }

        // 反向導航屬性
        public virtual ICollection<Survey> Surveys { get; set; } = new List<Survey>();
    }
}
