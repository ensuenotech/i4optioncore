using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using i4optioncore.DBModels;
using i4optioncore.DBModelsMaster;
using i4optioncore.Models;
using i4optioncore.Repositories.UpdateRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Namotion.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
//using Twilio.Http;
using static i4optioncore.Controllers.CommonController;
using static i4optioncore.Models.CommonModel;
using static StackExchange.Redis.Role;
using HttpClient = System.Net.Http.HttpClient;

namespace i4optioncore.Repositories
{
    public class CommonBL : ICommonBL
    {
        private readonly IRedisBL redisBL;
        private readonly ICacheBL cacheBL;
        i4option_dbContext db;
        MasterdataDbContext dbMaster;
        public CommonBL(i4option_dbContext _db, MasterdataDbContext _masterdb,
            IRedisBL redisBL, ICacheBL cacheBL)
        {
            dbMaster = _masterdb;
            db = _db;
            this.redisBL = redisBL;
            this.cacheBL = cacheBL;
        }


        public int? GetLotSize(string Stock, DateTime Expiry)
        {
            var lotsize = db.Symbols.FirstOrDefault(x => x.Symbol1.StartsWith(Stock) && x.Expiry.HasValue && x.Expiry.Value.Date == Expiry.Date);
            if (lotsize == null)
                return db.Symbols.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Symbol1.StartsWith(Stock) && x.Expiry.HasValue)?.LotSize;
            else
                return lotsize.LotSize;
        }

        public async Task<List<DateTime>> GetAllExpiry()
        {
            return await db.Symbols.Where(x => x.Symbol1.StartsWith("NIFTY") && x.Symbol1.EndsWith("PE")).OrderBy(x => x.Expiry).Select(x => x.Expiry.Value).Distinct().OrderBy(x => x).ToListAsync();

        }
        public async Task<List<DateTime>> GetAllFinNiftyExpiry()
        {
            return await db.Symbols.Where(x => x.Symbol1.StartsWith("FINNIFTY") && x.Symbol1.EndsWith("PE")).OrderBy(x => x.Expiry).Select(x => x.Expiry.Value).Distinct().OrderBy(x => x).ToListAsync();

        }
        public async Task<List<DateTime>> GetAllExpiryDates(int? CalendarId)
        {
            if (!CalendarId.HasValue) CalendarId = 1;

            var key = $"AllExpiry_{CalendarId}";
            var redisValue = await redisBL.GetValue(key);
            if (redisValue != null)
                return JsonConvert.DeserializeObject<List<DateTime>>(redisValue);

            var _result = await db.CalendarDates.Where(x => x.CalendarId == CalendarId && !x.Deleted).Select(x => x.Date).OrderBy(x => x.Date).ToListAsync();
            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
            return _result;
        }
        public async Task<List<DateTime>> GetExpiryDates(int? CalendarId)
        {
            if (!CalendarId.HasValue) CalendarId = 1;


            var _result = await db.CalendarDates.Where(x => x.CalendarId == CalendarId && x.Active && !x.Deleted).Select(x => x.Date).OrderBy(x => x.Date).ToListAsync();
            return _result;
            //List<DateTime> ExpiryDates = new();
            //var currentDate = DateTime.Now;
            //bool indexStock = false;
            //if (StockId.HasValue)
            //{
            //    var stock = await db.Stocks.FirstOrDefaultAsync(x => x.Id == StockId);
            //    if (stock.CalendarId == 1) indexStock = true;
            //}
            //else
            //{
            //    indexStock = true;
            //}


            //List<string> indexFilterSymbols = new() { "NIFTY-I", "NIFTY-II", "NIFTY-III" };
            //List<string> stockFilterSymbols = new() { "FINNIFTY-1", "FINNIFTY-2" };

            //if (indexStock)
            //{
            //    //return await db.Symbols.Where(x => x.Symbol1.StartsWith("NIFTY") && x.Symbol1.EndsWith("CE")).OrderBy(x => x.Expiry).Select(x => x.Expiry.Value).Distinct().Take(2).ToListAsync();
            //    return db.Symbols.Where(x => stockFilterSymbols.Contains(x.Symbol1) || indexFilterSymbols.Contains(x.Symbol1)).OrderBy(x => x.Expiry).Select(x => x.Expiry.Value).AsEnumerable().Where(x=>x.DayOfWeek != DayOfWeek.Tuesday).ToList();
            //}
            //else
            //{
            //    return await db.Symbols.Where(x => indexFilterSymbols.Contains(x.Symbol1)).OrderBy(x => x.Expiry).Select(x => x.Expiry.Value).ToListAsync();

            //}

        }
        //public async Task<bool> SendMail(SendMailDetails mail)
        //{
        //    MailMessage mailmessage = new MailMessage();

        //    mailmessage.From = new MailAddress("admin@ifil.co.in", "i4option.com");
        //    //mailmessage.To.Add(new MailAddress("ensuenotechnologies@gmail.com"));
        //    mailmessage.To.Add(new MailAddress(mail.To));


        //    if (mail.BCC != null)
        //    {
        //        mail.BCC.ForEach(x =>
        //        {
        //            mailmessage.Bcc.Add(new MailAddress(x));
        //        });

        //    }
        //    if (mail.Attachments != null)
        //    {
        //        mail.Attachments.ForEach(attachment =>
        //        {
        //            var extension = attachment.Remove(0, attachment.LastIndexOf("."));
        //            var filename = Guid.NewGuid() + extension;
        //            byte[] fileBytes;

        //            string someUrl = attachment;
        //            using (var webClient = new WebClient())
        //            {
        //                fileBytes = webClient.DownloadData(someUrl);
        //            }
        //            mailmessage.Attachments.Add(new Attachment(new MemoryStream(fileBytes), filename));
        //        });



        //    }

        //    mailmessage.Subject = mail.Subject;
        //    mailmessage.IsBodyHtml = true;

        //    mailmessage.Body = mail.Body;

        //    var smtp = new SmtpClient()
        //    {

        //    };

        //    smtp.Host = "smtp.gmail.com";
        //    smtp.Port = 587;
        //    smtp.EnableSsl = true;
        //    smtp.Credentials = new NetworkCredential("admin@ifil.co.in", "man@2929");
        //    //mailmessage.From = new MailAddress("ensuenotechdevtest@gmail.com", "i4option.com");
        //    //smtp.Credentials = new NetworkCredential("ensuenotechdevtest@gmail.com", "EncryptMe@2021");

        //    await smtp.SendMailAsync(mailmessage);
        //    return true;
        //}

        public async Task DeleteStock(int StockId)
        {
            db.Stocks.FirstOrDefault(s => s.Id == StockId).Deleted = true;
            await db.SaveChangesAsync();
        }
        public async Task<int> SaveStockDetails(StockFormDetails values)
        {
            var stock = db.Stocks.FirstOrDefault(x => x.Id == values.Id) ?? new Stock();
            stock.Name = values.Name;
            stock.Type = values.Type;
            stock.DisplayName = values.DisplayName;
            stock.Change = values.Change;
            stock.UpdatedOnUtc = DateTime.Now;
            stock.CalendarId = values.CalendarId;
            stock.Depth = values.Depth;
            stock.LotSize = values.LotSize;
            stock.FreeFloat = values.FreeFloat;
            if (stock.Id == 0)
            {
                stock.Active = true;
                stock.Deleted = false;
                stock.CreatedOnUtc = DateTime.Now;
                db.Stocks.Add(stock);
            }
            await db.SaveChangesAsync();

            db.StockSegmentMappings.RemoveRange(db.StockSegmentMappings.Where(x => x.StockId == stock.Id).ToList());
            foreach (var seg in values.SegmentIds)
            {
                db.StockSegmentMappings.Add(new StockSegmentMapping
                {
                    SegmentId = seg,
                    StockId = stock.Id
                });
            }
            await db.SaveChangesAsync();
            var key = $"Stocks";
            await redisBL.DeleteKey(key);
            return stock.Id;
        }

        public async Task<List<StockDetails>> GetStocks()
        {


            var _result = await db.Stocks.Where(s => !s.Deleted && s.Type == "F&O")
                .Include(x => x.Calendar)
                .Include(x => x.StockSegmentMappings)
                .Select(s => new StockDetails
                {
                    Active = s.Active,
                    Change = s.Change,
                    CreatedOnUtc = s.CreatedOnUtc,
                    CalendarId = s.CalendarId,
                    FreeFloat = s.FreeFloat,
                    Expiry = s.Calendar.CalendarDates.Where(x => !x.Deleted && x.Active).OrderBy(x => x.Date).Select(d => d.Date).ToList(),
                    Depth = s.Depth,
                    DisplayName = s.DisplayName,
                    LotSize = s.LotSize,
                    //LotSize = db.Symbols.FirstOrDefault(x => x.Symbol1.StartsWith(s.DisplayName) && x.Expiry.HasValue && x.Expiry.Value.Date == db.CalendarDates.Where(x => x.CalendarId == s.CalendarId && !x.Deleted).Select(x => x.Date).OrderBy(x => x.Date).FirstOrDefault().Date) == null ? s.LotSize : db.Symbols.FirstOrDefault(x => x.Symbol1.StartsWith(s.DisplayName) && x.Expiry.HasValue && x.Expiry.Value.Date == db.CalendarDates.Where(x => x.CalendarId == s.CalendarId && !x.Deleted).Select(x => x.Date).OrderBy(x => x.Date).FirstOrDefault().Date).LotSize,
                    Id = s.Id,
                    MaxPain = s.MaxPain,
                    MaxPainLastUpdatedUtc = s.MaxPainLastUpdatedUtc,
                    Name = s.Name,
                    Type = s.Type,
                    //Segments =  db.StockSegmentMappings.Where(x => x.StockId == s.Id).Select(x => db.Segments.FirstOrDefault(seg => seg.Id == x.SegmentId).Name).ToList(),
                    //SegmentIds = db.StockSegmentMappings.Where(x => x.StockId == s.Id).Select(x => x.SegmentId).ToList(),
                    SegmentIds = s.StockSegmentMappings.Select(x => x.SegmentId).ToList(),
                    Segments = s.StockSegmentMappings.Select(x => x.Segment.Name).ToList(),
                    UpdatedOnUtc = s.UpdatedOnUtc

                }).ToListAsync();

            return _result;
        }
        public async Task<List<DBModels.Segment>> GetSegments()
        {

            var _result = await db.Segments.ToListAsync();

            return _result;
        }

        public async Task<List<DBModels.EarningRatio>> GetEarningRatios(string symbol)
        {
            return await db.EarningRatios.Where(x => x.IndexName.ToLower() == symbol.ToLower()).OrderBy(x => x.IndexDate).ToListAsync();
        }

        public async Task<bool> UpdateMaxPain(decimal MaxPain, int StockId)
        {
            var stock = await db.Stocks.FirstOrDefaultAsync(x => x.Id == StockId);
            if (stock == null) return false;
            stock.MaxPain = MaxPain;
            stock.MaxPainLastUpdatedUtc = DateTime.Now;

            await db.SaveChangesAsync();
            var key = $"Stocks";
            await redisBL.DeleteKey(key);
            return true;
        }

        public async Task<List<CalendarDetails>> GetCalendars()
        {
            return await db.Calendars.Where(x => !x.Deleted).Select(x => new CalendarDetails
            {
                Dates = x.CalendarDates.Where(x => !x.Deleted).OrderBy(x => x.Date).Select(x => new CalendarDateDetails { Active = x.Active, Date = x.Date, Id = x.Id }).ToList(),
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();
        }
        public async Task<bool> SaveCalendarDate(int? id, DateTime date, int calendarId, bool Active)
        {
            var calendar = await db.CalendarDates.FirstOrDefaultAsync(x => x.Id == id) ?? new CalendarDate();

            calendar.Active = Active;
            calendar.CalendarId = calendarId;
            calendar.Date = date;
            calendar.Deleted = false;
            calendar.UpdatedOnUtc = DateTime.Now;
            if (calendar.Id == 0)
            {
                calendar.CreatedOnUtc = DateTime.Now;
                db.CalendarDates.Add(calendar);
            }
            await db.SaveChangesAsync();
            var key = $"Expiry_{calendarId}";
            await redisBL.DeleteKey(key);
            key = $"AllExpiry_{calendarId}";
            await redisBL.DeleteKey(key);
            return true;
        }
        public async Task<bool> SaveCalendar(int id, string Name)
        {
            var calendar = await db.Calendars.FirstOrDefaultAsync(x => x.Id == id) ?? new Calendar();
            calendar.Active = true;
            calendar.Name = Name;
            calendar.Deleted = false;
            calendar.UpdatedOnUtc = DateTime.Now;
            if (calendar.Id == 0)
            {
                calendar.CreatedOnUtc = DateTime.Now;
                db.Calendars.Add(calendar);
            }
            await db.SaveChangesAsync();
            var key = $"Expiry_{id}";
            await redisBL.DeleteKey(key);
            return true;
        }
        public async Task<bool> DeleteCalendarDate(DateTime date)
        {
            db.CalendarDates.Where(x => x.Date == date).ToList().ForEach(x => x.Deleted = true);
            await db.SaveChangesAsync();

            var key = $"Expiry_{db.CalendarDates.FirstOrDefault(x => x.Date == date).CalendarId}";
            await redisBL.DeleteKey(key);
            key = $"AllExpiry_{db.CalendarDates.FirstOrDefault(x => x.Date == date).CalendarId}";
            await redisBL.DeleteKey(key);
            return true;
        }
        public async Task<bool> DeleteCalendar(int id)
        {
            db.Calendars.Where(x => x.Id == id).ToList().ForEach(x => x.Deleted = true);
            await db.SaveChangesAsync();
            var key = $"Expiry_{id}";
            await redisBL.DeleteKey(key);
            return true;
        }
        public async Task<List<string>> GetStrikes(string stockName, DateTime expiry, string Series = "CE")
        {
            Series ??= "CE";
            return await db.Symbols
                .Where(x => x.Symbol1.StartsWith(stockName) && x.Expiry == expiry && x.Series == Series)
                .Select(x => x.Strike)
                .Distinct()
                .OrderBy(x => x)
                .Select(x => x % 1 == 0 ? x.ToString("0") : x.ToString("0.0"))
                .ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetTouchlineStockByDate(List<string> Symbols, DateTime fromdate, DateTime todate)
        {

            return await dbMaster.TouchlineSubscriptionsStocks.Where(x => Symbols.Contains(x.Symbol)
            && fromdate.Date <= x.LastUpdatedTime.Date
            && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
            {
                Atp = x.Atp,
                High = x.High,
                LastUpdatedTime = x.LastUpdatedTime.Date,
                Low = x.Low,
                Ltp = x.Ltp,
                Open = x.Open,
                PreviousClose = x.PreviousClose,
                PreviousOiclose = x.PreviousOiclose,
                Symbol = x.Symbol,
                SymbolId = x.SymbolId,
                TickVolume = x.TickVolume,
                TodayOi = x.TodayOi,
                TotalVolume = x.TotalVolume,
                TurnOver = x.TurnOver,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
                Davol20 = x.Davol20,
                Ddel20 = x.Ddel20,
                DeliverablePercentage = x.DeliverablePercentage,
                DeliverableQuantity = x.DeliverableQuantity
            }).Distinct().ToListAsync();


        }

        public async Task<List<SymbolDetails>> GetSymbols()
        {
            var datenow = DateTime.Now.Date;
            return await db.Symbols.Where(x => (x.Expiry.HasValue && x.Expiry.Value.Year == 1970) || x.Expiry >= datenow || x.Expiry == null || x.Series == "EQ").Select(x => new SymbolDetails { Symbol = x.Symbol1, SymbolId = x.SymbolId, LotSize = x.LotSize, Expiry = x.Expiry }).ToListAsync();
        }
        public async Task<List<SymbolDetails>> GetTouchlineSymbols()
        {
            return await (from ts in db.TouchlineSubscriptions
                          join sb in db.Symbols on ts.SymbolId equals sb.SymbolId
                          let v = $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM} {sb.Strike:00} CE"
                          let v1 = $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM} {sb.Strike:00} PE"
                          let v2 = $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM} {sb.Strike:00} PE"
                          let v3 = $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM} {sb.Strike:00} CE"
                          select new SymbolDetails
                          {
                              Symbol = sb.Symbol1,
                              SymbolId = sb.SymbolId,
                              Strike = sb.Strike,
                              LotSize = sb.LotSize,
                              Expiry = sb.Expiry,
                              Series = sb.Series,
                              LTP = ts.Ltp,
                              Alias = ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)
                              ? v3
                              : (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)
                              ? v2
                              : (sb.Symbol15.EndsWith("FUT") && sb.Symbol1.IndexOf("2") > -1) ? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} FUT {sb.Expiry.Value:MMM}" : sb.Symbol1).ToUpper(),
                              TradingSymbol = ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)
                              ? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}{sb.Expiry.Value:yy}{sb.Expiry.Value:MMM}{sb.Strike:00}CE"
                              : (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)
                              ? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}{sb.Expiry.Value:yy}{sb.Expiry.Value:MMM}{sb.Strike:00}PE"
                              : sb.Symbol1).ToUpper(),
                              SearchTerm = new List<string>()
                              {
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? v: (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? v1: (sb.Symbol15.EndsWith("FUT") && sb.Symbol1.IndexOf("2") > -1) ? $"FUT {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM}" : sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? v: (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? v1: (sb.Symbol15.EndsWith("FUT") && sb.Symbol1.IndexOf("2") > -1) ? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} FUT {sb.Expiry.Value:MMM}" : sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Strike:00} CE": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Strike:00} PE": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Strike:00} CE {sb.Expiry.Value:MMM}": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Strike:00} PE {sb.Expiry.Value:MMM}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Strike:00} {sb.Expiry.Value:MMM} CE": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Strike:00} {sb.Expiry.Value:MMM} PE": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} CE {sb.Strike:00} {sb.Expiry.Value:MMM}": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} PE {sb.Strike:00} {sb.Expiry.Value:MMM}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:dd} {sb.Expiry.Value:MMM} {sb.Strike:00} CE": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:dd} {sb.Expiry.Value:MMM} {sb.Strike:00} PE": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:dd} {sb.Expiry.Value:MMM} CE {sb.Strike:00}": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:dd} {sb.Expiry.Value:MMM} PE {sb.Strike:00}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Expiry.Value:MMM} {sb.Strike:00} CE {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Expiry.Value:MMM} {sb.Strike:00} PE {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Strike:00} {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM} CE": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Strike:00} {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM} PE": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Strike:00} CE {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM}": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Strike:00} PE {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"CE {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Strike:00} {sb.Expiry.Value:MMM}": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"PE {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Strike:00} {sb.Expiry.Value:MMM}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("CE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Strike:00}CE{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}": (sb.Symbol1.EndsWith("PE") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Strike:00}PE{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("FUT") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM} FUT": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("FUT") && sb.Symbol1.IndexOf("2") > -1)? $"FUT {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))} {sb.Expiry.Value:MMM}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("FUT") && sb.Symbol1.IndexOf("2") > -1)? $"FUT {sb.Expiry.Value:MMM} {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("FUT") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Expiry.Value:MMM} FUT {sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("FUT") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Expiry.Value:MMM}FUT{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}": sb.Symbol1).ToUpper(),
                                  ((sb.Symbol1.EndsWith("FUT") && sb.Symbol1.IndexOf("2") > -1)? $"{sb.Symbol1.Remove(sb.Symbol1.IndexOf("2"))}FUT{sb.Expiry.Value:MMM}": sb.Symbol1).ToUpper(),
                              }
                          }).OrderBy(x => x.SymbolId).Distinct().ToListAsync();
            //return await db.TouchlineSubscriptions.Select(x => new SymbolDetails
            //{
            //    Symbol = x.Symbol,
            //    SymbolId = x.SymbolId,
            //    LotSize = db.Symbols.FirstOrDefault(s => s.Symbol1 == x.Symbol).LotSize,
            //}).ToListAsync();
        }

        public async Task<List<HistoryRecord>> GetHistory(List<string> Symbols, DateTime From, DateTime To, int Interval)
        {
            var data = new List<HistoryRecord>();
            var nine_sixteen_time = new DateTime(From.Year, From.Month, From.Day, 9, 16, 0);
            if (From < nine_sixteen_time) From = nine_sixteen_time;
            if (Interval > 1)
            {
                data.AddRange(await db.HistorySubscriptions.Where(x => Symbols.Contains(x.Symbol) && x.LastTradeTime >= From && x.LastTradeTime <= From)
                     .OrderBy(x => x.LastTradeTime).Select(x => new HistoryRecord
                     {
                         LastTradeTime = x.LastTradeTime,
                         Open = x.Open,
                         Close = x.Close,
                         Atp = x.Atp,
                         Symbol = x.Symbol,
                         High = x.High,
                         Low = x.Low,
                         Volume = x.Volume,
                         OpenInterest = x.OpenInterest,
                         Interval = Interval,
                         TotalVolume = x.TotalVolume.Value
                     }).OrderBy(x => x.Symbol).ThenBy(x => x.LastTradeTime).ToListAsync());
            }

            data.AddRange(await db.HistorySubscriptions.Where(x => Symbols.Contains(x.Symbol) && x.LastTradeTime >= From && x.LastTradeTime <= To && x.LastTradeTime.Minute % Interval == 0)
                   .OrderBy(x => x.LastTradeTime).Select(x => new HistoryRecord
                   {
                       LastTradeTime = x.LastTradeTime,
                       Open = x.Open,
                       Close = x.Close,
                       Atp = x.Atp,
                       Symbol = x.Symbol,
                       High = x.High,
                       Low = x.Low,
                       Volume = x.Volume,
                       OpenInterest = x.OpenInterest,
                       Interval = Interval,
                       TotalVolume = x.TotalVolume.Value
                   }).OrderBy(x => x.Symbol).ThenBy(x => x.LastTradeTime).ToListAsync());

            if (data.Count == 0)
            {
                data.AddRange(await dbMaster.HistorySubscriptions.Where(x => Symbols.Contains(x.Symbol) && x.LastTradeTime >= From && x.LastTradeTime <= To && x.LastTradeTime.Minute % Interval == 0)
                     .OrderBy(x => x.LastTradeTime).Select(x => new HistoryRecord
                     {
                         LastTradeTime = x.LastTradeTime,
                         Open = x.Open,
                         Close = x.Close,
                         Atp = x.Atp,
                         Symbol = x.Symbol,
                         High = x.High,
                         Low = x.Low,
                         Volume = x.Volume,
                         OpenInterest = x.OpenInterest,
                         Interval = Interval,
                     }).OrderBy(x => x.Symbol).ThenBy(x => x.LastTradeTime).ToListAsync());
            }
            return data;
        }
        public async Task<List<HistoryRecord>> GetFullHistory(List<string> Symbols, DateTime From, DateTime To, int Interval)
        {
            var selectedRecords = await db.HistorySubscriptions.Where(x => Symbols.Contains(x.Symbol) && x.LastTradeTime >= From && x.LastTradeTime <= To).ToListAsync();
            List<HistoryRecord> records = new();
            List<DateTime> timeslots = new();
            if (To > DateTime.Now) To = DateTime.Now;


            var maxtime = new DateTime(To.Year, To.Month, To.Day, 15, 30, 0);
            var minutes = (To - From).TotalMinutes;
            var intervals = minutes / Interval;
            for (int i = 0; i <= intervals; i++)
            {
                var x = From.AddMinutes(Interval * i);
                if (x <= maxtime)
                    timeslots.Add(x);
            }
            var c = timeslots;
            foreach (var symbol in Symbols)
            {
                foreach (var time in timeslots)
                {
                    var historydata = selectedRecords.FirstOrDefault(x => x.LastTradeTime == time && x.Symbol == symbol);
                    if (historydata != null)
                    {
                        records.Add(new HistoryRecord
                        {
                            Atp = historydata.Atp,
                            Close = historydata.Close,
                            High = historydata.High,
                            LastTradeTime = historydata.LastTradeTime,
                            Low = historydata.Low,
                            Open = historydata.Open,
                            OpenInterest = historydata.OpenInterest,
                            Symbol = historydata.Symbol,
                            Volume = historydata.Volume,
                            Interval = Interval

                        });
                    }
                    else
                    {
                        var historydataPrevious = db.HistorySubscriptions.Where(x => x.Symbol == symbol && x.LastTradeTime <= time).OrderByDescending(x => x.LastTradeTime).FirstOrDefault();
                        if (historydataPrevious != null)
                        {
                            records.Add(new HistoryRecord
                            {
                                Atp = historydataPrevious.Atp,
                                Close = historydataPrevious.Close,
                                High = historydataPrevious.High,
                                LastTradeTime = time,
                                Low = historydataPrevious.Low,
                                Open = historydataPrevious.Open,
                                OpenInterest = historydataPrevious.OpenInterest,
                                Symbol = historydataPrevious.Symbol,
                                Volume = historydataPrevious.Volume,
                                Interval = Interval
                            });
                        }

                    }
                }
            }
            return records;
            //return await db.HistorySubscriptions.Where(x => Symbols.Contains(x.Symbol) && x.LastTradeTime >= From && x.LastTradeTime <= To && x.LastTradeTime.Minute % Interval == 0)
            //     .OrderBy(x => x.LastTradeTime).Select(x => new
            //     {
            //         x.LastTradeTime,
            //         x.Open,
            //         x.Close,
            //         x.Atp,
            //         x.Symbol,
            //         x.High,
            //         x.Low,
            //         x.Volume,
            //         x.OpenInterest,
            //         interval = Interval
            //     }).OrderBy(x => x.Symbol).ThenBy(x => x.LastTradeTime).ToListAsync();

            //return data;
            //var quadrant = 60 / Interval;

            //if (Interval == 3)



        }

        public async Task<List<VolumeCommentary>> GetVolumeCommentary()
        {
            return await db.VolumeCommentaries.OrderByDescending(x => x.LastTradeTime).ToListAsync();
        }
        public async Task<List<SpotVolumeCommentary>> GetSpotVolumeCommentary()
        {
            return await db.SpotVolumeCommentaries.OrderByDescending(x => x.LastTradeTime).ToListAsync();
        }

        public async Task SubscribeSymbol(List<string> Symbols)
        {
            Symbols.ForEach(Symbol =>
            {
                var symbol = db.Symbols.FirstOrDefault(x => x.Symbol1 == Symbol);
                if (symbol != null)
                    if (!db.Rtdata.Any(x => x.Symbol == Symbol))
                    {
                        db.Rtdata.Add(new Rtdatum
                        {
                            SymbolId = symbol.SymbolId,
                            Symbol = Symbol,
                        });
                    }

            });
            await db.SaveChangesAsync();

        }

        public async Task<int> GetMarketHoliday()
        {
            var date = DateTime.Now;
            var holidays = new List<Holiday>();
            var key = "holiday";
            var value = await redisBL.GetValue(key);
            if (value != null)
            {
                holidays = JsonConvert.DeserializeObject<List<Holiday>>(value);
            }
            else
            {
                holidays = await db.Holidays.ToListAsync();
                await redisBL.SetValue(key, JsonConvert.SerializeObject(holidays));
            }
            int HolidayCount = 0;
            var holiday = holidays.FirstOrDefault(x => x.Date == date.Date);
            if (holiday != null)
            {
                var newDate = date.AddDays(-1);
                HolidayCount = 1;
                if (holidays.FirstOrDefault(x => x.Date == newDate.Date) != null)
                {
                    HolidayCount += 1;
                }
                else if (newDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    HolidayCount += 1;
                }
                else if (newDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    HolidayCount += 2;
                }
            }
            //var config = await db.Configurations.FirstOrDefaultAsync(x => x.Key == "IsMarketOpen");
            //var isMarketOpen = bool.Parse(config.Value);
            //if (!isMarketOpen)
            //{
            //    config = await db.Configurations.FirstOrDefaultAsync(x => x.Key == "HistoryData");
            //    return int.Parse(config.Value);
            //}

            //return 0;
            return HolidayCount;
        }

        public async Task<List<TouchlineSubscriptionDetailsWithMto>> TouchlineWithMTO(string symbol, DateTime fromDate, DateTime toDate)
        {

            var rediskey = $"TOUCHLINEWITHMTO_{symbol}_{fromDate:dd-MM-yyyy}_{toDate:dd-MM-yyyy}";
            var value = await redisBL.GetValue(rediskey);
            var timekey = rediskey + "_time";
            var time = DateTime.Now; //10000000 = 1 sec

            var timekeyvalue = await redisBL.GetValue(timekey);
            if (value != null)
            {
                var res = JsonConvert.DeserializeObject<List<TouchlineSubscriptionDetailsWithMto>>(value);
                if (timekeyvalue != null && DateTime.Parse(timekeyvalue).AddMinutes(1) > time && res.Count > 0)
                    return res;
            }
            var results = new List<TouchlineSubscriptionDetailsWithMto>();

            var param = new SqlParameter[]
                   {
                new SqlParameter()
                {
                    ParameterName = "Symbol",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = symbol
                },
                new SqlParameter()
                {
                    ParameterName = "FromDate",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = fromDate
                },new SqlParameter()
                {
                    ParameterName = "ToDate",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = toDate
                }
                   };
            using var connection = new SqlConnection(dbMaster.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sproc_GET_TOUCHLINEBYDATE]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new TouchlineSubscriptionDetailsWithMto
                    {
                        LastUpdatedTime = Convert.ToDateTime(reader["LastUpdatedTime"]).Date,
                        Atp = Convert.ToDecimal(reader["ATP"]),
                        High = Convert.ToDecimal(reader["High"]),
                        Low = Convert.ToDecimal(reader["Low"]),
                        Ltp = Convert.ToDecimal(reader["LTP"]),
                        Open = Convert.ToDecimal(reader["Open"]),
                        PreviousClose = Convert.ToDecimal(reader["PreviousClose"]),
                        PreviousOiclose = Convert.ToDecimal(reader["PreviousOIClose"]),
                        Symbol = reader["Symbol"].ToString(),
                        SymbolId = Convert.ToInt32(reader["SymbolId"]),
                        TickVolume = Convert.ToDecimal(reader["TickVolume"]),
                        TodayOi = Convert.ToDecimal(reader["TodayOI"]),
                        TotalVolume = Convert.ToInt64(reader["TotalVolume"]),
                        TurnOver = Convert.ToDecimal(reader["TurnOver"]),
                        Change = Convert.ToDecimal(reader["Change"]),
                        ChangePercentage = Convert.ToDecimal(reader["ChangePercentage"]),
                        OiChange = Convert.ToDecimal(reader["OIChange"]),
                        OiChangePercentage = Convert.ToDecimal(reader["OIChangePercentage"]),
                        Delivery = Convert.ToInt64(reader["DeliverableQuantity"]),
                        DeliveryPercentage = Convert.ToInt64(reader["DeliverablePercentage"]),
                        DDEL20 = Convert.ToInt64(reader["DDEL20"]),
                        DAVOL20 = Convert.ToInt64(reader["DAVOL20"]),
                    });
                }
            }
            connection.Close();
            await redisBL.SetValue(rediskey, JsonConvert.SerializeObject(results));
            //results.ForEach(async r =>
            //{
            //    var rediskey = $"TOUCHLINEWITHMTO_{r.Symbol}_{r.LastUpdatedTime:dd-MM-yyyy}";
            //    await redisBL.SetValue(rediskey, JsonConvert.SerializeObject(r));

            //});
            return results;
        }
        public async Task<DBModels.TouchlineSubscription> GetTouchline(string symbol)
        {
            var symbolkey = $"TOUCHLINE_{symbol}";
            var touchline = await db.TouchlineSubscriptions.FirstOrDefaultAsync(t => t.Symbol == symbol);
            if (touchline != null)
                await redisBL.SetValue(symbolkey, JsonConvert.SerializeObject(touchline));
            return touchline;
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetOptionTouchline(string stockName, DateTime expiry)
        {
            var stockDisplayName = db.Stocks.Where(x => x.Name == stockName).FirstOrDefault().DisplayName;
            var symbols = await db.Symbols.Where(x => x.Symbol1 == stockDisplayName || (x.Symbol1.StartsWith(stockName) && ((x.Expiry.HasValue && x.Expiry.Value.Date == expiry.Date && (x.Series == "CE" || x.Series == "PE")) || x.Series == "XX"))).Select(x => x.Symbol1).Distinct().OrderBy(x => x).ToListAsync();
            var date = expiry.ToString("yyMMdd");
            return await db.TouchlineSubscriptions.Where(x => x.Ltp != 0 && (x.Symbol.StartsWith(stockName) || x.Symbol == stockDisplayName) && symbols.Contains(x.Symbol)).Select(x => new TouchlineSubscriptionDetails
            {
                Atp = x.Atp,
                High = x.High,
                LastUpdatedTime = x.LastUpdatedTime,
                Low = x.Low,
                Ltp = x.Ltp,
                Open = x.Open,
                PreviousClose = x.PreviousClose,
                PreviousOiclose = x.PreviousOiclose,
                Symbol = x.Symbol,
                SymbolId = x.SymbolId,
                TickVolume = x.TickVolume,
                TodayOi = x.TodayOi,
                TotalVolume = x.TotalVolume,
                TurnOver = x.TurnOver,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).Distinct().OrderBy(x => x.LastUpdatedTime).ToListAsync();
        }


        public async Task<List<TouchlineSubscriptionDetails>> GetTouchline(List<string> Symbols)
        {


            var touchlines = new List<DBModels.TouchlineSubscription>();


            //var touchlines = await Task.WhenAll(Symbols.Select(symbol => db.TouchlineSubscriptions.FirstOrDefaultAsync(t => t.Symbol == symbol)));
            var _touchlines = await Task.WhenAll(Symbols.Select(symbol => redisBL.GetValue($"TOUCHLINE_{symbol}")));
            foreach (var touchline in _touchlines.Where(t => t != null))
            {
                try
                {
                    if (touchline != null)
                    {
                        var _t = JsonConvert.DeserializeObject<DBModels.TouchlineSubscription>(touchline);
                        touchlines.Add(_t);
                    }
                }
                catch
                {
                    var __ = touchlines;
                }
            }

            var newSymbols = Symbols.Where(x => !touchlines.Where(t => t != null).Select(t => t.Symbol).Contains(x)).ToList();
            await UpdateTouchlineRedis(newSymbols);
            var _newtouchlines = await Task.WhenAll(newSymbols.Select(symbol => redisBL.GetValue($"TOUCHLINE_{symbol}")));
            foreach (var touchline in _newtouchlines.Where(t => t != null))
            {
                try
                {
                    if (touchline != null)
                    {
                        var _t = JsonConvert.DeserializeObject<DBModels.TouchlineSubscription>(touchline);
                        touchlines.Add(_t);
                    }
                }
                catch
                {
                    var __ = touchlines;
                }
            }


            return touchlines.Select(x => new TouchlineSubscriptionDetails
            {
                Atp = x.Atp,
                High = x.High,
                LastUpdatedTime = x.LastUpdatedTime,
                Low = x.Low,
                Ltp = x.Ltp,
                Open = x.Open,
                PreviousClose = x.PreviousClose,
                PreviousOiclose = x.PreviousOiclose,
                Symbol = x.Symbol,
                SymbolId = x.SymbolId,
                TickVolume = x.TickVolume,
                TodayOi = x.TodayOi,
                TotalVolume = x.TotalVolume,
                TurnOver = x.TurnOver,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).ToList();

        }

        public async Task<List<TouchlineSubscriptionDetails>> GetTouchlineByDate(List<string> Symbols, DateTime fromdate, DateTime todate)
        {

            //if (fromdate.Year != todate.Year)
            //{
            //    if (fromdate.Year < todate.Year)
            //    {
            //        fromdate = new DateTime(todate.Year, 1, 1);
            //    }
            //    throw new Exception("Years should be same");
            //}

            // var touchline = await GetTouchlineByYear(fromdate.Year);
            // return touchline.Where(x => Symbols.Contains(x.Symbol)
            //&& fromdate.Date <= x.LastUpdatedTime.Date
            //&& todate.Date >= x.LastUpdatedTime.Date).ToList();

            var results = new List<TouchlineSubscriptionDetails>();
            if (fromdate.Year == 2019)
            {
                 results.AddRange( await dbMaster.TouchlineSubscriptions2019s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (fromdate.Year == 2020)
            {
                results.AddRange(await dbMaster.TouchlineSubscriptions2020s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (fromdate.Year == 2021)
            {
                results.AddRange(await dbMaster.TouchlineSubscriptions2021s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (fromdate.Year == 2022)
            {
                results.AddRange(await dbMaster.TouchlineSubscriptions2022s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (fromdate.Year == 2023)
            {
                results.AddRange(await dbMaster.TouchlineSubscriptions2023s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (fromdate.Year == 2024)
            {
                results.AddRange(await dbMaster.TouchlineSubscriptions2024s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (fromdate.Year == 2025)
            {
                results.AddRange(await dbMaster.TouchlineSubscriptions2025s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }

            if (fromdate.Date.Year == todate.Date.Year)
                return results;


            if (todate.Year == 2019)
            {
                results.AddRange( await dbMaster.TouchlineSubscriptions2019s.Where(x => Symbols.Contains(x.Symbol)
          && fromdate.Date <= x.LastUpdatedTime.Date
          && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
          {
              Atp = x.Atp,
              High = x.High,
              LastUpdatedTime = x.LastUpdatedTime.Date,
              Low = x.Low,
              Ltp = x.Ltp,
              Open = x.Open,
              PreviousClose = x.PreviousClose,
              PreviousOiclose = x.PreviousOiclose,
              Symbol = x.Symbol,
              SymbolId = x.SymbolId,
              TickVolume = x.TickVolume,
              TodayOi = x.TodayOi,
              TotalVolume = x.TotalVolume,
              TurnOver = x.TurnOver,
              Change = x.Ltp - x.PreviousClose,
              ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
              OiChange = x.TodayOi - x.PreviousOiclose,
              OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
          }).Distinct().ToListAsync());
            }
            else if (todate.Year == 2020)
            {
                results.AddRange( await dbMaster.TouchlineSubscriptions2020s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (todate.Year == 2021)
            {
                results = await dbMaster.TouchlineSubscriptions2021s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync();
            }
            else if (todate.Year == 2022)
            {
                results.AddRange( await dbMaster.TouchlineSubscriptions2022s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (todate.Year == 2023)
            {
                results.AddRange( await dbMaster.TouchlineSubscriptions2023s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (todate.Year == 2024)
            {
                results.AddRange( await dbMaster.TouchlineSubscriptions2024s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }
            else if (todate.Year == 2025)
            {
                results.AddRange( await dbMaster.TouchlineSubscriptions2025s.Where(x => Symbols.Contains(x.Symbol)
           && fromdate.Date <= x.LastUpdatedTime.Date
           && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
           {
               Atp = x.Atp,
               High = x.High,
               LastUpdatedTime = x.LastUpdatedTime.Date,
               Low = x.Low,
               Ltp = x.Ltp,
               Open = x.Open,
               PreviousClose = x.PreviousClose,
               PreviousOiclose = x.PreviousOiclose,
               Symbol = x.Symbol,
               SymbolId = x.SymbolId,
               TickVolume = x.TickVolume,
               TodayOi = x.TodayOi,
               TotalVolume = x.TotalVolume,
               TurnOver = x.TurnOver,
               Change = x.Ltp - x.PreviousClose,
               ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
               OiChange = x.TodayOi - x.PreviousOiclose,
               OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
           }).Distinct().ToListAsync());
            }

            return results;
            // return await dbMaster.TouchlineSubscriptions.Where(x => Symbols.Contains(x.Symbol)
            // && fromdate.Date <= x.LastUpdatedTime.Date
            // && todate.Date >= x.LastUpdatedTime.Date).Select(x => new TouchlineSubscriptionDetails
            // {
            //     Atp = x.Atp,
            //     High = x.High,
            //     LastUpdatedTime = x.LastUpdatedTime.Date,
            //     Low = x.Low,
            //     Ltp = x.Ltp,
            //     Open = x.Open,
            //     PreviousClose = x.PreviousClose,
            //     PreviousOiclose = x.PreviousOiclose,
            //     Symbol = x.Symbol,
            //     SymbolId = x.SymbolId,
            //     TickVolume = x.TickVolume,
            //     TodayOi = x.TodayOi,
            //     TotalVolume = x.TotalVolume,
            //     TurnOver = x.TurnOver,
            //     Change = x.Ltp - x.PreviousClose,
            //     ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
            //     OiChange = x.TodayOi - x.PreviousOiclose,
            //     OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            // }).Distinct().ToListAsync();


        }

        public async Task<List<TouchlineSubscriptionDetailsWithMto>> GetTouchlineWithMTOBulk(List<string> symbols, DateTime fromDate, DateTime toDate)
        {
            var touchlines = new List<TouchlineSubscriptionDetailsWithMto>();

            foreach (var symbol in symbols)
            {
                touchlines.AddRange(await TouchlineWithMTO(symbol, fromDate, toDate));
            }

            return touchlines;
        }
        public async Task<SegmentTouchline> GetSegmentTouchline(string segment, DateTime date)
        {
            var rediskey = $"SEGMENTTOUCHLINE_{segment}_{date:dd-MM-yyyy}";
            var redisValue = await redisBL.GetValue(rediskey);

            if (redisValue != null)
            {
                return JsonConvert.DeserializeObject<SegmentTouchline>(redisValue);
            }

            var st = dbMaster.SegmentTouchlines.FirstOrDefault(s => s.Segment == segment && s.LastUpdatedTime.Date == date.Date);
            await redisBL.SetValue(rediskey, JsonConvert.SerializeObject(st)); ;

            return st;


        }
        public async Task<List<SegmentTouchline>> GetSegmentTouchline(string Segment, DateTime fromdate, DateTime todate)
        {

            var results = new List<SegmentTouchline>();
            var dates = new List<DateTime>();
            for (var date = fromdate; date <= todate; date = date.AddDays(1))
            {
                dates.Add(date);

            }
            var _touchlines = await Task.WhenAll(dates.Select(date => redisBL.GetValue($"SEGMENTTOUCHLINE_{Segment}_{date:dd-MM-yyyy}")));
            foreach (var touchline in _touchlines.Where(t => t != null))
            {
                try
                {
                    //if (touchline != null)
                    //{
                    var _t = JsonConvert.DeserializeObject<SegmentTouchline>(touchline);
                    results.Add(_t);
                    //}
                }
                catch
                {
                    var __ = results;
                }
            }
            var newLines = dates.Where(x => !results.Where(t => t != null).Select(t => t.LastUpdatedTime.Date).Contains(x)).ToList();
            await UpdateSegmentTouchlineRedis(newLines, Segment);
            var _newtouchlines = await Task.WhenAll(newLines.Select(date => redisBL.GetValue($"SEGMENTTOUCHLINE_{Segment}_{date:dd-MM-yyyy}")));
            foreach (var touchline in _newtouchlines.Where(t => t != null))
            {
                try
                {
                    //if (touchline != null)
                    //{
                    var _t = JsonConvert.DeserializeObject<SegmentTouchline>(touchline);
                    results.Add(_t);
                    //}
                }
                catch
                {
                    var __ = results;
                }
            }
            return results.Where(x => x.LastUpdatedTime.DayOfWeek != DayOfWeek.Saturday && x.LastUpdatedTime.DayOfWeek != DayOfWeek.Sunday).ToList();
            //return dbMaster.SegmentTouchlines.Where(s => s.Segment == Segment && s.LastUpdatedTime.Date <= todate.Date && s.LastUpdatedTime.Date > todate.Date);
        }

        public List<BreadthDetails> GetBreadth()
        {
            var response = new List<BreadthDetails>();
            var results = (from b in db.Breadths group b by b.LastTradeTime into g select new { g.Key, values = g.ToList() }).ToList();
            foreach (var row in results)
            {
                response.Add(new BreadthDetails
                {
                    Time = row.Key,
                    AdvanceOpen = row.values.Where(x => x.ChangeWrtOpen > 0).Count(),
                    DeclineOpen = row.values.Where(x => x.ChangeWrtOpen < 0).Count(),
                    AdvanceChange = row.values.Where(x => x.Change > 0).Count(),
                    DeclineChange = row.values.Where(x => x.Change < 0).Count(),
                    AdvanceATP = row.values.Where(x => x.ChangeWrtAtp > 0).Count(),
                    DeclineATP = row.values.Where(x => x.ChangeWrtAtp < 0).Count(),
                });

            }
            return response;
        }

        public async Task<List<IScalping>> GetScalping(List<string> symbols)
        {
            return await db.HistorySubscriptions.Where(x => symbols.Contains(x.Symbol)).OrderBy(x => x.LastTradeTime).Select(x => new IScalping
            {
                Open = x.Open,
                Low = x.Low,
                High = x.High,
                OpenInterest = x.OpenInterest,
                Atp = x.Atp,
                Close = x.Close,
                LastTradeTime = x.LastTradeTime,
                Volume = x.Volume,
                Symbol = x.Symbol
            }).ToListAsync();
        }

        public List<IActiveOI> GetActiveOI(string symbol, DateTime expiry, int strike)
        {
            var results = new List<IActiveOI>();
            var param = new SqlParameter[]
            {
                new SqlParameter()
                {
                    ParameterName = "Symbol",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = symbol
                },
                new SqlParameter()
                {
                    ParameterName = "Expiry",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = expiry
                },
                new SqlParameter()
                {
                    ParameterName = "Strike",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = strike
                }
            };
            using var connection = new SqlConnection(db.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sproc_GET_ACTIVE_OI]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new IActiveOI
                    {
                        LastTradeTime = Convert.ToDateTime(reader["LastTradeTime"]),
                        CEOI = reader.IsDBNull("CEOI") ? 0 : reader.GetInt32("CEOI"),
                        PEOI = reader.IsDBNull("PEOI") ? 0 : reader.GetInt32("PEOI"),
                        FutureVwap = reader.IsDBNull("FutVwap") ? 0 : reader.GetDecimal("FutVwap"),
                        FutureLTP = reader.IsDBNull("FutLtp") ? 0 : reader.GetDecimal("FutLtp"),
                    });
                }
            }
            connection.Close();
            return results;
        }
        public List<IActiveVOL> GetActiveVOL(string symbol, DateTime expiry, int strike)
        {
            var results = new List<IActiveVOL>();
            var param = new SqlParameter[]
            {
                new SqlParameter()
                {
                    ParameterName = "Symbol",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = symbol
                },
                new SqlParameter()
                {
                    ParameterName = "Expiry",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = expiry
                },
                new SqlParameter()
                {
                    ParameterName = "Strike",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = strike
                }
            };
            using var connection = new SqlConnection(db.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sproc_GET_ACTIVE_VOLUME]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new IActiveVOL
                    {
                        LastTradeTime = Convert.ToDateTime(reader["LastTradeTime"]),
                        CEVOL = reader.IsDBNull("CEVOL") ? 0 : reader.GetInt64("CEVOL"),
                        PEVOL = reader.IsDBNull("PEVOL") ? 0 : reader.GetInt64("PEVOL"),
                        FutureVwap = reader.IsDBNull("FutVwap") ? 0 : reader.GetDecimal("FutVwap"),
                    });
                }
            }
            connection.Close();
            return results;
        }

        public List<IPremiumDecay> GetPremiumDecay(string symbol, DateTime expiry, int strike)
        {
            var results = new List<IPremiumDecay>();
            var param = new SqlParameter[]
            {
                new SqlParameter()
                {
                    ParameterName = "Symbol",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = symbol
                },
                new SqlParameter()
                {
                    ParameterName = "Expiry",
                    SqlDbType=SqlDbType.DateTime,
                    Direction = ParameterDirection.Input,
                    Value = expiry
                },
                new SqlParameter()
                {
                    ParameterName = "Strike",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = strike
                }
            };
            using var connection = new SqlConnection(db.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sproc_GET_PREMIUM_DECAY]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new IPremiumDecay
                    {
                        LastTradeTime = Convert.ToDateTime(reader["LastTradeTime"]),
                        CEDecay = reader.GetDecimal("CEDecay"),
                        PEDecay = reader.GetDecimal("PEDecay"),
                        Average = reader.GetDecimal("Average"),
                    });
                }
            }
            connection.Close();
            return results;
        }

        public async Task<IExpiryOI> GetExpiryOI(string stock, DateTime expiry)
        {
            var symbol = db.Stocks.FirstOrDefault(s => s.DisplayName == stock).Name;
            List<string> symbols = new();
            List<DBModels.TouchlineSubscription> touchlines = new();
            var dateString = expiry.ToString("yyMMdd");
            symbols.Add($"{symbol}{dateString}");

            var _result = new IExpiryOI();
            var key = $"ExpiryOI_{symbol}_{expiry}";

            var redisValue = await redisBL.GetValue(key);
            if (redisValue != null)
            {
                _result = JsonConvert.DeserializeObject<IExpiryOI>(redisValue);
            }

            _result = new IExpiryOI
            {
                COICE = db.TouchlineSubscriptions.Where(t => t.Symbol.StartsWith($"{symbol}{dateString}") && t.Symbol.EndsWith("CE")).Sum(x => x.TodayOi - x.PreviousOiclose),
                OICE = db.TouchlineSubscriptions.Where(t => t.Symbol.StartsWith($"{symbol}{dateString}") && t.Symbol.EndsWith("CE")).Sum(x => x.TodayOi),
                VolumeCE = db.TouchlineSubscriptions.Where(t => t.Symbol.StartsWith($"{symbol}{dateString}") && t.Symbol.EndsWith("CE")).Sum(x => x.TotalVolume),
                Expiry = expiry,
                OIPE = db.TouchlineSubscriptions.Where(t => t.Symbol.StartsWith($"{symbol}{dateString}") && t.Symbol.EndsWith("PE")).Sum(x => x.TodayOi),
                COIPE = db.TouchlineSubscriptions.Where(t => t.Symbol.StartsWith($"{symbol}{dateString}") && t.Symbol.EndsWith("PE")).Sum(x => x.TodayOi - x.PreviousOiclose),
                VolumePE = db.TouchlineSubscriptions.Where(t => t.Symbol.StartsWith($"{symbol}{dateString}") && t.Symbol.EndsWith("PE")).Sum(x => x.TotalVolume)
            };
            bool afterMarket = false;
            if ((DateTime.Now.Minute > 30 && DateTime.Now.Hour == 21) || DateTime.Now.Hour > 21 || DateTime.Now.Hour < 9 || (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 15))
                afterMarket = true;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                afterMarket = true;
            var holidays = await GetHolidays();
            if (holidays.Any(h => h.Date == DateTime.Now.Date)) afterMarket = true;
            if (afterMarket)
                await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
            else
                await redisBL.SetValueWithExpiry(key, JsonConvert.SerializeObject(_result), 1);

            return _result;

        }

        #region Option Window

        #endregion

        public async Task<List<TouchlineSubscription1minDetails>> Get1minTouchline(List<string> Symbols)
        {

            return await db.HistorySubscriptions.Where(x => Symbols.Contains(x.Symbol)).OrderByDescending(x => x.LastTradeTime).Take(1).Select(x => new TouchlineSubscription1minDetails
            {
                Atp = x.Atp,
                High = x.High,
                Low = x.Low,
                Ltp = x.Close,
                Open = x.Open,
                SymbolId = db.Symbols.FirstOrDefault(s => s.Symbol1 == x.Symbol).SymbolId,
                Symbol = x.Symbol,

            }).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetTouchlineOption()
        {
            var marketholiday = await GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);
            return await db.TouchlineSubscriptions.Where(x =>
            x.TodayOi > 0 &&
            x.LastUpdatedTime > tradetime
            && !((x.Symbol.Contains("RECLTD"))
            || (x.Symbol.Contains("MANAPPURAM"))
            || (x.Symbol.Contains("GAIL"))
            || (x.Symbol.Contains("PFC"))
            || (x.Symbol.Contains("IOC"))
            || (x.Symbol.Contains("NMDC"))
            || (x.Symbol.Contains("TATAPOWER"))
            || (x.Symbol.Contains("ONGC"))
            || (x.Symbol.Contains("L&TFH"))
            || (x.Symbol.Contains("IDFCFIRSTB"))
            || (x.Symbol.Contains("SAIL"))
            || (x.Symbol.Contains("FEDERALBNK"))
            || (x.Symbol.Contains("BHEL"))
            || (x.Symbol.Contains("BANKBARODA"))
            || (x.Symbol.Contains("PNB"))
            || (x.Symbol.Contains("NATIONALUM"))
            || (x.Symbol.Contains("GMRINFRA"))
            || (x.Symbol.Contains("IDEA"))
            || (x.Symbol.Contains("NIFTY")))
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && x.TotalVolume != 0).Select(x => new TouchlineSubscriptionDetails
            {
                Atp = x.Atp,
                High = x.High,
                LastUpdatedTime = x.LastUpdatedTime,
                Low = x.Low,
                Ltp = x.Ltp,
                Open = x.Open,
                PreviousClose = x.PreviousClose,
                PreviousOiclose = x.PreviousOiclose,
                Symbol = x.Symbol,
                SymbolId = x.SymbolId,
                TickVolume = x.TickVolume,
                TodayOi = x.TodayOi,
                TotalVolume = x.TotalVolume,
                TurnOver = x.TotalVolume * x.Atp,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.TurnOver).Take(40).ToListAsync();
        }
        public async Task<List<TouchlineSubscriptionDetails>> GetTouchlineIndexWriting()
        {
            var marketholiday = await GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);
            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && ((x.Symbol.Contains("BANKNIFTY"))
            || (x.Symbol.Contains("NIFTY")))
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && x.TotalVolume > 100000 && x.TodayOi > 100000 && x.PreviousClose != 0 && ((x.Ltp - x.PreviousClose) * 100 / x.PreviousClose) < -30).Select(x => new TouchlineSubscriptionDetails
            {
                Atp = x.Atp,
                High = x.High,
                LastUpdatedTime = x.LastUpdatedTime,
                Low = x.Low,
                Ltp = x.Ltp,
                Open = x.Open,
                PreviousClose = x.PreviousClose,
                PreviousOiclose = x.PreviousOiclose,
                Symbol = x.Symbol,
                SymbolId = x.SymbolId,
                TickVolume = x.TickVolume,
                TodayOi = x.TodayOi,
                TotalVolume = x.TotalVolume,
                TurnOver = x.TurnOver,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.TotalVolume).Take(40).ToListAsync();
        }


        public async Task<List<TouchlineSubscriptionDetails>> GetTouchlineIndexOption()
        {
            var marketholiday = await GetMarketHoliday();
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                marketholiday = -1;
            }
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                marketholiday = -2;
            }
            else if (DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 9)
            {
                marketholiday = -1;
            }
            else
            {
                marketholiday *= -1;
            }
            DateTime tradetime = DateTime.Now.Date.AddDays(marketholiday).AddHours(9).AddMinutes(15);
            return await db.TouchlineSubscriptions.Where(x =>
            x.LastUpdatedTime > tradetime
            && ((x.Symbol.Contains("NIFTY") || x.Symbol.Contains("BANKNIFTY") || x.Symbol.Contains("BANKNIFTY"))
            && (x.Symbol.EndsWith("CE") || x.Symbol.EndsWith("PE"))
            && x.TotalVolume != 0)).Select(x => new TouchlineSubscriptionDetails
            {
                Atp = x.Atp,
                High = x.High,
                LastUpdatedTime = x.LastUpdatedTime,
                Low = x.Low,
                Ltp = x.Ltp,
                Open = x.Open,
                PreviousClose = x.PreviousClose,
                PreviousOiclose = x.PreviousOiclose,
                Symbol = x.Symbol,
                SymbolId = x.SymbolId,
                TickVolume = x.TickVolume,
                TodayOi = x.TodayOi,
                TotalVolume = x.TotalVolume,
                TurnOver = x.TurnOver,
                Id = x.Id,
                Change = x.Ltp - x.PreviousClose,
                ChangePercentage = x.PreviousClose != 0 ? (x.Ltp - x.PreviousClose) * 100 / x.PreviousClose : 0,
                OiChange = x.TodayOi - x.PreviousOiclose,
                OiChangePercentage = x.PreviousOiclose != 0 ? (x.TodayOi - x.PreviousOiclose) * 100 / x.PreviousOiclose : 0,
            }).OrderByDescending(x => x.TotalVolume).Take(20).ToListAsync();
        }

        #region MasterDataSync
        public async Task<List<DBModels.HistorySubscription>> GetDataForMaster()
        {
            var date = DateTime.Now.Date;
            return await db.HistorySubscriptions.Where(x => (x.Symbol.ToUpper().Contains("NIFTY") || x.Symbol.ToUpper().Contains("BANKNIFTY"))).ToListAsync();
        }

        #endregion
        public async Task<List<DateTime>> GetHolidays()
        {
            var key = "holidays";
            var value = await redisBL.GetValue(key);
            if (value != null)
                return JsonConvert.DeserializeObject<List<Holiday>>(value).Select(x => x.Date).ToList();

            var holidays = await db.Holidays.ToListAsync();
            await redisBL.SetValue(key, JsonConvert.SerializeObject(holidays));
            return holidays.Select(x => x.Date).OrderByDescending(x => x.Date).ToList();

        }

        public async Task<List<Ivdatum>> GetIvdata(DateTime date)
        {
            return await db.Ivdata.Where(x => x.UpdatedOn.Date == date.Date).OrderBy(x => x.Symbol).ToListAsync();
        }
        //public async Task<Ivdatum> GetIvdata(string symbol, DateTime date)
        //{
        //    var key = $"IV_{symbol}_{date:dd-MM-yyyy}";
        //    var result = await redisBL.GetValue(key);
        //    if (result != null)
        //    {
        //        return JsonConvert.DeserializeObject<Ivdatum>(result);
        //    }
        //    var res = db.Ivdata.FirstOrDefault(x => x.Symbol == symbol && x.UpdatedOn.Date == date.Date);
        //    await redisBL.SetValue(key, JsonConvert.SerializeObject(res));
        //    return res;
        //}
        public async Task<List<Ivdatum>> GetIvdata(GetIVDataByDateRequest request)
        {
            return await db.Ivdata.Where(x => request.Symbols.Contains(x.Symbol)
            && x.Expiry.Value.Date >= request.Expiry
            && x.UpdatedOn.Date >= request.FromDate.Date
            && x.UpdatedOn.Date <= request.ToDate.Date).ToListAsync();
        }
        public List<Volatility> GetVolatility(GetVolatilityRequest request)
        {
            var res = dbMaster.Volatilities.Where(x => x.Symbol == request.Symbol
            && x.Date.Date >= request.FromDate.Date
            && x.Date.Date <= request.ToDate.Date).ToList();
            if (request.Type.ToLower().Equals("monthly"))
                return dbMaster.Volatilities.Where(x => x.Symbol == request.Symbol
                && x.Date.Date >= request.FromDate.Date
                && x.Date.Date <= request.ToDate.Date).ToList().Where(x => IsLastOfMonth(x.Date)).ToList();
            return res;
        }
        public List<Ivdatum> GetIvdataWeeklyMonthly(GetIVDataByDateRequestWeeklyMonthly request)
        {
            if (request.Type.ToLower().Equals("weekly"))
            {
                return db.Ivdata.Where(x => x.Symbol == request.Symbol && x.Expiry.Value.Date <= x.UpdatedOn.Date.AddDays(7) && x.UpdatedOn.Date >= request.FromDate.Date
                 && x.UpdatedOn.Date <= request.ToDate.Date).OrderBy(x => x.Expiry).ToList();
            }

            var res = db.Ivdata.Where(x =>
            x.Symbol == request.Symbol
            && x.UpdatedOn.Date >= request.FromDate.Date
            && x.UpdatedOn.Date <= request.ToDate.Date).ToList();

            if (request.Type.ToLower().Equals("monthly"))
                return db.Ivdata.Where(x => x.Symbol == request.Symbol
                && x.UpdatedOn.Date >= request.FromDate.Date
                && x.UpdatedOn.Date <= request.ToDate.Date).ToList().Where(x => IsLastOfMonth(x.Expiry.Value)).ToList();
            return res;
        }

        public async Task<List<DBModels.MaxPain>> GetMaxPain(List<string> Symbols, DateTime Expiry, DateTime fromTime, DateTime toTime)
        {
            bool afterMarket = false;
            if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                afterMarket = true;
            if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
                afterMarket = false;

            var _result = new List<DBModels.MaxPain>();
            var key = $"MaxPain_{JsonConvert.SerializeObject(Symbols)}_{Expiry.Date}_{fromTime.Date}_{toTime.Date}";
            var time = DateTime.Now; //10000000 = 1 sec
            var timekey = key + "_time";
            var redisValue = await redisBL.GetValue(key);
            if (redisValue != null)
            {
                var timekeyvalue = await redisBL.GetValue(timekey);

                if (timekeyvalue != null)
                {
                    if (afterMarket)
                    {
                        _result = JsonConvert.DeserializeObject<List<DBModels.MaxPain>>(redisValue);
                        return _result;
                    }
                    else if (DateTime.Parse(timekeyvalue).AddHours(1) > time)
                    {
                        _result = JsonConvert.DeserializeObject<List<DBModels.MaxPain>>(redisValue);
                        return _result;
                    }

                }
            }

            _result = await db.MaxPains.Where(x => Symbols.Contains(x.Stock) && x.UpdatedOn.Date >= fromTime.Date && x.UpdatedOn.Date <= toTime.Date && x.Expiry.Date == Expiry.Date && x.UpdatedOn.Hour > 9 && x.UpdatedOn.Hour < 16).ToListAsync();
            await redisBL.SetValue(key, JsonConvert.SerializeObject(_result));
            await redisBL.SetValue(timekey, _result.OrderByDescending(x => x.UpdatedOn).FirstOrDefault().UpdatedOn.ToString());
            return _result;
        }


        public async Task<MovingAverageDetails> GetMovingAverage(string Symbol)
        {
            var key = $"GETMOVINGAVERAGE_{Symbol}";
            var value = await redisBL.GetValue(key);

            if (value != null)
            {
                return JsonConvert.DeserializeObject<MovingAverageDetails>(value);
            }
            var result = new MovingAverageDetails
            {
                DMA20 = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-20)).Select(x => x.Ltp).Average(),
                DMA50 = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-50)).Select(x => x.Ltp).Average(),
                DMA100 = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-100)).Select(x => x.Ltp).Average(),
                DMA200 = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-200)).Select(x => x.Ltp).Average(),
                DMA52WH = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-52 * 7)).Select(x => x.High).Max(),
                DMA52WL = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-52 * 7)).Select(x => x.Low).Min(),
                CMP = db.TouchlineSubscriptions.Where(x => x.Symbol == Symbol).OrderByDescending(x => x.LastUpdatedTime).FirstOrDefault().Ltp,
                LastTime = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol).Select(x => x.LastUpdatedTime).OrderByDescending(x => x).FirstOrDefault()
            };

            bool afterMarket = false;
            if ((DateTime.Now.Minute > 30 && DateTime.Now.Hour == 21) || DateTime.Now.Hour > 21 || DateTime.Now.Hour < 9 || (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 15))
                afterMarket = true;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                afterMarket = true;
            var holidays = await GetHolidays();
            if (holidays.Any(h => h.Date == DateTime.Now.Date)) afterMarket = true;
            if (afterMarket)
                await redisBL.SetValue(key, JsonConvert.SerializeObject(result));
            else
                await redisBL.SetValueWithExpiry(key, JsonConvert.SerializeObject(result), 5);

            return result;
        }
        public async Task<MovingAverageDetails> GetMovingExponential(string Symbol)
        {
            var key = $"GETMOVINGEXPONENTIAL_{Symbol}";
            var value = await redisBL.GetValue(key);

            if (value != null)
            {
                return JsonConvert.DeserializeObject<MovingAverageDetails>(value);
            }
            var result = new MovingAverageDetails
            {
                DMA20 = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-20)).Select(x => x.Ltp).ToList().Aggregate((ema, nextQuote) => (2 / 21) * nextQuote + (1 - (2 / 21)) * ema),
                DMA50 = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-50)).Select(x => x.Ltp).ToList().Aggregate((ema, nextQuote) => (2 / 51) * nextQuote + (1 - (2 / 51)) * ema),
                DMA100 = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-100)).Select(x => x.Ltp).ToList().Aggregate((ema, nextQuote) => (2 / 101) * nextQuote + (1 - (2 / 101)) * ema),
                DMA200 = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-200)).Select(x => x.Ltp).ToList().Aggregate((ema, nextQuote) => (2 / 201) * nextQuote + (1 - (2 / 201)) * ema),
                DMA52WH = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-52 * 7)).Select(x => x.High).Max(),
                DMA52WL = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol && x.LastUpdatedTime > DateTime.Now.AddDays(-52 * 7)).Select(x => x.Low).Min(),
                CMP = db.TouchlineSubscriptions.Where(x => x.Symbol == Symbol).OrderByDescending(x => x.LastUpdatedTime).FirstOrDefault().Ltp,
                LastTime = dbMaster.TouchlineSubscriptionsStocks.Where(x => x.Symbol == Symbol).Select(x => x.LastUpdatedTime).OrderByDescending(x => x).FirstOrDefault()
            };
            bool afterMarket = false;
            if ((DateTime.Now.Minute > 30 && DateTime.Now.Hour == 21) || DateTime.Now.Hour > 21 || DateTime.Now.Hour < 9 || (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 15))
                afterMarket = true;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                afterMarket = true;
            var holidays = await GetHolidays();
            if (holidays.Any(h => h.Date == DateTime.Now.Date)) afterMarket = true;
            if (afterMarket)
                await redisBL.SetValue(key, JsonConvert.SerializeObject(result));
            else
                await redisBL.SetValueWithExpiry(key, JsonConvert.SerializeObject(result), 5);

            return result;
        }



        public async Task<FutureDashboard> GetFutureDashboard()
        {
            //bool afterMarket = false;
            //if (DateTime.Now.Hour < 9 || (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 16 || DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            //    afterMarket = true;
            //if (DateTime.Now.Minute > 30 && DateTime.Now.Hour == 19 && DateTime.Now.Hour < 22)
            //    afterMarket = false;
            var _result = new FutureDashboard();
            var key = $"FutureDashboard";
            var time = DateTime.Now; //10000000 = 1 sec
            var redisValue = await redisBL.GetValue(key);
            if (redisValue != null)
            {
                //if (afterMarket)
                //{
                //    _result = JsonConvert.DeserializeObject<FutureDashboard>(redisValue);
                //    return _result;
                //}
                _result = JsonConvert.DeserializeObject<FutureDashboard>(redisValue);
                return _result;

            }
            var _stocks = await GetStocks();
            var stocks = _stocks.Select(s => $"{s.DisplayName}-I").ToList();
            var touchline = await GetTouchline(stocks);
            _result = new FutureDashboard
            {
                OIGainer = touchline.OrderByDescending(t => t.OiChangePercentage).Take(5).ToList(),
                OILooser = touchline.OrderBy(t => t.OiChangePercentage).Take(5).ToList(),
                PriceGainer = touchline.OrderByDescending(t => t.ChangePercentage).Take(5).ToList(),
                PriceLooser = touchline.OrderBy(t => t.ChangePercentage).Take(5).ToList(),
                MostActiveByVolume = touchline.OrderByDescending(t => t.TickVolume).Take(5).ToList(),
                LongBuildUp = touchline.Where(t => t.OiChangePercentage > 0 && t.ChangePercentage > 0)
                .OrderByDescending(t => t.OiChangePercentage).ThenByDescending(t => t.ChangePercentage).Take(5).ToList(),
                ShortBuildUp = touchline.Where(t => t.OiChangePercentage > 0 && t.ChangePercentage < 0)
                .OrderByDescending(t => t.OiChangePercentage).ThenBy(t => t.ChangePercentage).Take(5).ToList(),
            };
            await redisBL.SetValueWithExpiry(key, JsonConvert.SerializeObject(_result), 5);
            return _result;
        }


        public async Task<List<IFutureRollover>> GetFutureRollOver(string expiry)
        {
            var key = $"FutureRollOver_{expiry}";
            var value = await redisBL.GetValue(key);
            if (value != null)
                return JsonConvert.DeserializeObject<List<IFutureRollover>>(value);
            dbMaster.Database.SetCommandTimeout(10000);
            var results = new List<IFutureRollover>();
            var param = new SqlParameter[]
            {

                new SqlParameter()
                {
                    ParameterName = "Expiry",
                    SqlDbType=SqlDbType.VarChar,
                    Direction = ParameterDirection.Input,
                    Value = expiry.ToUpper()
                }
            };
            using var connection = new SqlConnection(dbMaster.Database.GetDbConnection().ConnectionString);
            {
                var cmm = connection.CreateCommand();
                cmm.CommandType = CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sproc_GET_FUTURE_ROLLOVER]";
                cmm.Parameters.AddRange(param);
                cmm.Connection = connection;
                connection.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    results.Add(new IFutureRollover
                    {
                        Date = Convert.ToDateTime(reader["Expiry"]),
                        PreviousOIClose = reader.GetDecimal("PreviousOIClose"),
                        TodayOI = reader.GetDecimal("TodayOI"),
                        Symbol = reader.GetString("Symbol"),
                    });
                }
            }
            connection.Close();

            await redisBL.SetValue(key, JsonConvert.SerializeObject(results));
            return results;
        }


        public async Task<string> SendWhatsapp(string mobile, string template_name, string name, string expiryDate, string amount, string mailId, string otp)
        {
            var parameters = new List<SMSTemplateParams>();

            var template = new SMSTemplate
            {
                broadcast_name = template_name,
                template_name = template_name,
                parameters = parameters
            };
            template.parameters = new List<SMSTemplateParams>()
                    {
                        new SMSTemplateParams { name = "1", value =otp },
                        new SMSTemplateParams { name = "otp", value =otp },
                        new SMSTemplateParams { name = "name", value =name },
                        new SMSTemplateParams { name = "date", value =expiryDate },
                        new SMSTemplateParams { name = "amount", value =amount },
                        new SMSTemplateParams { name = "mail_Id", value =mailId }
                    }.Where(param => !string.IsNullOrWhiteSpace(param.value)) // Filter out blank or null values
.ToList();
            string url = $"https://live-server-115918.wati.io/api/v1/sendTemplateMessage?whatsappNumber={mobile}";
            using var handler = new HttpClientHandler();
            using var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJqdGkiOiJjZWY3MThkMy1kZTc3LTQ5ZWUtOTI5Yy00Yzg2OTQyMWFkY2UiLCJ1bmlxdWVfbmFtZSI6Im1hbm9qQGlmaWwuY28uaW4iLCJuYW1laWQiOiJtYW5vakBpZmlsLmNvLmluIiwiZW1haWwiOiJtYW5vakBpZmlsLmNvLmluIiwiYXV0aF90aW1lIjoiMTAvMTMvMjAyMyAwNTo0MTo0OCIsImRiX25hbWUiOiIxMTU5MTgiLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBRE1JTklTVFJBVE9SIiwiZXhwIjoyNTM0MDIzMDA4MDAsImlzcyI6IkNsYXJlX0FJIiwiYXVkIjoiQ2xhcmVfQUkifQ.JbQKEp9-K9hMGh8PwAJA39R7Qek0_P0DVB-_apyiXAk");
            try
            {
                HttpContent c = new StringContent(JsonConvert.SerializeObject(template), Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync(url, c);
                if (result.IsSuccessStatusCode)
                {
                    var responseStream = await result.Content.ReadAsStreamAsync();
                    if (responseStream != null)
                    {
                        var resultString = new StreamReader(responseStream).ReadToEnd();
                        return resultString;
                    }
                }
                throw new Exception($"{result.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task SendSms(string number, string otp)
        {
            await GETREQUEST($"https://api.msg91.com/api/v5/otp?template_id=615e8b829d896e62a006400e&mobile=91{number}&authkey=368201AY4ebCS9615d9041P1&otp={otp}");
            //var client = new RestClient($"https://api.msg91.com/api/v5/otp?template_id=615e8b829d896e62a006400e&mobile=91{number}&authkey=368201AY4ebCS9615d9041P1&otp={otp}");
            //var request = new RestRequest(Method.Get);
            //request.AddHeader("Content-Type", "application/json");
            //IRestResponse response = client.Execute(request);

            //if (response.IsSuccessful) return true;
            //return false;
        }

        public async Task<string> SendMsg91Email(string template_id, string Var1, string Var2, string ToEmail, string ToName)
        {
            var mailrequest = new MSG91EmailRequest
            {
                template_id = template_id,
                to = new List<MSG91EmailLine>() { new MSG91EmailLine { email = ToEmail, name = ToName } },
                reply_to = new List<MSG91EmailLine>() { new MSG91EmailLine { email = "support@ifil.co.in", name = "IFIL" } },
                domain = "mail.i4option.co.in",
                authkey = "368201AY4ebCS9615d9041P1",
                from = new MSG91EmailLine { email = "mail@i4option.co.in", name = "i4option" },
                mail_type_id = 1,
                variables = new MSG91EmailVariables { VAR1 = Var1, VAR2 = Var2 }
            };
            //var client = new RestClient("https://api.msg91.com/api/v5/email/send");
            //var request = new RestRequest(Method.POST);
            //request.AddHeader("Content-Type", "application/JSON");
            //request.AddHeader("Accept", "application/json");
            //request.AddParameter("application/JSON", JsonConvert.SerializeObject(mailrequest), ParameterType.RequestBody);
            //IRestResponse response = client.Execute(request);
            //return response.Content;

            return await POSTREQUEST("https://api.msg91.com/api/v5/email/send", JsonConvert.SerializeObject(mailrequest));
        }

        public async Task<string> POSTREQUEST(string url, string body)
        {
            using var handler = new HttpClientHandler();
            using var client = new HttpClient(handler);
            try
            {
                HttpContent c = new StringContent(body, Encoding.UTF8, "application/json");
                HttpResponseMessage result = await client.PostAsync(url, c);
                if (result.IsSuccessStatusCode)
                {
                    var responseStream = await result.Content.ReadAsStreamAsync();
                    if (responseStream != null)
                    {
                        var resultString = new StreamReader(responseStream).ReadToEnd();
                        return resultString;
                    }
                }
                throw new Exception($"{result.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> GETREQUEST(string url)
        {
            using var handler = new HttpClientHandler();
            using var client = new HttpClient(handler);

            try
            {
                HttpResponseMessage result = await client.GetAsync(url);
                if (result.IsSuccessStatusCode)
                {
                    var responseStream = await result.Content.ReadAsStreamAsync();
                    if (responseStream != null)
                    {
                        var resultString = new StreamReader(responseStream).ReadToEnd();
                        return resultString;
                        //return JsonConvert.DeserializeObject<ob>(resultString).ID;
                    }
                }
                throw new Exception($"{result.ReasonPhrase}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        private class MaxpainResults
        {
            public int Strike { get; set; }
            public decimal Value { get; set; }
        }
        private class MaxPain
        {
            public decimal CEOI { get; set; }
            public decimal CEValue { get; set; }
            public decimal PEOI { get; set; }
            public decimal PEValue { get; set; }
            public decimal Total { get; set; }
            public int Strike { get; set; }
        }
        private class GetSymbolResponse
        {
            public string Status { get; set; }
            public string[][] Records { get; set; }
        }
        private class GetHistoryResponse
        {
            public string Status { get; set; }
            public string[][] Records { get; set; }
        }
        private class GenerateAuthResponse
        {
            public string access_token { get; set; }

        }

        public class EarningRatioDetailsForCopy
        {
            public string Symbol { get; set; }
            public DateTime ErDate { get; set; }
            public string Index { get; set; }
            public string Open { get; set; }
            public string High { get; set; }
            public string Low { get; set; }
        }
        public class EarningRatioDetails
        {
            public int Id { get; set; }
            public string Symbol { get; set; }
            public DateTime ErDate { get; set; }
            public int? Index { get; set; }
            public int? Open { get; set; }
            public int? High { get; set; }
            public int? Low { get; set; }
            public DateTime CreatedOnUtc { get; set; }
            public DateTime UpdatedOnUtc { get; set; }
        }
        public class MSG91EmailRequest
        {
            public List<MSG91EmailLine> to { get; set; }
            public MSG91EmailLine from { get; set; }
            //public List<MSG91EmailLine> cc { get; set; }
            //public List<MSG91EmailLine> bcc { get; set; }
            public string domain { get; set; }
            public int mail_type_id { get; set; }
            public List<MSG91EmailLine> reply_to { get; set; }
            public string authkey { get; set; }
            public string template_id { get; set; }
            public MSG91EmailVariables variables { get; set; }
        }
        public class MSG91EmailLine
        {
            public string name { get; set; }
            public string email { get; set; }
        }
        public class MSG91EmailVariables
        {
            public string VAR1 { get; set; }
            public string VAR2 { get; set; }
        }
        public class FutureDashboard
        {
            public List<TouchlineSubscriptionDetails> PriceGainer { get; set; }
            public List<TouchlineSubscriptionDetails> PriceLooser { get; set; }
            public List<TouchlineSubscriptionDetails> OIGainer { get; set; }
            public List<TouchlineSubscriptionDetails> OILooser { get; set; }
            public List<TouchlineSubscriptionDetails> LongBuildUp { get; set; }
            public List<TouchlineSubscriptionDetails> ShortBuildUp { get; set; }
            public List<TouchlineSubscriptionDetails> MostActiveByVolume { get; set; }
        }

        #region Helpers
        private async Task UpdateTouchlineRedis(List<string> symbols)
        {
            bool afterMarket = false;
            if ((DateTime.Now.Minute > 30 && DateTime.Now.Hour == 15) || DateTime.Now.Hour > 15 || DateTime.Now.Hour < 9 || (DateTime.Now.Hour == 9 && DateTime.Now.Minute < 15))
                afterMarket = true;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                afterMarket = true;
            var holidays = await GetHolidays();
            if (holidays.Any(h => h.Date == DateTime.Now.Date)) afterMarket = true;

            await Task.WhenAll(db.TouchlineSubscriptions.Where(t => symbols.Contains(t.Symbol))
                .Select(t => afterMarket ? redisBL.SetValue($"TOUCHLINE_{t.Symbol}", JsonConvert.SerializeObject(t)) : redisBL.SetValueWithExpiry($"TOUCHLINE_{t.Symbol}", JsonConvert.SerializeObject(t), 1)));
        }
        private async Task UpdateSegmentTouchlineRedis(List<DateTime> dates, string segment)
        {
            await Task.WhenAll(dbMaster.SegmentTouchlines.Where(t => t.Segment == segment && dates.Contains(t.LastUpdatedTime.Date))
                .Select(t => redisBL.SetValue($"SEGMENTTOUCHLINE_{segment}_{t.LastUpdatedTime:dd-MM-yyyy}", JsonConvert.SerializeObject(t))));
        }
        bool IsLastOfMonth(DateTime date)
        {
            var oneWeekAfter = date.AddDays(7);
            return oneWeekAfter.Month != date.Month;
        }
        #endregion


        #region Excel Customized
        public object GetExcelCalendar()
        {
            return db.Calendars.Where(x => x.Id != 2).Include(x => x.CalendarDates).Select(x =>
            new
            {
                Name = x.Name == "INDEX" ? "NIFTY" : x.Name == "MID CAP NIFTY" ? "MIDCPNIFTY" : x.Name == "BANK NIFTY" ? "BANKNIFTY" : x.Name,
                Expiry = x.CalendarDates.Where(c => c.Active && !c.Deleted).Select(c => c.Date).OrderBy(c => c.Date).ToArray()
            }).ToList();
        }
        #endregion

        #region Helper Models
        private class SMSTemplate
        {
            public string template_name { get; set; }
            public string broadcast_name { get; set; }
            public List<SMSTemplateParams> parameters { get; set; }
        }
        private class SMSTemplateParams
        {
            public string name { get; set; }
            public string value { get; set; }
        }
        #endregion
    }
}
