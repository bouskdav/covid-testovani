using System;
using System.Collections.Generic;
using System.Text;
using FiremniTestovani.Data.Tables;
using FiremniTestovani.Data.Views;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FiremniTestovani.Data.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        #region tables
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<TimeSlot> TimeSlots { get; set; }
        public virtual DbSet<TimeSlotBooking> TimeSlotBookings { get; set; }
        #endregion

        #region views
        public virtual DbSet<DateCapacity> DateCapacities { get; set; }
        public virtual DbSet<DateOccupancy> DateOccupancies { get; set; }
        public virtual DbSet<DateOverview> DateOverviews { get; set; }
        public virtual DbSet<TimeSlotOccupancy> TimeSlotOccupancies { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<DateCapacity>(
                    eb =>
                    {
                        eb.HasNoKey();
                        eb.ToView("DateCapacity");
                    });

            builder
                .Entity<DateOccupancy>(
                    eb =>
                    {
                        eb.HasNoKey();
                        eb.ToView("DateOccupancy");
                    });

            builder
                .Entity<DateOverview>(
                    eb => 
                    {
                        eb.HasNoKey();
                        eb.ToView("DateOverview");
                    });

            builder
                .Entity<TimeSlotOccupancy>(
                    eb =>
                    {
                        eb.HasNoKey();
                        eb.ToView("TimeSlotOccupancy");
                    });

            builder.Entity<Settings>().HasKey(table => new { 
                table.SourceID,
                table.Name
            });

            base.OnModelCreating(builder);
        }
    }
}
