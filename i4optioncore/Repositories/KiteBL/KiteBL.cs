using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using KiteConnect;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public class KiteBL : IKiteBL
    {
        // Persist these tokens in database or settings
        string MyAPIKey = "lrerpvt8qkh3plcj";
        string MySecret = "lhvk9fcwqr9t0jk6xmpn5f1jqeb4cour";
        Kite kite;
        I4optionUserDbContext dbuser;

        private static List<Instrument> InstrumentList = new();
        public KiteBL(I4optionUserDbContext _dbuser)
        {
            dbuser = _dbuser;
            kite = new(MyAPIKey, Debug: true);


            kite.SetAccessToken(GetAccessToken());
            kite.SetSessionExpiryHook(() =>
            {
                login();
                throw new Exception("Need to login again");
            });

            if (InstrumentList.Count == 0)
            {
                try
                {
                    InstrumentList = kite.GetInstruments();
                }
                catch
                { }
            }
            // Example call for functions like "GetHoldings" that returns a data structure
            //List<Holding> holdings = kite.GetHoldings();
            //Console.WriteLine(holdings[0].AveragePrice);
        }
        public decimal CalculateMargin(List<CalculateMarginRequest> request)
        {
            //Kite kite = new(MyAPIKey, Debug: true);



            string[] exchanges = { "NSE", "NFO" };
            List<OrderMarginParams> OrderMarginParams = new();

            request.ForEach(req =>
            {
                var symbols = JsonConvert.SerializeObject(InstrumentList);
                var exchange = InstrumentList.Where(x => exchanges.Contains(x.Exchange)).FirstOrDefault(x => x.TradingSymbol == req.Symbol).Exchange;
                OrderMarginParams.Add(new OrderMarginParams
                {
                    Exchange = exchange,
                    OrderType = "MARKET",
                    TransactionType = req.TransactionType.ToUpper(),
                    Price = req.Price,
                    Quantity = req.Quantity,
                    TradingSymbol = req.Symbol,
                    TriggerPrice = req.TriggerPrice,
                    Variety = "regular",
                    Product = "MIS"
                });
            });

            var response = kite.GetBasketMargins(OrderMarginParams);
            return response.Final.Total;
        }
        string login()
        {
            Kite kite = new(MyAPIKey, Debug: true);
            KiteConnect.User user = kite.GenerateSession(GetRequestToken(), MySecret);
            updateAccessToken(user.AccessToken);
            return user.AccessToken;
        }
        public async Task UpdateRequestToken(string requestToken)
        {
            await dbuser.Configurations.Where(x => x.Key == "zerodha_request_token")
            .ForEachAsync(x =>
            {
                x.Value = requestToken;
                x.UpdatedOnUtc = DateTime.Now;
            });
            await dbuser.SaveChangesAsync();
        }
        void updateAccessToken(string accessToken)
        {
            dbuser.Configurations.FirstOrDefault(x => x.Key == "zerodha_access_token").Value = accessToken;
            dbuser.SaveChanges();
        }
        string GetRequestToken()
        {
            return dbuser.Configurations.FirstOrDefault(x => x.Key == "zerodha_request_token").Value;
        }
        string GetAccessToken()
        {
            return dbuser.Configurations.FirstOrDefault(x => x.Key == "zerodha_access_token").Value;
        }


        public List<Historical> GetHistory(string symbol, DateTime FromDate, DateTime ToDate, string Interval)
        {
            //Kite kite = new(MyAPIKey, Debug: true);

            //kite.SetAccessToken(GetAccessToken());
            //kite.SetSessionExpiryHook(() =>
            //{
            //    login();
            //    throw new Exception("Need to login again");
            //});
            var instrumenttoken = InstrumentList.FirstOrDefault(x => x.TradingSymbol == symbol).InstrumentToken;
            return kite.GetHistoricalData(instrumenttoken.ToString(), FromDate, ToDate, Interval, false, true);
        }
    }
}
