using System.IdentityModel.Tokens.Jwt;
using System;
using i4optioncore.Repositories;
using System.Linq;
using i4optioncore.DBModelsUser;
using System.Threading.Tasks;

namespace i4optioncore.Services
{
    public class AuthService : IAuthService
    {
        private readonly ICacheBL cacheService;
        private readonly I4optionUserDbContext db;

        public AuthService(ICacheBL cacheService, I4optionUserDbContext db)
        {
            this.cacheService = cacheService;
            this.db = db;
        }
        public async Task LogOut(int userId)
        {
            db.UserTokens.RemoveRange(db.UserTokens.Where(x => x.UserId == userId));
            await db.SaveChangesAsync();
        }
        public int GetUserId(string jwtToken)
        {
            jwtToken = jwtToken.Split(' ')[1];
            //if (!IsTokenValid(jwtToken)) throw new UnauthorizedAccessException("TOKEN_NOT_FOUND");
            // Parse the JWT token
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);
            _ = int.TryParse(jwtSecurityToken.Claims.Where(x => x.Type == "nameid").FirstOrDefault()?.Value, out int id);
            return id;
        }
        public bool IsTokenValid(string token)
        {
            var key = $"USER_TOKEN_{token}";
            // Check if token exists in cache
            var cachedToken = cacheService.GetValue(token);
            if (cachedToken != null)
            {
                return true; // Token is valid
            }


            // Token not found in cache, check database
            var tokenFromDb = db.UserTokens.FirstOrDefault(x => x.Token == token)?.Token;
            if (tokenFromDb != null)
            {
                // Cache the token with an appropriate expiration time
                cacheService.SetValue(key, tokenFromDb, 10);
                return true; // Token is valid
            }



            return false; // Token is not valid
        }

    }
}
