using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeveloperStore.Infra.Data.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.Property(p => p.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.Price)
            .IsRequired();

        builder.Property(p => p.Description)
            .IsRequired();

        builder.Property(p => p.Category)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.Image)
            .IsRequired()
            .HasMaxLength(50);

        builder.ComplexProperty(p => p.Rating, rating =>
        {
            rating.Property(r => r.Rate)
                .HasColumnName("Rating");

            rating.Property(r => r.Count)
                .HasColumnName("Count");
        });
    }
}
