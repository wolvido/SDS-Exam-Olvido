using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdsExamOlvido.Models;

namespace SdsExamOlvido.ServiceContracts
{
    public interface IRecyclableTypeService
    {
        Task<bool> CreateRecyclableType(RecyclableType recyclableType);

        Task<bool> DeleteRecyclableType(int id);

        Task<bool> UpdateRecyclableType(RecyclableType recyclableType);

        Task<IEnumerable<RecyclableType>> GetAllRecyclableTypes();

        Task<RecyclableType> GetRecyclableTypeById(int id);

    }
}
