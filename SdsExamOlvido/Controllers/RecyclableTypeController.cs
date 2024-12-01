using SdsExamOlvido.Models;
using SdsExamOlvido.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SdsExamOlvido.Controllers
{
    public class RecyclableTypeController : Controller
    {
        private readonly IRecyclableTypeService _recyclableTypeService;

        public RecyclableTypeController(IRecyclableTypeService recyclableTypeService)
        {
            _recyclableTypeService = recyclableTypeService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> RecyclableTypeList()
        {
            var typeList = await _recyclableTypeService.GetAllRecyclableTypes();
            return View(typeList);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> CreateRecyclableType(int? id)
        {
            if (id.HasValue && id.Value > 0)
            {
                RecyclableType recyclableType = await _recyclableTypeService.GetRecyclableTypeById(id.Value);
                if (recyclableType == null) {
                   //try again page
                }
                return View(recyclableType);
            }

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> CreateRecyclableType(RecyclableType recyclableType)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (recyclableType.Id > 0)
            {
                bool updateResult = await _recyclableTypeService.UpdateRecyclableType(recyclableType);
                if (!updateResult)
                {
                    // redirect to try again page
                }
                return RedirectToAction("RecyclableTypeList");
            }

            bool createResult = await _recyclableTypeService.CreateRecyclableType(recyclableType);
            if (!createResult) {
                // redirect to try again page
            }

            //tests, delete later
            TempData["Test"] = recyclableType;

            return RedirectToAction("RecyclableTypeList");
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> DeleteRecyclableType(int id)
        {
            bool result = await _recyclableTypeService.DeleteRecyclableType(id);

            if (!result)
            {
                //try again page
            }
            return RedirectToAction("RecyclableTypeList");
        }

        //public async Task<ActionResult> IsTypeAvailable(string type)
        //{

        //}

    }
}