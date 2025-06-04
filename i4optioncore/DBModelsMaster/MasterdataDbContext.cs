using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace i4optioncore.DBModelsMaster;

public partial class MasterdataDbContext : DbContext
{
    public MasterdataDbContext()
    {
    }

    public MasterdataDbContext(DbContextOptions<MasterdataDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cashflow> Cashflows { get; set; }

    public virtual DbSet<DayPcr> DayPcrs { get; set; }

    public virtual DbSet<Eod> Eods { get; set; }

    public virtual DbSet<FaoParticipantOi> FaoParticipantOis { get; set; }

    public virtual DbSet<FaoParticipantVol> FaoParticipantVols { get; set; }

    public virtual DbSet<FiiStat> FiiStats { get; set; }

    public virtual DbSet<FutureRollover> FutureRollovers { get; set; }

    public virtual DbSet<HistoricPerformance> HistoricPerformances { get; set; }

    public virtual DbSet<History012021> History012021s { get; set; }

    public virtual DbSet<History012022> History012022s { get; set; }

    public virtual DbSet<History012023> History012023s { get; set; }

    public virtual DbSet<History012024> History012024s { get; set; }

    public virtual DbSet<History012025> History012025s { get; set; }

    public virtual DbSet<History022021> History022021s { get; set; }

    public virtual DbSet<History022022> History022022s { get; set; }

    public virtual DbSet<History022023> History022023s { get; set; }

    public virtual DbSet<History022024> History022024s { get; set; }

    public virtual DbSet<History022025> History022025s { get; set; }

    public virtual DbSet<History032021> History032021s { get; set; }

    public virtual DbSet<History032022> History032022s { get; set; }

    public virtual DbSet<History032023> History032023s { get; set; }

    public virtual DbSet<History032024> History032024s { get; set; }

    public virtual DbSet<History032025> History032025s { get; set; }

    public virtual DbSet<History042021> History042021s { get; set; }

    public virtual DbSet<History042022> History042022s { get; set; }

    public virtual DbSet<History042023> History042023s { get; set; }

    public virtual DbSet<History042024> History042024s { get; set; }

    public virtual DbSet<History042025> History042025s { get; set; }

    public virtual DbSet<History052021> History052021s { get; set; }

    public virtual DbSet<History052022> History052022s { get; set; }

    public virtual DbSet<History052023> History052023s { get; set; }

    public virtual DbSet<History052024> History052024s { get; set; }

    public virtual DbSet<History052025> History052025s { get; set; }

    public virtual DbSet<History062021> History062021s { get; set; }

    public virtual DbSet<History062022> History062022s { get; set; }

    public virtual DbSet<History062023> History062023s { get; set; }

    public virtual DbSet<History062024> History062024s { get; set; }

    public virtual DbSet<History062025> History062025s { get; set; }

    public virtual DbSet<History072021> History072021s { get; set; }

    public virtual DbSet<History072022> History072022s { get; set; }

    public virtual DbSet<History072023> History072023s { get; set; }

    public virtual DbSet<History072024> History072024s { get; set; }

    public virtual DbSet<History072025> History072025s { get; set; }

    public virtual DbSet<History082021> History082021s { get; set; }

    public virtual DbSet<History082022> History082022s { get; set; }

    public virtual DbSet<History082023> History082023s { get; set; }

    public virtual DbSet<History082024> History082024s { get; set; }

    public virtual DbSet<History082025> History082025s { get; set; }

    public virtual DbSet<History092021> History092021s { get; set; }

    public virtual DbSet<History092022> History092022s { get; set; }

    public virtual DbSet<History092023> History092023s { get; set; }

    public virtual DbSet<History092024> History092024s { get; set; }

    public virtual DbSet<History092025> History092025s { get; set; }

    public virtual DbSet<History102021> History102021s { get; set; }

    public virtual DbSet<History102022> History102022s { get; set; }

    public virtual DbSet<History102023> History102023s { get; set; }

    public virtual DbSet<History102024> History102024s { get; set; }

    public virtual DbSet<History102025> History102025s { get; set; }

    public virtual DbSet<History112021> History112021s { get; set; }

    public virtual DbSet<History112022> History112022s { get; set; }

    public virtual DbSet<History112023> History112023s { get; set; }

    public virtual DbSet<History112024> History112024s { get; set; }

    public virtual DbSet<History112025> History112025s { get; set; }

    public virtual DbSet<History122021> History122021s { get; set; }

    public virtual DbSet<History122022> History122022s { get; set; }

    public virtual DbSet<History122023> History122023s { get; set; }

    public virtual DbSet<History122024> History122024s { get; set; }

    public virtual DbSet<History122025> History122025s { get; set; }

    public virtual DbSet<HistorySubscription> HistorySubscriptions { get; set; }

    public virtual DbSet<MtoStat> MtoStats { get; set; }

    public virtual DbSet<Pcr> Pcrs { get; set; }

    public virtual DbSet<Sector> Sectors { get; set; }

    public virtual DbSet<SectorInvestment> SectorInvestments { get; set; }

    public virtual DbSet<SegmentTouchline> SegmentTouchlines { get; set; }

    public virtual DbSet<Tl2007> Tl2007s { get; set; }

    public virtual DbSet<TouchlineSubscription> TouchlineSubscriptions { get; set; }

    public virtual DbSet<TouchlineSubscriptions2018> TouchlineSubscriptions2018s { get; set; }

    public virtual DbSet<TouchlineSubscriptions2019> TouchlineSubscriptions2019s { get; set; }

    public virtual DbSet<TouchlineSubscriptions2020> TouchlineSubscriptions2020s { get; set; }

    public virtual DbSet<TouchlineSubscriptions2021> TouchlineSubscriptions2021s { get; set; }

    public virtual DbSet<TouchlineSubscriptions2022> TouchlineSubscriptions2022s { get; set; }

    public virtual DbSet<TouchlineSubscriptions2023> TouchlineSubscriptions2023s { get; set; }

    public virtual DbSet<TouchlineSubscriptions2024> TouchlineSubscriptions2024s { get; set; }

    public virtual DbSet<TouchlineSubscriptions2025> TouchlineSubscriptions2025s { get; set; }

    public virtual DbSet<TouchlineSubscriptionsStock> TouchlineSubscriptionsStocks { get; set; }

    public virtual DbSet<Volatility> Volatilities { get; set; }

    public virtual DbSet<ZerodhaHistory> ZerodhaHistories { get; set; }

    public virtual DbSet<_52weekHighLow> _52weekHighLows { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("data source=13.202.249.163;initial catalog=masterdata_db;Persist Security Info=True;User ID=sa;Password='io#123321';App=EntityFramework;MultipleActiveResultSets=true;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cashflow>(entity =>
        {
            entity.ToTable("Cashflow");

            entity.Property(e => e.BuyValueDii)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("BuyValueDII");
            entity.Property(e => e.BuyValueFii)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("BuyValueFII");
            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.NetValueDii)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("NetValueDII");
            entity.Property(e => e.NetValueFii)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("NetValueFII");
            entity.Property(e => e.SellValueDii)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SellValueDII");
            entity.Property(e => e.SellValueFii)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("SellValueFII");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Year).HasMaxLength(50);
        });

        modelBuilder.Entity<DayPcr>(entity =>
        {
            entity.ToTable("DayPCR");

            entity.Property(e => e.Ceoi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CEOI");
            entity.Property(e => e.Cevolume).HasColumnName("CEVolume");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Expiry).HasColumnType("datetime");
            entity.Property(e => e.Oipcr)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("OIPCR");
            entity.Property(e => e.Oisum)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("OISum");
            entity.Property(e => e.Peoi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PEOI");
            entity.Property(e => e.Pevolume).HasColumnName("PEVolume");
            entity.Property(e => e.Stock)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.VolumePcr)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("VolumePCR");
        });

        modelBuilder.Entity<Eod>(entity =>
        {
            entity.ToTable("EOD");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Davol10)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DAVOL10");
            entity.Property(e => e.Davol20)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DAVOL20");
            entity.Property(e => e.Davol5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DAVOL5");
            entity.Property(e => e.Day).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DayGap).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Ddel10)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DDEL10");
            entity.Property(e => e.Ddel20)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DDEL20");
            entity.Property(e => e.Ddel5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DDEL5");
            entity.Property(e => e.Dsma10)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DSMA10");
            entity.Property(e => e.Dsma100)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DSMA100");
            entity.Property(e => e.Dsma20)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DSMA20");
            entity.Property(e => e.Dsma200)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DSMA200");
            entity.Property(e => e.Dsma50)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DSMA50");
            entity.Property(e => e.Ltp)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("LTP");
            entity.Property(e => e.Month).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Month3).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Month6).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Stock)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Week).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Year).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.YearHigh).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.YearHighDate).HasColumnType("datetime");
            entity.Property(e => e.YearLow).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.YearLowDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<FaoParticipantOi>(entity =>
        {
            entity.ToTable("FAO_Participant_OI");

            entity.Property(e => e.ClientType).IsRequired();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.FutureIndexLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FutureIndexShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FutureStockLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FutureStockShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionIndexCallLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionIndexCallShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionIndexPutLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionIndexPutShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionStockCallLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionStockCallShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionStockPutLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionStockPutShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalLongContracts).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalShortContracts).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<FaoParticipantVol>(entity =>
        {
            entity.ToTable("FAO_Participant_Vol");

            entity.Property(e => e.ClientType).IsRequired();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.FutureIndexLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FutureIndexShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FutureStockLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.FutureStockShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionIndexCallLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionIndexCallShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionIndexPutLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionIndexPutShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionStockCallLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionStockCallShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionStockPutLong).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.OptionStockPutShort).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalLongContracts).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.TotalShortContracts).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<FiiStat>(entity =>
        {
            entity.ToTable("FII_Stats");

            entity.Property(e => e.BuyAmtInCrores).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.BuyNoOfContracts).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.OiEodamtInCrores)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("OiEODAmtInCrores");
            entity.Property(e => e.OiEodnoOfContracts)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("OiEODNoOfContracts");
            entity.Property(e => e.SellAmtInCrores).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.SellNoOfContracts).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.VerticleHeading).IsRequired();
        });

        modelBuilder.Entity<FutureRollover>(entity =>
        {
            entity.ToTable("FutureRollover");

            entity.Property(e => e.Rollover).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<HistoricPerformance>(entity =>
        {
            entity.ToTable("HistoricPerformance");

            entity.Property(e => e.Change).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<History012021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC0757C1FE18");

            entity.ToTable("History012021");

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

        modelBuilder.Entity<History012022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07362520FC");

            entity.ToTable("History012022");

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

        modelBuilder.Entity<History012023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07393BE819");

            entity.ToTable("History012023");

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

        modelBuilder.Entity<History012024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07AF80FBD4");

            entity.ToTable("History012024");

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

        modelBuilder.Entity<History012025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07A72A5E56");

            entity.ToTable("History012025");

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

        modelBuilder.Entity<History022021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC072EA42EC6");

            entity.ToTable("History022021");

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

        modelBuilder.Entity<History022022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC075EEA721F");

            entity.ToTable("History022022");

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

        modelBuilder.Entity<History022023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC0732D5844F");

            entity.ToTable("History022023");

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

        modelBuilder.Entity<History022024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07A3AC03E0");

            entity.ToTable("History022024");

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

        modelBuilder.Entity<History022025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07A5C53CE3");

            entity.ToTable("History022025");

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

        modelBuilder.Entity<History032021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07A3FDB96E");

            entity.ToTable("History032021");

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

        modelBuilder.Entity<History032022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC0760B68251");

            entity.ToTable("History032022");

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

        modelBuilder.Entity<History032023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC074647A8A6");

            entity.ToTable("History032023");

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

        modelBuilder.Entity<History032024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07EDF0F4EC");

            entity.ToTable("History032024");

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

        modelBuilder.Entity<History032025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07A37D1358");

            entity.ToTable("History032025");

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

        modelBuilder.Entity<History042021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07B858DC5B");

            entity.ToTable("History042021");

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

        modelBuilder.Entity<History042022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07ABD065FB");

            entity.ToTable("History042022");

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

        modelBuilder.Entity<History042023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC074EF48CA0");

            entity.ToTable("History042023");

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

        modelBuilder.Entity<History042024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07DB051581");

            entity.ToTable("History042024");

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

        modelBuilder.Entity<History042025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC072B284637");

            entity.ToTable("History042025");

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

        modelBuilder.Entity<History052021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC073061CB03");

            entity.ToTable("History052021");

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

        modelBuilder.Entity<History052022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07A34D62CD");

            entity.ToTable("History052022");

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

        modelBuilder.Entity<History052023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC0724A85740");

            entity.ToTable("History052023");

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

        modelBuilder.Entity<History052024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07F24289CE");

            entity.ToTable("History052024");

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

        modelBuilder.Entity<History052025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07557A8952");

            entity.ToTable("History052025");

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

        modelBuilder.Entity<History062021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07326B7BEA");

            entity.ToTable("History062021");

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

        modelBuilder.Entity<History062022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07E1DF6C34");

            entity.ToTable("History062022");

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

        modelBuilder.Entity<History062023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07252CFEF8");

            entity.ToTable("History062023");

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

        modelBuilder.Entity<History062024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07625EB6C4");

            entity.ToTable("History062024");

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

        modelBuilder.Entity<History062025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07C97EFD3F");

            entity.ToTable("History062025");

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

        modelBuilder.Entity<History072021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC072C9A469B");

            entity.ToTable("History072021");

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

        modelBuilder.Entity<History072022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC073338CFBD");

            entity.ToTable("History072022");

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

        modelBuilder.Entity<History072023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07F868C03B");

            entity.ToTable("History072023");

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

        modelBuilder.Entity<History072024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07C8D00317");

            entity.ToTable("History072024");

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

        modelBuilder.Entity<History072025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07913D1BF6");

            entity.ToTable("History072025");

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

        modelBuilder.Entity<History082021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07BDDA2328");

            entity.ToTable("History082021");

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

        modelBuilder.Entity<History082022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07AC657408");

            entity.ToTable("History082022");

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

        modelBuilder.Entity<History082023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07FF14EB26");

            entity.ToTable("History082023");

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

        modelBuilder.Entity<History082024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC071C58210C");

            entity.ToTable("History082024");

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

        modelBuilder.Entity<History082025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC076D194F54");

            entity.ToTable("History082025");

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

        modelBuilder.Entity<History092021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07F6C52487");

            entity.ToTable("History092021");

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

        modelBuilder.Entity<History092022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC0753699934");

            entity.ToTable("History092022");

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

        modelBuilder.Entity<History092023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC07BC7A669C");

            entity.ToTable("History092023");

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

        modelBuilder.Entity<History092024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC078DE390F0");

            entity.ToTable("History092024");

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

        modelBuilder.Entity<History092025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History0__3214EC0701490239");

            entity.ToTable("History092025");

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

        modelBuilder.Entity<History102021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC07C30A6083");

            entity.ToTable("History102021");

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

        modelBuilder.Entity<History102022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC07C6EE2C15");

            entity.ToTable("History102022");

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

        modelBuilder.Entity<History102023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC0752C46917");

            entity.ToTable("History102023");

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

        modelBuilder.Entity<History102024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC07DD055721");

            entity.ToTable("History102024");

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

        modelBuilder.Entity<History102025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC071A75B6E4");

            entity.ToTable("History102025");

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

        modelBuilder.Entity<History112021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC079D2784BF");

            entity.ToTable("History112021");

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

        modelBuilder.Entity<History112022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC073E583E42");

            entity.ToTable("History112022");

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

        modelBuilder.Entity<History112023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC0712A2832D");

            entity.ToTable("History112023");

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

        modelBuilder.Entity<History112024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC0768DAF2C2");

            entity.ToTable("History112024");

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

        modelBuilder.Entity<History112025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC07617E9FD1");

            entity.ToTable("History112025");

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

        modelBuilder.Entity<History122021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC07F14D043F");

            entity.ToTable("History122021");

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

        modelBuilder.Entity<History122022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC077DE4EE3A");

            entity.ToTable("History122022");

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

        modelBuilder.Entity<History122023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC07FB88ED89");

            entity.ToTable("History122023");

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

        modelBuilder.Entity<History122024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC073D5B7A01");

            entity.ToTable("History122024");

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

        modelBuilder.Entity<History122025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__History1__3214EC077B3F4808");

            entity.ToTable("History122025");

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

        modelBuilder.Entity<HistorySubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__HistoryS__3214EC0765D9B1C0");

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

        modelBuilder.Entity<MtoStat>(entity =>
        {
            entity.ToTable("MTO_Stats");

            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.DeliverablePercentage).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
        });

        modelBuilder.Entity<Pcr>(entity =>
        {
            entity.ToTable("PCR");

            entity.Property(e => e.Ceoi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("CEOI");
            entity.Property(e => e.Cevolume).HasColumnName("CEVolume");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Expiry).HasColumnType("datetime");
            entity.Property(e => e.Oipcr)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("OIPCR");
            entity.Property(e => e.Oisum)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("OISum");
            entity.Property(e => e.Peoi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PEOI");
            entity.Property(e => e.Pevolume).HasColumnName("PEVolume");
            entity.Property(e => e.Stock)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.VolumePcr)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("VolumePCR");
        });

        modelBuilder.Entity<Sector>(entity =>
        {
            entity.ToTable("Sector");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<SectorInvestment>(entity =>
        {
            entity.ToTable("SectorInvestment");

            entity.Property(e => e.CashFlow).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<SegmentTouchline>(entity =>
        {
            entity.ToTable("SegmentTouchline");

            entity.Property(e => e.Atp).HasColumnName("ATP");
            entity.Property(e => e.Change).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ChangePercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DeliverablePercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LastUpdatedTime).HasColumnType("datetime");
            entity.Property(e => e.Low).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Ltp)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("LTP");
            entity.Property(e => e.Oichange)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("OIChange");
            entity.Property(e => e.OichangePercentage)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("OIChangePercentage");
            entity.Property(e => e.PreviousOiclose).HasColumnName("PreviousOIClose");
            entity.Property(e => e.Segment)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.TodayOi).HasColumnName("TodayOI");
        });

        modelBuilder.Entity<Tl2007>(entity =>
        {
            entity.ToTable("TL2007");

            entity.Property(e => e.Atp)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("ATP");
            entity.Property(e => e.Close).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.High).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.LastTradeTime).HasColumnType("datetime");
            entity.Property(e => e.Low).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Open).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(500);
        });

        modelBuilder.Entity<TouchlineSubscription>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC07DC3E1CD2");

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

        modelBuilder.Entity<TouchlineSubscriptions2018>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC07323DF117");

            entity.ToTable("TouchlineSubscriptions2018");

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

        modelBuilder.Entity<TouchlineSubscriptions2019>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC07B2E19885");

            entity.ToTable("TouchlineSubscriptions2019");

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

        modelBuilder.Entity<TouchlineSubscriptions2020>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC077C3D02A8");

            entity.ToTable("TouchlineSubscriptions2020");

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

        modelBuilder.Entity<TouchlineSubscriptions2021>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC077CBAB248");

            entity.ToTable("TouchlineSubscriptions2021");

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

        modelBuilder.Entity<TouchlineSubscriptions2022>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC074A12EDB7");

            entity.ToTable("TouchlineSubscriptions2022");

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

        modelBuilder.Entity<TouchlineSubscriptions2023>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC077BF6AECA");

            entity.ToTable("TouchlineSubscriptions2023");

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

        modelBuilder.Entity<TouchlineSubscriptions2024>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC0779C28C08");

            entity.ToTable("TouchlineSubscriptions2024");

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

        modelBuilder.Entity<TouchlineSubscriptions2025>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC0788CCD963");

            entity.ToTable("TouchlineSubscriptions2025");

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

        modelBuilder.Entity<TouchlineSubscriptionsStock>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Touchlin__3214EC07FBE27D6C");

            entity.Property(e => e.Atp)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("ATP");
            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.Davol20)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("DAVOL20");
            entity.Property(e => e.Ddel20)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("DDEL20");
            entity.Property(e => e.DeliverablePercentage).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.DeliverableQuantity).HasColumnType("decimal(18, 4)");
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

        modelBuilder.Entity<Volatility>(entity =>
        {
            entity.ToTable("Volatility");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Hv10)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("HV10");
            entity.Property(e => e.Hv20)
                .HasColumnType("decimal(18, 6)")
                .HasColumnName("HV20");
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(200);
        });

        modelBuilder.Entity<ZerodhaHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ZerodhaH__3214EC077F6902E6");

            entity.ToTable("ZerodhaHistory");

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

        modelBuilder.Entity<_52weekHighLow>(entity =>
        {
            entity.ToTable("52WeekHighLow");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.High).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.HighDate).HasColumnType("datetime");
            entity.Property(e => e.Low).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LowDate).HasColumnType("datetime");
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(200);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
