using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SdsExamOlvido.Models;

namespace SdsExamOlvido.Controllers
{
    public class RecyclableItemController : Controller
    {
        [Route("[action]")]
        [HttpGet]
        public ActionResult RecyclableItemList()
        {
            //sample data
            var recyclableItemList = new List<RecyclableItem>
                {
                    new RecyclableItem { Id = 1, RecyclableTypeId = 82, Weight = 123, ComputedRate = 34, ItemDescription = "very Recyclable" },
                    new RecyclableItem { Id = 2, RecyclableTypeId = 14, Weight = 6, ComputedRate = 7, ItemDescription = "test test"  },
                    new RecyclableItem { Id = 3, RecyclableTypeId = 51, Weight = 6, ComputedRate = 3, ItemDescription = "pure plastic"  },
                    new RecyclableItem { Id = 4, RecyclableTypeId = 5, Weight = 23, ComputedRate = 8, ItemDescription = "very hard"  }
                };
            IEnumerable<RecyclableItem> modelTest = recyclableItemList;

            return View(modelTest);
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult CreateRecyclableItem()
        {
            //tests, delete later
            ViewBag.Test = TempData["Test"];
            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public ActionResult CreateRecyclableItem(RecyclableItem recyclableItem)
        {
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