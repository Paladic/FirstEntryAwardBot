﻿// <auto-generated />
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AwardsBotContext))]
    [Migration("20220528155649_1thMigration")]
    partial class _1thMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Infrastructure.Models.KeysGift", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int unsigned");

                    b.Property<ulong>("ActivationAt")
                        .HasColumnType("bigint unsigned");

                    b.Property<ulong>("ActivationBy")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("Gift")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<ulong>("ServerId")
                        .HasColumnType("bigint unsigned");

                    b.HasKey("Id");

                    b.ToTable("KeyGifts");
                });
#pragma warning restore 612, 618
        }
    }
}
