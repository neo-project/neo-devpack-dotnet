using Microsoft.EntityFrameworkCore;
using R3E.WebGUI.Service.Domain.Models;
using System.Text.Json;

namespace R3E.WebGUI.Service.Infrastructure.Data;

public class WebGUIDbContext : DbContext
{
    public WebGUIDbContext(DbContextOptions<WebGUIDbContext> options) : base(options)
    {
    }

    public DbSet<ContractWebGUI> WebGUIs { get; set; }
    public DbSet<ContractManifestInfo> ContractManifests { get; set; }
    public DbSet<ContractMethod> ContractMethods { get; set; }
    public DbSet<ContractParameter> ContractParameters { get; set; }
    public DbSet<ContractEvent> ContractEvents { get; set; }
    public DbSet<ContractEventParameter> ContractEventParameters { get; set; }
    public DbSet<ContractAssetInfo> ContractAssets { get; set; }
    public DbSet<ContractBalance> ContractBalances { get; set; }
    public DbSet<ContractTransaction> ContractTransactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContractWebGUI>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ContractAddress)
                .IsRequired()
                .HasMaxLength(42); // 0x + 40 hex chars
                
            entity.Property(e => e.ContractName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Network)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.Subdomain)
                .IsRequired()
                .HasMaxLength(63); // DNS subdomain max length
                
            entity.Property(e => e.Description)
                .HasMaxLength(500);
                
            entity.Property(e => e.DeployerAddress)
                .IsRequired()
                .HasMaxLength(42);
                
            entity.Property(e => e.WebGUIVersion)
                .HasMaxLength(20);
                
            entity.Property(e => e.StoragePath)
                .IsRequired()
                .HasMaxLength(255);

            // JSON conversion for Metadata and new collections
            entity.Property(e => e.Metadata)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, object>>(v, JsonSerializerOptions.Default) ?? new Dictionary<string, object>());

            entity.Property(e => e.SupportedWallets)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>());

            entity.Property(e => e.ThemeSettings)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                    v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonSerializerOptions.Default) ?? new Dictionary<string, string>());

            // Relationships
            entity.HasOne(e => e.ContractManifest)
                .WithMany()
                .HasForeignKey(e => e.ContractManifestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.ContractAddress);
            entity.HasIndex(e => e.Subdomain).IsUnique();
            entity.HasIndex(e => e.Network);
            entity.HasIndex(e => new { e.ContractAddress, e.Network });
            entity.HasIndex(e => e.DeployedAt);
        });

        // ContractManifestInfo configuration
        modelBuilder.Entity<ContractManifestInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ContractAddress)
                .IsRequired()
                .HasMaxLength(42);
                
            entity.Property(e => e.Network)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Author)
                .HasMaxLength(100);
                
            entity.Property(e => e.Email)
                .HasMaxLength(100);
                
            entity.Property(e => e.Description)
                .HasMaxLength(1000);
                
            entity.Property(e => e.Version)
                .HasMaxLength(20);

            entity.Property(e => e.ManifestJson)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            // Relationships
            entity.HasMany(e => e.Methods)
                .WithOne(m => m.ContractManifest)
                .HasForeignKey(m => m.ContractManifestId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Events)
                .WithOne(ev => ev.ContractManifest)
                .HasForeignKey(ev => ev.ContractManifestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => new { e.ContractAddress, e.Network }).IsUnique();
            entity.HasIndex(e => e.FetchedAt);
        });

        // ContractMethod configuration
        modelBuilder.Entity<ContractMethod>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.ReturnType)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.Description)
                .HasMaxLength(500);

            // Relationships
            entity.HasMany(e => e.Parameters)
                .WithOne(p => p.ContractMethod)
                .HasForeignKey(p => p.ContractMethodId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.ContractManifestId);
            entity.HasIndex(e => e.Name);
        });

        // ContractParameter configuration
        modelBuilder.Entity<ContractParameter>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.DefaultValue)
                .HasMaxLength(200);
                
            entity.Property(e => e.Description)
                .HasMaxLength(500);

            // Indexes
            entity.HasIndex(e => e.ContractMethodId);
            entity.HasIndex(e => e.Order);
        });

        // ContractEvent configuration
        modelBuilder.Entity<ContractEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Description)
                .HasMaxLength(500);

            // Relationships
            entity.HasMany(e => e.Parameters)
                .WithOne(p => p.ContractEvent)
                .HasForeignKey(p => p.ContractEventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.ContractManifestId);
            entity.HasIndex(e => e.Name);
        });

        // ContractEventParameter configuration
        modelBuilder.Entity<ContractEventParameter>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50);

            // Indexes
            entity.HasIndex(e => e.ContractEventId);
            entity.HasIndex(e => e.Order);
        });

        // ContractAssetInfo configuration
        modelBuilder.Entity<ContractAssetInfo>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ContractAddress)
                .IsRequired()
                .HasMaxLength(42);
                
            entity.Property(e => e.Network)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.AssetHash)
                .IsRequired()
                .HasMaxLength(42);
                
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.Name)
                .HasMaxLength(100);
                
            entity.Property(e => e.TotalSupply)
                .HasMaxLength(50);
                
            entity.Property(e => e.AssetType)
                .HasMaxLength(20);

            // Indexes
            entity.HasIndex(e => new { e.ContractAddress, e.Network });
            entity.HasIndex(e => e.AssetHash);
            entity.HasIndex(e => e.Symbol);
        });

        // ContractBalance configuration
        modelBuilder.Entity<ContractBalance>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ContractAddress)
                .IsRequired()
                .HasMaxLength(42);
                
            entity.Property(e => e.Network)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.AssetHash)
                .IsRequired()
                .HasMaxLength(42);
                
            entity.Property(e => e.Balance)
                .IsRequired()
                .HasMaxLength(50);

            // Relationships
            entity.HasOne(e => e.AssetInfo)
                .WithMany()
                .HasForeignKey(e => e.AssetHash)
                .HasPrincipalKey(a => a.AssetHash)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => new { e.ContractAddress, e.Network, e.AssetHash }).IsUnique();
            entity.HasIndex(e => e.LastUpdated);
        });

        // ContractTransaction configuration
        modelBuilder.Entity<ContractTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ContractAddress)
                .IsRequired()
                .HasMaxLength(42);
                
            entity.Property(e => e.Network)
                .IsRequired()
                .HasMaxLength(20);
                
            entity.Property(e => e.TransactionHash)
                .IsRequired()
                .HasMaxLength(66); // 0x + 64 hex chars
                
            entity.Property(e => e.BlockHash)
                .IsRequired()
                .HasMaxLength(66);
                
            entity.Property(e => e.FromAddress)
                .HasMaxLength(42);
                
            entity.Property(e => e.ToAddress)
                .HasMaxLength(42);
                
            entity.Property(e => e.Method)
                .HasMaxLength(100);
                
            entity.Property(e => e.Parameters)
                .HasColumnType("nvarchar(max)");
                
            entity.Property(e => e.Result)
                .HasColumnType("nvarchar(max)");
                
            entity.Property(e => e.GasConsumed)
                .HasMaxLength(50);
                
            entity.Property(e => e.Exception)
                .HasColumnType("nvarchar(max)");

            // Indexes
            entity.HasIndex(e => e.TransactionHash).IsUnique();
            entity.HasIndex(e => new { e.ContractAddress, e.Network });
            entity.HasIndex(e => e.BlockIndex);
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.Method);
        });

        base.OnModelCreating(modelBuilder);
    }
}