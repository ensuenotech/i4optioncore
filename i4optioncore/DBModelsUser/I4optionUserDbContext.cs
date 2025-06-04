using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace i4optioncore.DBModelsUser;

public partial class I4optionUserDbContext : DbContext
{
    public I4optionUserDbContext()
    {
    }

    public I4optionUserDbContext(DbContextOptions<I4optionUserDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<AdminNotification> AdminNotifications { get; set; }

    public virtual DbSet<AdminNotificationUserRead> AdminNotificationUserReads { get; set; }

    public virtual DbSet<Alert> Alerts { get; set; }

    public virtual DbSet<Basket> Baskets { get; set; }

    public virtual DbSet<BasketOrder> BasketOrders { get; set; }

    public virtual DbSet<BrokeragePlan> BrokeragePlans { get; set; }

    public virtual DbSet<BrokeragePlanUserMapping> BrokeragePlanUserMappings { get; set; }

    public virtual DbSet<BuilderStrategy> BuilderStrategies { get; set; }

    public virtual DbSet<BuilderStrategyComponent> BuilderStrategyComponents { get; set; }

    public virtual DbSet<BuilderStrategySubComponent> BuilderStrategySubComponents { get; set; }

    public virtual DbSet<Calendar> Calendars { get; set; }

    public virtual DbSet<CalendarDate> CalendarDates { get; set; }

    public virtual DbSet<Configuration> Configurations { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Coupon> Coupons { get; set; }

    public virtual DbSet<CustomerAddress> CustomerAddresses { get; set; }

    public virtual DbSet<DhanCredential> DhanCredentials { get; set; }

    public virtual DbSet<DhanOrder> DhanOrders { get; set; }

    public virtual DbSet<EarningRatio> EarningRatios { get; set; }

    public virtual DbSet<Eodscan> Eodscans { get; set; }

    public virtual DbSet<Follower> Followers { get; set; }

    public virtual DbSet<HistoryAuthToken> HistoryAuthTokens { get; set; }

    public virtual DbSet<HistoryRequest> HistoryRequests { get; set; }

    public virtual DbSet<HistorySubscription> HistorySubscriptions { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<NiftyUser> NiftyUsers { get; set; }

    public virtual DbSet<NiftyUsers1> NiftyUsers1s { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Otp> Otps { get; set; }

    public virtual DbSet<Plan> Plans { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<PurchaseHistory> PurchaseHistories { get; set; }

    public virtual DbSet<Rtdatum> Rtdata { get; set; }

    public virtual DbSet<Segment> Segments { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<StockSegmentMapping> StockSegmentMappings { get; set; }

    public virtual DbSet<Strategy> Strategies { get; set; }

    public virtual DbSet<SubWatchlist> SubWatchlists { get; set; }

    public virtual DbSet<Symbol> Symbols { get; set; }

    public virtual DbSet<TouchlineSubscription> TouchlineSubscriptions { get; set; }

    public virtual DbSet<TradeOrder> TradeOrders { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Watchlist> Watchlists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.Property(e => e.Address1).HasColumnName("Address");
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.Gstin)
                .HasMaxLength(50)
                .HasColumnName("GSTIN");
            entity.Property(e => e.PinCode).HasMaxLength(50);
            entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");

            entity.HasOne(d => d.Country).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Addresses_Country");

            entity.HasOne(d => d.State).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Addresses_State");
        });

        modelBuilder.Entity<AdminNotification>(entity =>
        {
            entity.ToTable("AdminNotification");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<AdminNotificationUserRead>(entity =>
        {
            entity.ToTable("Admin_Notification_User_Read");

            entity.HasOne(d => d.User).WithMany(p => p.AdminNotificationUserReads)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Admin_Notification_User_Read_Users");
        });

        modelBuilder.Entity<Alert>(entity =>
        {
            entity.Property(e => e.AlertFor)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Condition)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.SymbolType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TriggeredOn).HasColumnType("datetime");
            entity.Property(e => e.Value).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<Basket>(entity =>
        {
            entity.ToTable("Basket");

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<BasketOrder>(entity =>
        {
            entity.ToTable("BasketOrder");

            entity.Property(e => e.Expiry).HasColumnType("datetime");
            entity.Property(e => e.OperationType).HasMaxLength(50);
            entity.Property(e => e.OrderType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RateType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Strategy).HasMaxLength(255);
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Time).HasColumnType("datetime");
            entity.Property(e => e.TriggerPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.Basket).WithMany(p => p.BasketOrders)
                .HasForeignKey(d => d.BasketId)
                .HasConstraintName("FK_BasketOrder_Basket");

            entity.HasOne(d => d.User).WithMany(p => p.BasketOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BasketOrder_Users");
        });

        modelBuilder.Entity<BrokeragePlan>(entity =>
        {
            entity.Property(e => e.BuyBrokerage).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.BuyMargin).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.SellBrokerage).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.SellMargin).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Strategy).WithMany(p => p.BrokeragePlans)
                .HasForeignKey(d => d.StrategyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BrokeragePlans_Strategy");
        });

        modelBuilder.Entity<BrokeragePlanUserMapping>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.PlanId });

            entity.ToTable("BrokeragePlanUserMapping");

            entity.HasOne(d => d.User).WithMany(p => p.BrokeragePlanUserMappings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BrokeragePlanUserMapping_Users");
        });

        modelBuilder.Entity<BuilderStrategy>(entity =>
        {
            entity.ToTable("BuilderStrategy");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.StrategyName).IsRequired();
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<BuilderStrategyComponent>(entity =>
        {
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.FutureSymbolName).HasMaxLength(50);
            entity.Property(e => e.LastQuoteFutPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LastQuotePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SavedFuturePrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SavedSpotPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SpotSymbolName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.SymbolName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TradeTime).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Strategy).WithMany(p => p.BuilderStrategyComponents)
                .HasForeignKey(d => d.StrategyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BuilderStrategyComponents_BuilderStrategy");
        });

        modelBuilder.Entity<BuilderStrategySubComponent>(entity =>
        {
            entity.Property(e => e.Delta).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EntryPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ExitPrice).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Expiry).HasColumnType("datetime");
            entity.Property(e => e.Iv)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("IV");
            entity.Property(e => e.LastQuoteLtp).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LotQty).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LotSize).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OptionType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Pnl)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PNL");
            entity.Property(e => e.Strike).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.StrikeSymbolName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Theta).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.TradeType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
            entity.Property(e => e.Vega).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.StrategyComponent).WithMany(p => p.BuilderStrategySubComponents)
                .HasForeignKey(d => d.StrategyComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_BuilderStrategySubComponents_BuilderStrategyComponents");
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

            entity.HasOne(d => d.Calendar).WithMany(p => p.CalendarDates)
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

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country");

            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
        });

        modelBuilder.Entity<Coupon>(entity =>
        {
            entity.ToTable("Coupon");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.DiscountType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.DiscountValue).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<CustomerAddress>(entity =>
        {
            entity.HasOne(d => d.Address).WithMany(p => p.CustomerAddresses)
                .HasForeignKey(d => d.AddressId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAddresses_Addresses");

            entity.HasOne(d => d.User).WithMany(p => p.CustomerAddresses)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CustomerAddresses_Users");
        });

        modelBuilder.Entity<DhanCredential>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_DhanCredentails");

            entity.Property(e => e.DhanClientId)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.DhanUcc)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TokenId).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.DhanCredentials)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DhanCredentails_Users");
        });

        modelBuilder.Entity<DhanOrder>(entity =>
        {
            entity.Property(e => e.BoprofitValue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("BOProfitValue");
            entity.Property(e => e.BostopLossValue)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("BOStopLossValue");
            entity.Property(e => e.CorrelationId).HasMaxLength(50);
            entity.Property(e => e.DrvexpiryDate)
                .HasMaxLength(50)
                .HasColumnName("DRVExpiryDate");
            entity.Property(e => e.DrvoptionType).HasColumnName("DRVOptionType");
            entity.Property(e => e.DrvstrikePrice)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("DRVStrikePrice");
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TriggerPrice).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<EarningRatio>(entity =>
        {
            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.ErDate).HasColumnType("datetime");
            entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
        });

        modelBuilder.Entity<Eodscan>(entity =>
        {
            entity.ToTable("EODScan");

            entity.Property(e => e.CandleStick).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Delivery).HasMaxLength(200);
            entity.Property(e => e.Future).HasMaxLength(200);
            entity.Property(e => e.MovingAverage).HasMaxLength(200);
            entity.Property(e => e.Option).HasMaxLength(200);
            entity.Property(e => e.PriceAction).HasMaxLength(200);
            entity.Property(e => e.Volatility).HasMaxLength(200);
            entity.Property(e => e.Volume).HasMaxLength(200);
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
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.Log1)
                .IsRequired()
                .HasColumnName("Log");
        });

        modelBuilder.Entity<NiftyUser>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("nifty_users");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Mobile).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserType).HasMaxLength(50);
        });

        modelBuilder.Entity<NiftyUsers1>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("nifty_users1");

            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Mobile).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UserType).HasMaxLength(50);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.Property(e => e.Campaign).IsRequired();
            entity.Property(e => e.Condition)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Days).HasMaxLength(200);
            entity.Property(e => e.Schedule)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.Template)
                .IsRequired()
                .HasMaxLength(200);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<Otp>(entity =>
        {
            entity.ToTable("OTP");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Otp1)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Otp");

            entity.HasOne(d => d.User).WithMany(p => p.Otps)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OTP_Users");
        });

        modelBuilder.Entity<Plan>(entity =>
        {
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.Domain).HasMaxLength(50);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.Property(e => e.BuyAvg).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Exchange)
                .HasMaxLength(200)
                .HasDefaultValue("BSE");
            entity.Property(e => e.Expiry).HasColumnType("datetime");
            entity.Property(e => e.InstrumentType)
                .HasMaxLength(200)
                .HasDefaultValue("stock-options");
            entity.Property(e => e.Ltp).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.OrderType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.PandL)
                .HasColumnType("decimal(18, 4)")
                .HasColumnName("PAndL");
            entity.Property(e => e.SellAvg).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Strategy).HasMaxLength(50);
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.UpdatedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<PurchaseHistory>(entity =>
        {
            entity.ToTable("Purchase_History");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.Email).IsRequired();
            entity.Property(e => e.Mobile).IsRequired();
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
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

        modelBuilder.Entity<State>(entity =>
        {
            entity.ToTable("State");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");

            entity.HasOne(d => d.Country).WithMany(p => p.States)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_State_Country");
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

            entity.HasOne(d => d.Calendar).WithMany(p => p.Stocks)
                .HasForeignKey(d => d.CalendarId)
                .HasConstraintName("FK_Stock_Calendar");
        });

        modelBuilder.Entity<StockSegmentMapping>(entity =>
        {
            entity.ToTable("Stock_Segment_Mapping");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Strategy>(entity =>
        {
            entity.ToTable("Strategy");

            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<SubWatchlist>(entity =>
        {
            entity.ToTable("SubWatchlist", tb => tb.HasComment(""));

            entity.Property(e => e.Atp)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("ATP");
            entity.Property(e => e.Change).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ChangePercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.High).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.LastUpdatedTime).HasColumnType("datetime");
            entity.Property(e => e.Low).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Ltp)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("LTP");
            entity.Property(e => e.OiChangePercentage).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.Oichange)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("OIChange");
            entity.Property(e => e.Open).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PreviousClose).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PreviousOiclose)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PreviousOIClose");
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TodayOi)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("TodayOI");

            entity.HasOne(d => d.WatchList).WithMany(p => p.SubWatchlists)
                .HasForeignKey(d => d.WatchListId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubWatchlist_Watchlist");
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

        modelBuilder.Entity<TradeOrder>(entity =>
        {
            entity.ToTable("TradeOrder");

            entity.Property(e => e.Boguid)
                .HasMaxLength(50)
                .HasColumnName("BOGUID");
            entity.Property(e => e.Exchange)
                .HasMaxLength(200)
                .HasDefaultValue("BSE");
            entity.Property(e => e.ExecutionTime).HasColumnType("datetime");
            entity.Property(e => e.Expiry).HasColumnType("datetime");
            entity.Property(e => e.Guid).HasMaxLength(50);
            entity.Property(e => e.InstrumentType)
                .HasMaxLength(200)
                .HasDefaultValue("stock-options");
            entity.Property(e => e.OperationType).HasMaxLength(50);
            entity.Property(e => e.OrderType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.RateType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.StopLoss).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Strategy).HasMaxLength(200);
            entity.Property(e => e.Symbol)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.TargetPrice).HasColumnType("decimal(18, 4)");
            entity.Property(e => e.Time).HasColumnType("datetime");
            entity.Property(e => e.TriggerPrice).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.TradeOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TradeOrder_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.AffiliateCode).HasMaxLength(50);
            entity.Property(e => e.AppPlanExpireDate).HasColumnType("datetime");
            entity.Property(e => e.AppToken).HasMaxLength(500);
            entity.Property(e => e.BtexpiryDate)
                .HasColumnType("datetime")
                .HasColumnName("BTExpiryDate");
            entity.Property(e => e.CreatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.ExcelExpiryDate).HasColumnType("datetime");
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Mobile).HasMaxLength(50);
            entity.Property(e => e.NiftyPlanExpireDate).HasColumnType("datetime");
            entity.Property(e => e.PlanExpireDate).HasColumnType("datetime");
            entity.Property(e => e.ReferalCode).HasMaxLength(50);
            entity.Property(e => e.ShowOinumbers).HasColumnName("ShowOINumbers");
            entity.Property(e => e.SocialProfileName).HasMaxLength(500);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
            entity.Property(e => e.UserType)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.WalletBalance).HasColumnType("decimal(18, 2)");
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.Property(e => e.Remarks).HasMaxLength(50);
            entity.Property(e => e.Token).IsRequired();
            entity.Property(e => e.UpdatedOnUtc).HasColumnType("datetime");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.ToTable("Wallet");

            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(200);
        });

        modelBuilder.Entity<Watchlist>(entity =>
        {
            entity.ToTable("Watchlist", tb => tb.HasComment(""));

            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.Watchlists)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Watchlist_Watchlist");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
