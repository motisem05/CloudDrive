using CloudDrive.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudDrive.Persistence
{
	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
		}

		public DbSet<Data> Data { get; set; }
		public DbSet<Notebook> Notebooks { get; set; }
		public DbSet<Notes> Notes { get; set; }
		public DbSet<UserPassword> UserPasswords { get; set; }
		public DbSet<Audit> Audit { get; set; }
		public DbSet<UserCreditCard> UserCreditCards { get; set; }
	}
}
