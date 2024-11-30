using SdsExamOlvido.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SdsExamOlvido.Controllers
{
    public class RecyclableTypeController : Controller
    {
        [Route("[action]")]
        [HttpGet]
        public ActionResult RecyclableTypeList()
        {
                //sample data of the model in a list
                var recyclableTypeList = new List<RecyclableType>
                {
                    new RecyclableType { Id = 1, Type = "Plastic", Rate = 2.33M, MinKg = 1, MaxKg = 10 },
                    new RecyclableType { Id = 2, Type = "Paper", Rate = 3.25M, MinKg = 1, MaxKg = 10 },
                    new RecyclableType { Id = 3, Type = "Glass", Rate = 1.56M, MinKg = 1, MaxKg = 10 },
                    new RecyclableType { Id = 4, Type = "Metal", Rate = 1.45M, MinKg = 1, MaxKg = 10 }
                };

                IEnumerable<RecyclableType> modelTest = recyclableTypeList;

            return View(modelTest);
        }

        [Route("[action]")]
        [HttpGet]
        public ActionResult CreateRecyclableType()
        {
            //tests, delete later
            ViewBag.Test = TempData["Test"];

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public ActionResult CreateRecyclableType(RecyclableType recyclableType)
        {
            if (ModelState.IsValid)
            {
                // Process the data

                //tests, delete later
                TempData["Test"] = recyclableType;

                return RedirectToAction("CreateRecyclableType");
            }

            return View();
        }
    }
}