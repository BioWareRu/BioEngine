using BioEngine.Admin.Old.Entities;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace BioEngine.Admin.Old
{
    public class OldBrcContext : DbContext
    {
        public OldBrcContext(DbContextOptions<OldBrcContext> options) : base(options)
        {
        }

        public virtual DbSet<OldContentBlock> ContentBlocks { get; set; }
        public virtual DbSet<OldFacebookPublishRecord> FacebookPublishRecords { get; set; }
        public virtual DbSet<OldIpbComment> IpbComments { get; set; }
        public virtual DbSet<OldIpbPublishRecord> IpbPublishRecords { get; set; }
        public virtual DbSet<OldMenu> Menus { get; set; }
        public virtual DbSet<OldPage> Pages { get; set; }
        public virtual DbSet<OldPost> Posts { get; set; }
        public virtual DbSet<OldSection> Sections { get; set; }
        public virtual DbSet<OldSite> Sites { get; set; }
        public virtual DbSet<OldStorageItem> StorageItems { get; set; }
        public virtual DbSet<OldTag> Tags { get; set; }
        public virtual DbSet<OldTwitterPublishRecord> TwitterPublishRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "en_US.utf8");


            modelBuilder.Entity<OldContentBlock>(entity =>
            {
                entity.HasIndex(e => e.ContentId, "IX_ContentBlocks_ContentId");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("'2019-07-02 05:14:25.6692+00'::timestamp with time zone");

                entity.Property(e => e.DateUpdated)
                    .HasColumnType("timestamp with time zone")
                    .HasDefaultValueSql("'2019-07-02 05:14:25.669677+00'::timestamp with time zone");

                entity.Property(e => e.Type).IsRequired();
            });


            modelBuilder.Entity<OldFacebookPublishRecord>(entity =>
            {
                entity.ToTable("FacebookPublishRecord");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.PostId).IsRequired();

                entity.Property(e => e.SiteIds).IsRequired();

                entity.Property(e => e.Type).IsRequired();
            });


            modelBuilder.Entity<OldIpbComment>(entity =>
            {
                entity.ToTable("IPBComment");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AuthorId).IsRequired();

                entity.Property(e => e.ContentType).IsRequired();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.SiteIds).IsRequired();
            });

            modelBuilder.Entity<OldIpbPublishRecord>(entity =>
            {
                entity.ToTable("IPBPublishRecord");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.SiteIds).IsRequired();

                entity.Property(e => e.Type).IsRequired();
            });


            modelBuilder.Entity<OldMenu>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Items)
                    .IsRequired();

                entity.Property(e => e.SiteIds).IsRequired();

                entity.Property(e => e.Title).IsRequired();
            });

            modelBuilder.Entity<OldPage>(entity =>
            {
                entity.HasIndex(e => e.IsPublished, "IX_Pages_IsPublished");

                entity.HasIndex(e => e.SiteIds, "IX_Pages_SiteIds");

                entity.HasIndex(e => e.Url, "IX_Pages_Url")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DatePublished).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.SiteIds).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Url).IsRequired();
            });

            modelBuilder.Entity<OldPost>(entity =>
            {
                entity.HasIndex(e => e.IsPublished, "IX_Posts_IsPublished");

                entity.HasIndex(e => e.SectionIds, "IX_Posts_SectionIds");

                entity.HasIndex(e => e.SiteIds, "IX_Posts_SiteIds");

                entity.HasIndex(e => e.TagIds, "IX_Posts_TagIds");

                entity.HasIndex(e => e.Url, "IX_Posts_Url")
                    .IsUnique();

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AuthorId).IsRequired();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DatePublished).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.SectionIds).IsRequired();

                entity.Property(e => e.SiteIds).IsRequired();

                entity.Property(e => e.TagIds).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Url).IsRequired();
            });

            modelBuilder.Entity<OldSection>(entity =>
            {
                entity.HasIndex(e => e.IsPublished, "IX_Sections_IsPublished");

                entity.HasIndex(e => e.SiteIds, "IX_Sections_SiteIds");

                entity.HasIndex(e => e.Type, "IX_Sections_Type");

                entity.HasIndex(e => e.Url, "IX_Sections_Url");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DatePublished).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.SiteIds).IsRequired();

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Type).IsRequired();

                entity.Property(e => e.Url).IsRequired();
            });


            modelBuilder.Entity<OldSite>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.Title).IsRequired();

                entity.Property(e => e.Url).IsRequired();
            });


            modelBuilder.Entity<OldStorageItem>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.FileName).IsRequired();

                entity.Property(e => e.FilePath).IsRequired();

                entity.Property(e => e.Path).IsRequired();

                entity.Property(e => e.PublicUri).IsRequired();
            });

            modelBuilder.Entity<OldTag>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");
            });

            modelBuilder.Entity<OldTwitterPublishRecord>(entity =>
            {
                entity.ToTable("TwitterPublishRecord");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.DateAdded).HasColumnType("timestamp with time zone");

                entity.Property(e => e.DateUpdated).HasColumnType("timestamp with time zone");

                entity.Property(e => e.SiteIds).IsRequired();

                entity.Property(e => e.Type).IsRequired();
            });
        }
    }
}
