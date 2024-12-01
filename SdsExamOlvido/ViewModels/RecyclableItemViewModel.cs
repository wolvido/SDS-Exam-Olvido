using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SdsExamOlvido.ViewModels
{
    public class RecyclableItemViewModel
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public decimal Weight { get; set; }

        public decimal ComputedRate { get; set; }

        public string ItemDescription { get; set; }
    }
}