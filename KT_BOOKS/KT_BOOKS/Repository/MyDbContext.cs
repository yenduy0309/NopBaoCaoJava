using KT_BOOKS.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace KT_BOOKS.Repository
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }
        public DbSet<BookModel> Books { get; set; } 
        public DbSet<AuthorModel> Authors { get; set; } 

        public DbSet<AccountModel> Accounts { get; set; }

        public DbSet<RoleModel> Roles { get; set; }
       
    }
}
