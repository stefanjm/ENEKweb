using IdentityData;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ENEKservices {
    public class IdentityService : IApplicationUser {

        /// <summary>
        /// Database context
        /// </summary>
        private readonly IdentityDataDbContext _context;

        /// <summary>
        /// Initialize database context
        /// </summary>
        /// <param name="context"></param>
        public IdentityService(IdentityDataDbContext context) {
            _context = context;
        }

        /// <summary>
        /// Check if user with given user Id exists in the database
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> UserExistsById(string userId) {
            if (userId == null) {
                return false;
            }

            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(i => i.Id == userId);
            if(user == null) {
                return false;
            }
            else {
                return true;
            }
        }
    }
}
