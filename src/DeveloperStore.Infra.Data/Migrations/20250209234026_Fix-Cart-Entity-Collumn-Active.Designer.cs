﻿// <auto-generated />
using System;
using System.Collections.Generic;
using DeveloperStore.Infra.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DeveloperStore.Infra.Data.Migrations
{
    [DbContext(typeof(DeveloperStoreDbContext))]
    [Migration("20250209234026_Fix-Cart-Entity-Collumn-Active")]
    partial class FixCartEntityCollumnActive
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("public")
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DeveloperStore.Domain.Entities.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Active")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("carts", "public");
                });

            modelBuilder.Entity("DeveloperStore.Domain.Entities.CartItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CartId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId");

                    b.ToTable("cartitems", "public");
                });

            modelBuilder.Entity("DeveloperStore.Domain.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("character varying(50)");

                    b.ComplexProperty<Dictionary<string, object>>("Rating", "DeveloperStore.Domain.Entities.Product.Rating#Rating", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<int?>("Count")
                                .HasColumnType("integer")
                                .HasColumnName("Count");

                            b1.Property<decimal?>("Rate")
                                .HasColumnType("numeric")
                                .HasColumnName("Rating");
                        });

                    b.HasKey("Id");

                    b.ToTable("products", "public");
                });

            modelBuilder.Entity("DeveloperStore.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(12)
                        .IsUnicode(false)
                        .HasColumnType("character varying(12)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character varying(20)");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("character varying(20)");

                    b.ComplexProperty<Dictionary<string, object>>("Address", "DeveloperStore.Domain.Entities.User.Address#Address", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(50)
                                .IsUnicode(false)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("City");

                            b1.Property<string>("Latitude")
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("character varying(255)")
                                .HasColumnName("Latitude");

                            b1.Property<string>("Longitude")
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("character varying(255)")
                                .HasColumnName("Longitude");

                            b1.Property<int>("Number")
                                .HasColumnType("integer")
                                .HasColumnName("Number");

                            b1.Property<string>("PostCode")
                                .IsRequired()
                                .HasMaxLength(255)
                                .IsUnicode(false)
                                .HasColumnType("character varying(255)")
                                .HasColumnName("PostCode");

                            b1.Property<string>("Street")
                                .IsRequired()
                                .HasMaxLength(100)
                                .IsUnicode(false)
                                .HasColumnType("character varying(100)")
                                .HasColumnName("Street");
                        });

                    b.ComplexProperty<Dictionary<string, object>>("Name", "DeveloperStore.Domain.Entities.User.Name#Name", b1 =>
                        {
                            b1.IsRequired();

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasMaxLength(50)
                                .IsUnicode(false)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("FirstName");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasMaxLength(50)
                                .IsUnicode(false)
                                .HasColumnType("character varying(50)")
                                .HasColumnName("LastName");
                        });

                    b.HasKey("Id");

                    b.ToTable("users", "public");
                });

            modelBuilder.Entity("DeveloperStore.Domain.Entities.Cart", b =>
                {
                    b.HasOne("DeveloperStore.Domain.Entities.User", "User")
                        .WithMany("Carts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DeveloperStore.Domain.Entities.CartItem", b =>
                {
                    b.HasOne("DeveloperStore.Domain.Entities.Cart", "Cart")
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DeveloperStore.Domain.Entities.Product", "Product")
                        .WithMany("CartItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cart");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("DeveloperStore.Domain.Entities.Cart", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("DeveloperStore.Domain.Entities.Product", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("DeveloperStore.Domain.Entities.User", b =>
                {
                    b.Navigation("Carts");
                });
#pragma warning restore 612, 618
        }
    }
}
