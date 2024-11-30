using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SdsExamOlvido.Models
{
    public class RecyclableType
    {
        [Key]
        [Required(ErrorMessage = "Recyclable Type ID is required.")]
        public int Id { get; set; }
        //[Column(TypeName = "varchar(10)")]
        [MaxLength(10, ErrorMessage="Type name must be 10 characters or less")]
        public string Type { get; set; }
        //[Column(TypeName = "decimal(18, 2)")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "The value must not exceed 2 decimal places.")]
        public decimal Rate { get; set; }
        //[Column(TypeName = "decimal(18, 2)")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "The value must not exceed 2 decimal places.")]
        public decimal MinKg { get; set; }
        //[Column(TypeName = "decimal(18, 2)")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "The value must not exceed 2 decimal places.")]
        public decimal MaxKg { get; set; }
    }
}