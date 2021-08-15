using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BioEngine.Core.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Sitko.Blockly.EntityFrameworkCore;
using Sitko.Core.Db.Postgres;

namespace BioEngine.Core.Data
{
    public class BioDbContext : DbContext
    {
        public DbSet<Section> Sections => Set<Section>();
        public DbSet<Game> Games => Set<Game>();
        public DbSet<Developer> Developers => Set<Developer>();
        public DbSet<Topic> Topics => Set<Topic>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Site> Sites => Set<Site>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Page> Pages => Set<Page>();

        public BioDbContext(DbContextOptions<BioDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Section>()
                .HasDiscriminator(section => section.Type)
                .HasValue<Developer>(SectionType.Developer)
                .HasValue<Game>(SectionType.Game)
                .HasValue<Topic>(SectionType.Topic);

            modelBuilder.RegisterBlocklyConversion<Section>(section => section.Blocks, nameof(Section.Blocks));
            modelBuilder.RegisterJsonConversion<Developer, DeveloperData>(developer => developer.Data, "Data");
            modelBuilder.RegisterJsonConversion<Game, GameData>(game => game.Data, "Data");
            modelBuilder.RegisterJsonConversion<Topic, TopicData>(topic => topic.Data, "Data");


            modelBuilder.RegisterBlocklyConversion<Post>(post => post.Blocks, "Blocks");
            modelBuilder.RegisterBlocklyConversion<Page>(page => page.Blocks, "Blocks");

            modelBuilder
                .Entity<Post>()
                .HasMany(p => p.Tags)
                .WithMany(p => p.Posts)
                .UsingEntity(j => j.ToTable("PostTags"));

            modelBuilder
                .Entity<Post>()
                .HasMany(p => p.Sections)
                .WithMany(p => p.Posts)
                .UsingEntity(j => j.ToTable("PostSections"));

            modelBuilder
                .Entity<Section>()
                .HasMany(p => p.Sites)
                .WithMany(p => p.Sections)
                .UsingEntity(j => j.ToTable("SectionSites"));

            modelBuilder
                .Entity<Page>()
                .HasMany(p => p.Sites)
                .WithMany(p => p.Pages)
                .UsingEntity(j => j.ToTable("PageSites"));
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void RegisterJsonEnumerableConversion<TEntity, TData, TCollection>(this ModelBuilder modelBuilder,
            Expression<Func<TEntity, TCollection>> getProperty, string name)
            where TEntity : class where TCollection : IEnumerable<TData>
        {
            var valueComparer = new ValueComparer<TCollection>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v!.GetHashCode())),
                c => Deserialize<TCollection>(Serialize(c)));
            modelBuilder
                .Entity<TEntity>()
                .Property(getProperty)
                .HasColumnType("jsonb")
                .HasColumnName(name)
                .HasConversion(data => Serialize(data),
                    json => Deserialize<TCollection>(json))
                .Metadata.SetValueComparer(valueComparer);
        }

        private static readonly JsonSerializerSettings _jsonSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
        };

        private static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _jsonSettings);
        }

        private static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSettings);
        }
    }
}
