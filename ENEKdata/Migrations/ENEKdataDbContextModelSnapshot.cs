﻿// <auto-generated />
using System;
using ENEKdata;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ENEKdata.Migrations
{
    [DbContext(typeof(ENEKdataDbContext))]
    partial class ENEKdataDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ENEKdata.Models.Leiunurk.Item", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(95);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25);

                    b.Property<decimal>("Price");

                    b.HasKey("Id");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("ENEKdata.Models.Leiunurk.ItemImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ImageFileName")
                        .IsRequired();

                    b.Property<int?>("ItemId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ItemId");

                    b.ToTable("ItemImages");
                });

            modelBuilder.Entity("ENEKdata.Models.Partnerid.Partner", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Partners");
                });

            modelBuilder.Entity("ENEKdata.Models.Partnerid.PartnerImage", b =>
                {
                    b.Property<int>("Id");

                    b.Property<string>("ImageFileName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("PartnerImages");
                });

            modelBuilder.Entity("ENEKdata.Models.Leiunurk.ItemImage", b =>
                {
                    b.HasOne("ENEKdata.Models.Leiunurk.Item", "Item")
                        .WithMany("Images")
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ENEKdata.Models.Partnerid.PartnerImage", b =>
                {
                    b.HasOne("ENEKdata.Models.Partnerid.Partner", "Partner")
                        .WithOne("Image")
                        .HasForeignKey("ENEKdata.Models.Partnerid.PartnerImage", "Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
