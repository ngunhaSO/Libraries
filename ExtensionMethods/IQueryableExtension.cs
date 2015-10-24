using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Objects;

namespace Core.Common.Extensions
{
//     usage: var existingParent = context.AnnualConstructionPrograms.Where(a => a.Id == program.Id)
//                                    .Include(a => a.AnnualConstructionProgramItems).FirstOrDefault();
    public static class IQueryableExtension
    {
        public static IQueryable<T> Include<T> (this IQueryable<T> query, string property) where T:new()
        {
            var objectQuery = query as ObjectQuery<T>;
            if(objectQuery == null)
            {
                throw new NotSupportedException("Include can only be called on ObjectQuery");
            }
            return objectQuery.Include(property);
        }
    }
}
