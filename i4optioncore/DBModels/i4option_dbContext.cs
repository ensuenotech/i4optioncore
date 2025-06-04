using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace i4optioncore.DBModels
{
    public partial class i4option_dbContext : DbContext
    {
        public i4option_dbContext()
        {
        }

        public i4option_dbContext(DbContextOptions<i4option_dbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Breadth> Breadths { get; set; }
        public virtual DbSet<Calendar> Calendars { get; set; }
        public virtual DbSet<CalendarDate> CalendarDates { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }
        public virtual DbSet<EarningRatio> EarningRatios { get; set; }
        public virtual DbSet<Fullnse20220218> Fullnse20220218s { get; set; }
        public virtual DbSet<HistoryAuthToken> HistoryAuthTokens { get; set; }
        public virtual DbSet<HistoryRequest> HistoryRequests { get; set; }
        public virtual DbSet<HistorySubscription> HistorySubscriptions { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<Ivdatum> Ivdata { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<MaxPain> MaxPains { get; set; }
        public virtual DbSet<Rtdatum> Rtdata { get; set; }
        public virtual DbSet<Segment> Segments { get; set; }
        public virtual DbSet<SpotVolumeCommentary> SpotVolumeCommentaries { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<StockSegmentMapping> StockSegmentMappings { get; set; }
        public virtual DbSet<Symbol> Symbols { get; set; }
        public virtual DbSet<TouchlineSubscription> TouchlineSubscriptions { get; set; }
        public virtual DbSet<TrueData20220826> TrueData20220826s { get; set; }
        public virtual DbSet<VolumeCommentary> VolumeCommentaries { get; set; }
        public virtual DbSet<_20220826> _20220826s { get; set; }

       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Breadth>(entity =>
            {
                entity.ToTable("Breadth");

                entity.Property(e => e.Change).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ChangeWrtAtp).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.ChangeWrtOpen).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.LastTradeTime).HasColumnType("datetime");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Calendar>(entity =>
            {
                entity.ToTable("Calendar");

                entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<CalendarDate>(entity =>
            {
                entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Calendar)
                    .WithMany(p => p.CalendarDates)
                    .HasForeignKey(d => d.CalendarId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CalendarDates_Calendar");
            });

            modelBuilder.Entity<Configuration>(entity =>
            {
                entity.ToTable("Configuration");

                entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");

                entity.Property(e => e.Value).IsRequired();
            });

            modelBuilder.Entity<EarningRatio>(entity =>
            {
               

                entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<Fullnse20220218>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("FULLNSE_20220218");

                entity.Property(e => e.Column2)
                    .HasColumnType("date")
                    .HasColumnName("column2");

                entity.Property(e => e.Symbol).HasMaxLength(50);
            });

            modelBuilder.Entity<HistoryAuthToken>(entity =>
            {
                entity.ToTable("HistoryAuthToken");

                entity.Property(e => e.HistoryToken).IsRequired();

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<HistoryRequest>(entity =>
            {
                entity.Property(e => e.RequestOn).HasColumnType("datetime");

                entity.Property(e => e.Symbol).IsRequired();
            });

            modelBuilder.Entity<HistorySubscription>(entity =>
            {
                entity.Property(e => e.Atp)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("ATP");

                entity.Property(e => e.Close).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.High).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.LastTradeTime).HasColumnType("datetime");

                entity.Property(e => e.Low).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Open).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Symbol).IsRequired();

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<Holiday>(entity =>
            {
                entity.ToTable("Holiday");

                entity.Property(e => e.Date).HasColumnType("date");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Ivdatum>(entity =>
            {
                entity.ToTable("IVData");

                entity.Property(e => e.Ceiv)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("CEIV");

                entity.Property(e => e.Expiry).HasColumnType("datetime");

                entity.Property(e => e.Ivp)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("IVP");

                entity.Property(e => e.Ivr)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("IVR");

                entity.Property(e => e.Peiv)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("PEIV");

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<Log>(entity =>
            {
                entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");

                entity.Property(e => e.Log1)
                    .IsRequired()
                    .HasColumnName("Log");
            });

            modelBuilder.Entity<MaxPain>(entity =>
            {
                entity.ToTable("MaxPain");

                entity.Property(e => e.Expiry).HasColumnType("datetime");

                entity.Property(e => e.MaxPain1)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("MaxPain");

                entity.Property(e => e.Stock)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            modelBuilder.Entity<Rtdatum>(entity =>
            {
                entity.ToTable("RTData");

                entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<Segment>(entity =>
            {
                entity.ToTable("Segment");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<SpotVolumeCommentary>(entity =>
            {
                entity.ToTable("SpotVolumeCommentary");

                entity.Property(e => e.Atp)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("ATP");

                entity.Property(e => e.Close).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.High).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.LastTradeTime).HasColumnType("datetime");

                entity.Property(e => e.Low).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Open).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Symbol).IsRequired();
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.ToTable("Stock");

                entity.Property(e => e.Change).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");

                entity.Property(e => e.DisplayName).IsRequired();

                entity.Property(e => e.FreeFloat)
                    .HasColumnType("decimal(18, 2)")
                    .HasColumnName("FreeFLoat");

                entity.Property(e => e.MaxPain).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.MaxPainLastUpdatedUtc).HasColumnType("datetime");

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");

                entity.HasOne(d => d.Calendar)
                    .WithMany(p => p.Stocks)
                    .HasForeignKey(d => d.CalendarId)
                    .HasConstraintName("FK_Stock_Calendar");
            });

            modelBuilder.Entity<StockSegmentMapping>(entity =>
            {
                entity.ToTable("Stock_Segment_Mapping");
            });

            modelBuilder.Entity<Symbol>(entity =>
            {
                entity.Property(e => e.Exchange)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Expiry).HasColumnType("datetime");

                entity.Property(e => e.Isin)
                    .HasMaxLength(50)
                    .HasColumnName("ISIN");

                entity.Property(e => e.Segment).HasMaxLength(50);

                entity.Property(e => e.Series)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Strike).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Symbol1)
                    .IsRequired()
                    .HasColumnName("Symbol");

                entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<TouchlineSubscription>(entity =>
            {
                entity.Property(e => e.Atp)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("ATP");

                entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");

                entity.Property(e => e.High).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.LastUpdatedTime).HasColumnType("datetime");

                entity.Property(e => e.Low).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Ltp)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("LTP");

                entity.Property(e => e.Open).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.PreviousClose).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.PreviousOiclose)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("PreviousOIClose");

                entity.Property(e => e.Symbol).IsRequired();

                entity.Property(e => e.TickVolume).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.TodayOi)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("TodayOI");

                entity.Property(e => e.TurnOver).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
            });

            modelBuilder.Entity<TrueData20220826>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("TrueData_20220826");

                entity.Property(e => e.Ltd)
                    .HasColumnType("date")
                    .HasColumnName("LTD");

                entity.Property(e => e.Ltt).HasColumnName("LTT");

                entity.Property(e => e.Symbol).HasMaxLength(50);
            });

            modelBuilder.Entity<VolumeCommentary>(entity =>
            {
                entity.ToTable("VolumeCommentary");

                entity.Property(e => e.Atp)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("ATP");

                entity.Property(e => e.Close).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.High).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.LastTradeTime).HasColumnType("datetime");

                entity.Property(e => e.Low).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Open).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Symbol).IsRequired();
            });

            modelBuilder.Entity<_20220826>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("20220826");

                entity.Property(e => e.Atp)
                    .HasColumnType("decimal(18, 4)")
                    .HasColumnName("ATP");

                entity.Property(e => e.Close).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.High).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.Property(e => e.LastTradeTime).HasColumnType("datetime");

                entity.Property(e => e.Low).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Open).HasColumnType("decimal(18, 4)");

                entity.Property(e => e.Symbol).IsRequired();

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
