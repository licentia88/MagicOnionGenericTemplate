using MagicT.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicT.Server.Database;

public sealed class MagicTContext:DbContext
{
	public MagicTContext(DbContextOptions<MagicTContext> options):base(options)
	{
		Database.EnsureCreated();
	}

	public DbSet<TestModel> TestModel { get; set; }
}

