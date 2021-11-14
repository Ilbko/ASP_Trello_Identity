using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace ASP_Trello_Identity.Data
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ApplicationUser> AspNetUsers { get; set; }
        public virtual DbSet<Board> Boards { get; set; }
        public virtual DbSet<Card> Cards { get; set; }
        public virtual DbSet<List> Lists { get; set; }
        public virtual DbSet<UserBoard> UserBoards { get; set; }
        public virtual DbSet<UserWorkspace> UserWorkspaces { get; set; }
        public virtual DbSet<Workspace> Workspaces { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
