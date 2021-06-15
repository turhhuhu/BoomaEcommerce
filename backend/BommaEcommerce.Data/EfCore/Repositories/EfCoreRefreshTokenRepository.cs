using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoomaEcommerce.Domain;
using Microsoft.EntityFrameworkCore;

namespace BoomaEcommerce.Data.EfCore.Repositories
{
    public class EfCoreRefreshTokenRepository : EfCoreRepository<RefreshToken>
    {
        public EfCoreRefreshTokenRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public override async Task InsertOneAsync(RefreshToken entity)
        {
            if (!await DbContext.RefreshTokens.AnyAsync(r => r.User.Guid == entity.User.Guid))
            {
                await base.InsertOneAsync(entity);
                await DbContext.SaveChangesAsync();
            }

        }
    }
}
