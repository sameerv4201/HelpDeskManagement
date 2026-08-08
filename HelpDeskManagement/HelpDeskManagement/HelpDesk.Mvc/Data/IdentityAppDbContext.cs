using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Mvc.Data
{
    /// <summary>
    /// Stores login/role data only. Ticket data still lives entirely behind
    /// HelpDesk.Api and is never accessed directly from this context.
    /// </summary>
    public class IdentityAppDbContext : IdentityDbContext<IdentityUser>
    {
        public IdentityAppDbContext(DbContextOptions<IdentityAppDbContext> options) : base(options)
        {
        }
    }
}
