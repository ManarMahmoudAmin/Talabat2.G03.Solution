using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Infrastructure
{
    internal static class SpecificationsEvalutor<T> where T : BaseEntity
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecifications<T> specs)
        {
            /// _dbContext.Products.Where(P => P.Id == id).Include(P => P.Brand).Include(P =>
            /// P.Category).FirstOrDefaultAsync();

            var query = inputQuery;
            if (specs.Criteria is not null)
                query = query.Where(specs.Criteria);

            query = specs.Includes.Aggregate(query, (currentQuery, IncludeExpression) => currentQuery.Include(IncludeExpression));
            return query;
        }
    }
}
