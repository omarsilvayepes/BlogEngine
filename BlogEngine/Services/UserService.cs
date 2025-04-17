using BlogEngine.Models;
using BlogEngine.Repositories;
using BlogEngine.Settings;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BlogEngine.Services
{
    public class UserService : IUserRepository
    {

        private readonly IMongoCollection<User> mongoCollection;
        private readonly IConfiguration _configuration; // for token

        public UserService(IDatabaseSettings settings, IConfiguration configuration)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            mongoCollection = database.GetCollection<User>(settings.UserCollection);
            _configuration = configuration;
        }
        public async Task<bool> IsRegister(string userName)
        {
            User user = await mongoCollection.FindAsync(new BsonDocument { { "UserName", userName } })
             .Result.FirstOrDefaultAsync();
            if (user != null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponse> Login(string userName, string password,string role)
        {
            User user = await mongoCollection.FindAsync(new BsonDocument { { "UserName", userName } })
             .Result.FirstOrDefaultAsync();
            if (user == null)
            {
                return new LoginResponse { Status = "NoUser" };
            }
            if (!VerificarPassWordHash(password, user.PassWordHash, user.PassWordSalt))
            {
                return new LoginResponse { Status = "PassWordWrong" };
            }
            if (!user.Role.Equals(role))
            {
                return new LoginResponse { Status = "RoleNotAuthorize" };
            }

            var token= await createToken(user);
            return  token;
        }

        public async Task<string> Register(User user, string password)
        {
            try
            {
                if (!await IsRegister(user.UserName))
                {
                    CrearPassWordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PassWordHash = passwordHash;
                    user.PassWordSalt = passwordSalt;
                    await mongoCollection.InsertOneAsync(user);
                    return ("OK");

                }
                return "User already register";
            }
            catch (Exception)
            {
                return "500";
            }
        }

        private void CrearPassWordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)//encrypted pass
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerificarPassWordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }

                }
                return true;
            }
        }

        private async Task<LoginResponse> createToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name,user.UserName),
                new Claim("Role",user.Role)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8
                .GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),// time to expire
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            var ResponseToken = new LoginResponse
            { 
              Token = tokenHandler.WriteToken(token),
              RefreshToken= await generateRefreshToken(user.UserName),
              Status= "Success"
            };

            return ResponseToken;
        }

        private async Task<string> generateRefreshToken(string userName)
        {
            // just for this basic case, the refresh token and the user data are in the same collection

            var refreshTokenValidMins = _configuration.GetValue<int>("AppSettings:RefreshToken");
            var refreshToken = new RefreshToken
            {
                Token=Guid.NewGuid().ToString(),
                Expiry = DateTime.UtcNow.AddMinutes(refreshTokenValidMins)
                //UserId = userId
            };

            //update  user with refresh token in DB
            if(await IsRegister(userName))
            {
                // Find the user in MongoDB
                var filter = Builders<User>.Filter.Eq(u => u.UserName, userName);
                var update = Builders<User>.Update
                    .Set(u => u.Token, refreshToken.Token)
                    .Set(u => u.Expiry, refreshToken.Expiry);

                await mongoCollection.UpdateOneAsync(filter, update);
            }

            return refreshToken.Token;
        }

        public async  Task<LoginResponse?> validateRefreshToken(string token)
        {
            // Find the user with this refresh token
            var filter = Builders<User>.Filter.Eq(u => u.Token, token);
            var user = await mongoCollection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
            {
                return new LoginResponse { Status = "InvalidToken" };
            }

            // Check if refresh token is expired
            if (user.Expiry < DateTime.UtcNow)
            {
                return new LoginResponse { Status = "ExpiredToken" };
            }

            //Clear the old refresh token from DB (one-time use)
            var clearTokenUpdate = Builders<User>.Update
                .Set(u => u.Token, null)
                .Set(u => u.Expiry, DateTime.MinValue);

            await mongoCollection.UpdateOneAsync(filter, clearTokenUpdate);

            // Generate new tokens
            return await  createToken(user);
        }
    }
}
