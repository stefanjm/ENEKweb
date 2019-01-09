using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IdentityData {
    public interface IApplicationUser {

        /// <summary>
        /// Checks if user with given Id exists in the database. 
        /// Mostly used for checking if user is valid before allowing changes to the database.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>True if user exists</returns>
        Task<bool> UserExistsById(string userId);

    }
}
