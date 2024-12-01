using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SdsExamOlvido.Models;
using SdsExamOlvido.ServiceContracts;
using SdsExamOlvido.ViewModels;

namespace SdsExamOlvido.Controllers
{
    public class RecyclableItemController : Controller
    {
        private readonly IRecyclableTypeService _recyclableTypeService;
        private readonly IRecyclableItemService _recyclableItemService;

        public RecyclableItemController(IRecyclableTypeService recyclableTypeService, IRecyclableItemService recyclableItemService)
        {
            _recyclableTypeService = recyclableTypeService;
            _recyclableItemService = recyclableItemService;
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> RecyclableItemList()
        {
            var recyclableItems = await _recyclableItemService.GetAllRecyclableItems();
            var recyclableTypes = await _recyclableTypeService.GetAllRecyclableTypes();

            var recyclableItemViewModels = recyclableItems.Select(
                item => new RecyclableItemViewModel
                {
                    Id = item.Id,
                    Type = recyclableTypes.FirstOrDefault(x => x.Id == item.RecyclableTypeId)?.Type ?? "No Type",
                    Weight = item.Weight,
                    ComputedRate = item.ComputedRate,
                    ItemDescription = item.ItemDescription
                });

            return View(recyclableItemViewModels);
        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> CreateRecyclableItem(int? id)
        {
            //dropdown recyclable types options
            var recyclableTypes = await _recyclableTypeService.GetAllRecyclableTypes();
            ViewBag.RecyclableTypes = recyclableTypes;

            if (id.HasValue && id.Value > 0)
            {
                RecyclableItem recyclableItem = await _recyclableItemService.GetRecyclableItemById(id.Value);
                if (recyclableItem != null)
                {
                    //try again page
                }
                return View(recyclableItem);
            }

            return View();
        }

        [Route("[action]")]
        [HttpPost]
        public async Task<ActionResult> CreateRecyclableItem(RecyclableItem recyclableItem)
        {
            var recyclableTypes = await _recyclableTypeService.GetAllRecyclableTypes();
            ViewBag.RecyclableTypes = recyclableTypes;

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (recyclableItem.Id > 0)
            {
                bool updateResult = await _recyclableItemService.UpdateRecyclableItem(recyclableItem);
                if (!updateResult)
                {
                    // redirect to try again page
                }
                return RedirectToAction("RecyclableItemList");
            }

            bool createResult = await _recyclableItemService.CreateRecyclableItem(recyclableItem);
            if (!createResult) 
            {
                // redirect to try again page
            }
            //tests, delete later
            TempData["Test"] = recyclableItem;
            return RedirectToAction("RecyclableItemList");

        }

        [Route("[action]")]
        [HttpGet]
        public async Task<ActionResult> DeleteRecyclableItem(int id)
        {
            bool deleteResult = await _recyclableItemService.DeleteRecyclableItem(id);
            if (!deleteResult)
            {
                // redirect to try again page
            }
            return RedirectToAction("RecyclableItemList");
        }
    }
}