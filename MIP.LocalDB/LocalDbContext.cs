using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace MIP.LocalDB;

public class LocalDbContext : DbContext
{
	public DbSet<AnalysisAdminSearch> AnalysisAdminSearches { get; set; }

	public LocalDbContext() {}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		//base.OnConfiguring(optionsBuilder);
		string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
		string directory = Path.Combine(path, "MedicInPoint");
		Directory.CreateDirectory(directory);
		optionsBuilder.UseSqlite($"Data Source={Path.Combine(directory, "local.db")}");
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<AnalysisAdminSearch>(entity => {
			entity.ToTable("analysis_admin_search");
			entity.HasKey(a => a.id);
			entity.Property(a => a.name).IsRequired();
		});
	}
}

public class AnalysisAdminSearch
{
	[Key]
	public int id { get; set; }

	public string name { get; set; } = string.Empty;
}