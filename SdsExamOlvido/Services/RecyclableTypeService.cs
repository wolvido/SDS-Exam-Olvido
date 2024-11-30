using SdsExamOlvido.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using SdsExamOlvido.Models;
using SdsExamOlvido.DbContexts;

namespace SdsExamOlvido.Services
{
    public class RecyclableTypeService : IRecyclableTypeService
    {
        private readonly ApplicationDbContext _context;
        public RecyclableTypeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateRecyclableType(RecyclableType recyclableType)
        {
            if (recyclableType == null)
            {
                return false;
            }

            if (!IsValidRateOrWeight(recyclableType))
            {
                return false;
            }

            _context.RecyclableTypes.Add(recyclableType);

            bool result = await _context.SaveChangesAsync() > 0;

            return result;
        }

        private bool IsValidRateOrWeight(RecyclableType recyclableType)
        {
            return Decimal.Round(recyclableType.Rate, 2) == recyclableType.Rate &&
                   Decimal.Round(recyclableType.MinKg, 2) == recyclableType.MinKg &&
                   Decimal.Round(recyclableType.MaxKg, 2) == recyclableType.MaxKg;
        }

        public async Task<bool> DeleteRecyclableType(int id)
        {
            RecyclableType recyclableType = await _context.RecyclableTypes.FindAsync(id);

            if (recyclableType == null)
            {
                return false;
            }

            _context.RecyclableTypes.Remove(recyclableType);
            bool result = await _context.SaveChangesAsync() > 0;
            return result;
        }

        public async Task<bool> UpdateRecyclableType(RecyclableType recyclableType)
        {
            RecyclableType existingRecyclableType = await _context.RecyclableTypes.FindAsync(recyclableType.Id);
            if (existingRecyclableType == null)
            {
                return false;
            }

            existingRecyclableType.Type = recyclableType.Type;
            existingRecyclableType.Rate = recyclableType.Rate;
            existingRecyclableType.MinKg = recyclableType.MinKg;
            existingRecyclableType.MaxKg = recyclableType.MaxKg;

            bool result = await _context.SaveChangesAsync() > 0;
            return result;
        }

        public async Task<IEnumerable<RecyclableType>> GetAllRecyclableTypes()
        {
            return await _context.RecyclableTypes.ToListAsync();
        }

    }
}