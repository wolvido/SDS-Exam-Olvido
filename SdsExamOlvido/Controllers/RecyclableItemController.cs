using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SdsExamOlvido.Models;
using SdsExamOlvido.ServiceContracts;

namespace SdsExamOlvido.Controllers
{
    public class RecyclableItemController : Controller
    {
        private readonly IRecyclableTypeService _recyclableTypeService;

        public RecyclableItemController(IRecyclableTypeService recyclableTypeService)
        {
            _recyclableTypeService = recyclableTypeService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> RecyclableItemList()
        {
            //sample data
            var recyclableItemList = new List<RecyclableItem>
                {
                    new RecyclableItem { Id = 1, RecyclableTypeId = 82, Weight = 123, ComputedRate = 34, ItemDescription = "dangerous" },
                    new RecyclableItem { Id = 2, RecyclableTypeId = 14, Weight = 6, ComputedRate = 7, ItemDescription = "hard matter" },
                    new RecyclableItem { Id = 3, RecyclableTypeId = 51, Weight = 6, ComputedRate = 3, ItemDescription = "flammable" },
                    new RecyclableItem { Id = 4, RecyclableTypeId = 5, Weight = 23, ComputedRate = 8, ItemDescription = "toxic" }
                };
            IEnumerable<RecyclableItem> itemsTest = recyclableItemList;

            return View(itemsTest);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> CreateRecyclableItem()
        {
            //ViewBag.RecyclableTypes = await _recyclableTypeService.GetAllRecyclableTypes();
            var recyclableTypeList = new List<RecyclableType>
                {
                    new RecyclableType { Id = 1, Type = "Plastic", Rate = 2.33M, MinKg = 1, MaxKg = 10 },
                    new RecyclableType { Id = 2, Type = "Paper", Rate = 3.25M, MinKg = 1, MaxKg = 10 },
                    new RecyclableType { Id = 3, Type = "Glass", Rate = 1.56M, MinKg = 1, MaxKg = 10 },
                    new RecyclableType { Id = 4, Type = "Metal", Rate = 1.45M, MinKg = 1, MaxKg = 10 }
                };

            ViewBag.RecyclableTypes = recyclableTypeList;
            
            //tests, delete later
            ViewBag.Test = TempData["Test"];
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> CreateRecyclableItem(RecyclableItem recyclableItem)
        {
            var recyclableTypeList = new List<RecyclableType>
            {
                new RecyclableType { Id = 1, Type = "Plastic", Rate = 2.33M, MinKg = 1, MaxKg = 10 },
                new RecyclableType { Id = 2, Type = "Paper", Rate = 3.25M, MinKg = 1, MaxKg = 10 },
                new RecyclableType { Id = 3, Type = "Glass", Rate = 1.56M, MinKg = 1, MaxKg = 10 },
                new RecyclableType { Id = 4, Type = "Metal", Rate = 1.45M, MinKg = 1, MaxKg = 10 }
            };

            ViewBag.RecyclableTypes = recyclableTypeList;
            if (ModelState.IsValid)
            {
                // Process the data

                //tests, delete later
                TempData["Test"] = recyclableItem;
                return RedirectToAction("CreateRecyclableItem");
            }

            return View();
        }
    }
}