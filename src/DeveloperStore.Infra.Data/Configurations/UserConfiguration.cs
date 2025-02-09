using DeveloperStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeveloperStore.Infra.Data.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        
        builder.Property(u => u.Id)
            .ValueGeneratedOnAdd()
            .UseIdentityColumn();

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property<string>(u => u.UserName)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.Password)
            .IsRequired()
            .HasMaxLength(12);

        builder.ComplexProperty(u => u.Name, name =>
        {
            name.Property(n => n.FirstName)
                .HasColumnName("FirstName")
                .IsRequired()
                .HasMaxLength(50);

            name.Property(n => n.LastName)
                .HasColumnName("LastName")
                .IsRequired()
                .HasMaxLength(50);
        });

        builder.ComplexProperty(u => u.Address, address =>
        {
            address.Property(a => a.City)
                .HasColumnName("City")
                .HasMaxLength(50);

            address.Property(a => a.Street)
                .HasColumnName("Street")
                .HasMaxLength(100);

            address.Property(a => a.Number)
                .HasColumnName("Number");

            address.Property(a => a.PostCode)
                .HasColumnName("PostCode");

            address.Property(g => g.Latitude)
                .HasColumnName("Latitude");

            address.Property(g => g.Longitude)
                .HasColumnName("Longitude");
        });

        builder.Property(u => u.Phone)
            .HasMaxLength(20);

        builder.Property(u => u.Status)
            .IsRequired();

        builder.Property(u => u.Role)
            .IsRequired();
    }
}