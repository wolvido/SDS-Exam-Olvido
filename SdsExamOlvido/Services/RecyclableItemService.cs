using SdsExamOlvido.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SdsExamOlvido.Models;
using SdsExamOlvido.DbContexts;
using System.Data.Entity;

namespace SdsExamOlvido.Services
{
    public class RecyclableItemService : IRecyclableItemService
    {
        private readonly ApplicationDbContext _context;
        public RecyclableItemService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateRecyclableItem(RecyclableItem recyclableItem)
        {
            _context.RecyclableItems.Add(recyclableItem);
            bool result = await _context.SaveChangesAsync() > 0;
            return result;
        }

        public async Task<bool> DeleteRecyclableItem(int id)
        {
            RecyclableItem recyclableItem = await _context.RecyclableItems.FindAsync(id);
            if (recyclableItem == null)
            {
                return false;
            }
            _context.RecyclableItems.Remove(recyclableItem);
            bool result = await _context.SaveChangesAsync() > 0;
            return result;
        }

        public async Task<bool> UpdateRecyclableItem(RecyclableItem recyclableItem)
        {
            RecyclableItem existingItem = await _context.RecyclableItems.FindAsync(recyclableItem.Id);
            if (existingItem == null)
            {
                return false;
            }
            existingItem.RecyclableTypeId = recyclableItem.RecyclableTypeId;
            existingItem.Weight = recyclableItem.Weight;
            existingItem.ComputedRate = recyclableItem.ComputedRate;
            existingItem.ItemDescription = recyclableItem.ItemDescription;

            bool result = await _context.SaveChangesAsync() > 0;
            return result;
        }

        public async Task<IEnumerable<RecyclableItem>> GetAllRecyclableItems()
        {
            return await _context.RecyclableItems.ToListAsync();
        }

        public async Task<RecyclableItem> GetRecyclableItemById(int id)
        {
            RecyclableItem recyclableItem = await _context.RecyclableItems.FindAsync(id);
            if (recyclableItem == null)
            {
                return null;
            }
            return recyclableItem;
        }
    }
}