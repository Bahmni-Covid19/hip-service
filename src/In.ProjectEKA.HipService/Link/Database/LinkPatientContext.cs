using System.Collections.Generic;
using In.ProjectEKA.HipService.Link.Model;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace In.ProjectEKA.HipService.Link.Database
{
    public class LinkPatientContext : DbContext
    {
        public LinkPatientContext(DbContextOptions<LinkPatientContext> options) : base(options)
        {
        }

        public DbSet<LinkEnquires> LinkEnquires { get; set; }

        public DbSet<LinkedAccounts> LinkedAccounts { get; set; }

        public DbSet<CareContextMap> CareContextMap { get; set; }

        public DbSet<InitiatedLinkRequest> InitiatedLinkRequest { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CareContext>()
                .HasKey("CareContextName", "LinkReferenceNumber")
                .HasName("Id");

            modelBuilder.Entity<LinkEnquires>()
                .HasMany(l => l.CareContexts)
                .WithOne(c => c.LinkEnquires)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LinkedAccounts>(builder =>
            {
                builder.Property(p => p.DateTimeStamp)
                    .HasDefaultValueSql("now()");
            });

            modelBuilder.Entity<CareContextMap>(builder =>
            {
                builder.HasKey(p=>p.CareContextName);
                builder.Property(p => p.CareContextName);
                builder.Property(p => p.CareContextType);
            });

            modelBuilder.Entity<InitiatedLinkRequest>(builder =>
            {
                builder.Property(p => p.DateTimeStamp)
                    .HasDefaultValueSql("now()");
                builder.Property(p => p.Status)
                    .HasDefaultValueSql("false");
            });

            modelBuilder.Entity<LinkedAccounts>()
                .Property(e => e.CareContexts)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}),
                    v => JsonConvert.DeserializeObject<List<string>>(v,
                        new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
        }
    }
}