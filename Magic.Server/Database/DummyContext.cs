using Magic.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace Magic.Server.Database;

public class DummyContext:DbContext
{
	public DummyContext(DbContextOptions<DummyContext> options):base(options)
	{
		Database.EnsureCreated();
	}

	public DbSet<TestModel> TestModel { get; set; }
}

