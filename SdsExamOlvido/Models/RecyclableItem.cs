using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SdsExamOlvido.Models
{
    public class RecyclableItem
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int RecyclableTypeId { get; set; }

        [ForeignKey("RecyclableTypeId")]
        public RecyclableType RecyclableType { get; set; }

        //[Column(TypeName = "decimal(18, 2)")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "The value must not exceed 2 decimal places.")]
        public decimal Weight { get; set; }

        //[Column(TypeName = "decimal(18, 2)")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "The value must not exceed 2 decimal places.")]
        [Display(Name = "Computed Rate")]
        public decimal ComputedRate { get; set; }

        //[Column(TypeName = "varchar(150)")]
        [MaxLength(150, ErrorMessage="Item Description must be 150 characters or less")]
        [Display(Name = "Description")]
        public string ItemDescription { get; set; }

    }
}