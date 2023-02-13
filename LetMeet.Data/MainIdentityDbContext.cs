using LetMeet.Data.Entites.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LetMeet.Data
{
    public class MainIdentityDbContext :IdentityDbContext<AppIdentityUser, AppIdentityRole, Guid> { 
        public MainIdentityDbContext(DbContextOptions<MainIdentityDbContext> options) : base(options)
        {
        }

    }
}
