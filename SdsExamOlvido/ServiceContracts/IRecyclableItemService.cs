using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdsExamOlvido.Models;

namespace SdsExamOlvido.ServiceContracts
{
    public interface IRecyclableItemService
    {
        Task<bool> CreateRecyclableItem(RecyclableItem recyclableItem);
        Task<bool> DeleteRecyclableItem(int id);
        Task<bool> UpdateRecyclableItem(RecyclableItem recyclableItem);
        Task<IEnumerable<RecyclableItem>> GetAllRecyclableItems();
        Task<RecyclableItem> GetRecyclableItemById(int id);
    }
}
