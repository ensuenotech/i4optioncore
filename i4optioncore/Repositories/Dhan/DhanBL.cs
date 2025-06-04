using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Security.Policy;
using Microsoft.AspNetCore.Hosting;
using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

namespace i4optioncore.Repositories.Dhan
{
    public class DhanBL : IDhanBL
    {
        private readonly I4optionUserDbContext dbuser;
        private readonly IWebHostEnvironment env;
        private readonly ICacheBL cacheBL;

        public DhanBL(I4optionUserDbContext dbuser, IWebHostEnvironment env, ICacheBL cacheBL)
        {
            this.dbuser = dbuser;
            this.env = env;
            this.cacheBL = cacheBL;
            var _ifDhanSymbolsDownloaded = cacheBL.GetValue("dhansymbolsdownloaded");
            if (_ifDhanSymbolsDownloaded == null)
            {
                //DownloadDhanSymbols().Wait();
            }

        }
        public async Task UpdateDhanCredentials(IDhanCredentialsRequest request)
        {
            var res = dbuser.DhanCredentials.FirstOrDefault(d => d.UserId == request.UserId) ?? new DhanCredential();
            res.DhanClientId = request.ClientId;
            res.DhanUcc = request.ClientUcc;
            if (res.Id == 0)
            {
                res.UserId = request.UserId;
                dbuser.DhanCredentials.Add(res);
            }
            await dbuser.SaveChangesAsync();
        }
        public async Task UpdateDhanToken(string token)
        {

            string url = $"https://auth.dhan.co/partner/consume-consent?tokenId={token}";

            using var httpClientHandler = new HttpClientHandler();

            using var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("partner_id", "0a9d3d34");
            client.DefaultRequestHeaders.Add("partner_secret", "78ffd6ab-87d7-4003-91a6-f894668b3f9d");

            using var result = await client.GetAsync(url);

            if (result.IsSuccessStatusCode)
            {
                var responseStream = await result.Content.ReadAsStreamAsync();
                if (responseStream != null)
                {
                    var resultString = new StreamReader(responseStream).ReadToEnd();
                    var tokenDetails = JsonConvert.DeserializeObject<IUpdateDhanToken>(resultString);
                    dbuser.DhanCredentials.Where(d => d.DhanUcc == tokenDetails.dhanClientUcc
                    && d.DhanClientId == tokenDetails.dhanClientId).ToList().ForEach(res =>
                    {
                        if (res != null)
                        {
                            res.TokenId = token;
                            res.AccessToken = tokenDetails.accessToken;
                        }
                    });
                    await dbuser.SaveChangesAsync();

                }
            }
            else
            {
                throw new Exception(result.ReasonPhrase);
            }
        }
        public async Task<string> GenerateDhanConsent()
        {

            string url = $"https://auth.dhan.co/partner/generate-consent";

            using var httpClientHandler = new HttpClientHandler();

            using var client = new HttpClient(httpClientHandler);
            client.DefaultRequestHeaders.Add("partner_id", "0a9d3d34");
            client.DefaultRequestHeaders.Add("partner_secret", "78ffd6ab-87d7-4003-91a6-f894668b3f9d");

            using var result = await client.GetAsync(url);

            if (result.IsSuccessStatusCode)
            {
                var responseStream = await result.Content.ReadAsStreamAsync();
                if (responseStream != null)
                {
                    var resultString = new StreamReader(responseStream).ReadToEnd();
                    return JsonConvert.DeserializeObject<IDhanConsent>(resultString).consentId;
                }
            }
            else
            {
                throw new Exception(result.ReasonPhrase);
            }
            return null;
        }
        public async Task<List<DhanModel.OrderDetails>> GetOrders(int userId)
        {
            var token = dbuser.DhanCredentials.FirstOrDefault(u => u.UserId == userId)?.AccessToken;
            if (token != null)
            {
                string url = $"https://api.dhan.co/orders";

                using var httpClientHandler = new HttpClientHandler();

                using var client = new HttpClient(httpClientHandler);
                client.DefaultRequestHeaders.Add("access-token", token);

                using var result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    var responseStream = await result.Content.ReadAsStreamAsync();
                    if (responseStream != null)
                    {
                        var resultString = new StreamReader(responseStream).ReadToEnd();
                        return JsonConvert.DeserializeObject<List<DhanModel.OrderDetails>>(resultString);

                    }
                    return null;
                }
                else
                {
                    throw new Exception(result.ReasonPhrase);
                }
            }
            return null;
        }
        public async Task<List<DhanModel.PositionDetails>> GetPositions(int userId)
        {
            var token = dbuser.DhanCredentials.FirstOrDefault(u => u.UserId == userId)?.AccessToken;
            if (token != null)
            {
                string url = $"https://api.dhan.co/positions";

                using var httpClientHandler = new HttpClientHandler();

                using var client = new HttpClient(httpClientHandler);
                client.DefaultRequestHeaders.Add("access-token", token);

                using var result = await client.GetAsync(url);

                if (result.IsSuccessStatusCode)
                {
                    var responseStream = await result.Content.ReadAsStreamAsync();
                    if (responseStream != null)
                    {
                        var resultString = new StreamReader(responseStream).ReadToEnd();
                        return JsonConvert.DeserializeObject<List<DhanModel.PositionDetails>>(resultString);

                    }
                    return null;
                }
                else
                {
                    throw new Exception(result.ReasonPhrase);
                }
            }
            return null;
        }
        public async Task PlaceOrder(DhanModel.DhanOrderRequest request)
        {
            var userId = dbuser.UserTokens.FirstOrDefault(u => u.Token == request.AuthToken.Trim())?.UserId;
            if (userId.HasValue)
            {
                var symbols = GetSymbols();
                request.securityId = symbols.FirstOrDefault(s => s.SEM_TRADING_SYMBOL == request.tradingSymbol)?.SEM_SMST_SECURITY_ID;
                request.correlationId = Guid.NewGuid().ToString();
                if (!string.IsNullOrEmpty(request.securityId))
                {
                    var token = dbuser.DhanCredentials.FirstOrDefault(u => u.UserId == userId)?.AccessToken;
                    if (token != null)
                    {

                        await AddDhanOrderToDB(request, userId.Value);

                        string url = $"https://api.dhan.co/orders";

                        using var httpClientHandler = new HttpClientHandler();

                        using var client = new HttpClient(httpClientHandler);
                        client.DefaultRequestHeaders.Add("access-token", token);
                        var body = JsonConvert.SerializeObject(request, Formatting.Indented);
                        HttpContent c = new StringContent(body, Encoding.UTF8, "application/json");

                        using var result = await client.PostAsync(url, c);
                        var __ = JsonConvert.SerializeObject(result);
                        if (result.IsSuccessStatusCode)
                        {
                            var responseStream = await result.Content.ReadAsStreamAsync();
                            if (responseStream != null)
                            {
                                var resultString = new StreamReader(responseStream).ReadToEnd();

                            }
                        }
                        else
                        {
                            throw new Exception(result.ReasonPhrase);
                        }
                    }
                }
            }
        }
        public async Task UpdateDhanOrder(DhanModel.OrderDetails request)
        {
            await dbuser.DhanOrders.Where(x => x.CorrelationId == request.CorrelationId).ForEachAsync(x =>
            {
                x.Status = request.OrderStatus;
            });
            await dbuser.SaveChangesAsync();
        }
        public async Task DownloadDhanSymbols()
        {
            using var handler = new HttpClientHandler();
            using var client = new HttpClient(handler);

            try
            {
                Directory.CreateDirectory(env.ContentRootPath + "/Assets/Dhan");
                var url = "https://images.dhan.co/api-data/api-scrip-master.csv";
                var localDestinationFilePath = $"{env.ContentRootPath}/Assets/Dhan/dhansymbols{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.csv";
                HttpResponseMessage result = await client.GetAsync(url); ;
                if (result.IsSuccessStatusCode)
                {
                    var responseStream = await result.Content.ReadAsStreamAsync();
                    if (responseStream != null)
                    {
                        var buffer = new byte[2048];
                        //var resultString = new StreamReader(responseStream).ReadToEnd();
                        var fileStream = new FileStream(localDestinationFilePath, FileMode.Create);

                        while (true)
                        {
                            if (responseStream == null) continue;
                            var bytesRead = responseStream.Read(buffer, 0, buffer.Length);

                            if (bytesRead == 0)
                                break;

                            fileStream.Write(buffer, 0, bytesRead);
                        }
                        fileStream.Close();

                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private List<DhanModel.DhanSymbolDetails> GetSymbols()
        {
            var localDestinationFilePath = $"{env.ContentRootPath}/Assets/Dhan/dhansymbols{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.csv";
            var csv = new List<string[]>();
            var lines = File.ReadAllLines(localDestinationFilePath);
            foreach (string line in lines)
                csv.Add(line.Split(','));

            //var properties = lines[0].Split(',');

            //var listObjResult = new List<Dictionary<string, string>>();
            var results = new List<DhanModel.DhanSymbolDetails>();

            for (int i = 1; i < lines.Length; i++)
            {
                results.Add(new DhanModel.DhanSymbolDetails
                {
                    SEM_EXM_EXCH_ID = csv[i][0].Trim(),
                    SEM_SEGMENT = csv[i][1].Trim(),
                    SEM_SMST_SECURITY_ID = csv[i][2].Trim(),
                    SEM_INSTRUMEMT_NAME = csv[i][3].Trim(),
                    SEM_EXPIRY_CODE = csv[i][4].Trim(),
                    SEM_TRADING_SYMBOL = csv[i][5].Trim(),
                    SEM_LOT_UNITS = csv[i][6].Trim(),
                    SEM_CUSTOM_SYMBOL = csv[i][7].Trim(),
                });
                //var objResult = new Dictionary<string, string>();
                //for (int j = 0; j < properties.Length; j++)
                //    objResult.Add(properties[j], csv[i][j]);

                //listObjResult.Add(objResult);
            }
            return results;
            //var json = JsonConvert.SerializeObject(listObjResult);
        }

        private async Task AddDhanOrderToDB(DhanModel.DhanOrderRequest request, int userId)
        {
            dbuser.DhanOrders.Add(new DhanOrder
            {
                BoprofitValue = request.boProfitValue,
                BostopLossValue = request.boStopLossValue,
                DisclosedQuantity = request.disclosedQuantity,
                DrvexpiryDate = request.drvExpiryDate,
                DrvoptionType = request.drvOptionType,
                DrvstrikePrice = request.drvStrikePrice,
                ExchangeSegment = request.exchangeSegment,
                OrderType = request.orderType,
                Price = request.price,
                ProductType = request.productType,
                Quantity = request.quantity,
                SecurityId = request.securityId,
                CorrelationId = request.correlationId,
                TradingSymbol = request.tradingSymbol,
                TransactionType = request.transactionType,
                TriggerPrice = request.triggerPrice,
                UserId = userId
            });
            await dbuser.SaveChangesAsync();
        }
    }
}
