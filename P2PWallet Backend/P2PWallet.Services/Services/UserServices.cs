using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using P2PWallet.API.DTO;
using P2PWallet.API;
using P2PWallet.Models;
using P2PWallet.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using P2PWallet.API.Data;
using Microsoft.EntityFrameworkCore;
using P2PWallet.Models.DTO;
using P2PWallet.Models.Entities;


namespace P2PWallet.Services.Services
{
    public class UserServices : IUserServices
    {
        private readonly ILogger<UserServices> _logger;
        private readonly IConfiguration _configuration;
        private readonly P2PDataContext _context;

        public UserServices(ILogger<UserServices> logger, IConfiguration configuration, P2PDataContext context)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public async Task<bool>? CreateUser(UserDTO user)
        {
            var person = new User
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
            };
            await _context.Users.AddAsync(person);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<UserDTO> GetSingleUser(int id)
        {
            var user = await _context.Users.Include("UserAccount").FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                return null;
            }

            var userDTO = new UserDTO
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Accounts = user.UserAccount.Select(account => new Account
                {
                    Balance = account.Balance,
                    Currency = account.Currency,
                    AccountNumber = account.AccountNumber
                }).ToList()
            };

            return userDTO;
        }

        public async Task<List<UserDTO>> GettAllUsers()
        {
            var allUsers = await _context.Users.Include(x => x.UserAccount).ToListAsync();

            var finalList = new List<UserDTO>();

            foreach (var user in allUsers)
            {
                var userDTO = new UserDTO
                {
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    Accounts = user.UserAccount.Select(account => new Account
                    {
                        Balance = account.Balance,
                        Currency = account.Currency,
                        AccountNumber = account.AccountNumber
                    }).ToList()
                };

                finalList.Add(userDTO);
            }

            return finalList;
        }

        public async Task<string> Login(LoginDTO request)
        {
            if (request.Username is null || string.IsNullOrEmpty(request.Password))
            {
                return "Please enter a username or password";
            }

            var findUser = await _context.Users.Where(x => x.Username == request.Username).FirstOrDefaultAsync();

            if (findUser == null)
            {
                return "Incorrect Username or Password!!";
            }

            if (VerifyPasswordHash(request.Password, findUser.PasswordHash, findUser.PasswordSalt))
            {
                return CreateToken(findUser);
            }

            return "Incorrect Username or Password!!";
        }

        public async Task<bool>? RemoveUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SignUp(SignUpDTO request)
        {
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            var model = new User();

            model.FirstName = request.FirstName;
            model.LastName = request.LastName;
            model.Email = request.Email;
            model.Username = request.Username;
            model.PhoneNumber = request.PhoneNumber;
            model.Address = request.Address;
            model.PasswordHash = passwordHash;
            model.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();

            var newUserAcct = new UserAccount
            {
                UserId = model.Id,
                AccountNumber = GenerateAccountNumber(),
                Balance = 10000,
                Currency = "NGN",
            };

            await _context.UserAccounts.AddAsync(newUserAcct);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateUser(int id, UpdateUserDTO updateUserDTO)
        {
            var user = await _context.Users.Where(x => x.Id == id).Include(z => z.UserAccount).FirstOrDefaultAsync();
            if (user == null)
                return false;

            user.FirstName = user.FirstName.ToLower() == updateUserDTO.FirstName.ToLower().Trim() ? user.FirstName : updateUserDTO.FirstName;
            user.LastName = user.LastName.ToLower() == updateUserDTO.LastName.ToLower().Trim() ? user.LastName : updateUserDTO.LastName;
            user.Email = user.Email.ToLower() == updateUserDTO.Email.ToLower().Trim() ? user.Email : updateUserDTO.Email;
            user.Address = user.Address.ToLower() == updateUserDTO.Address.ToLower().Trim() ? user.Address : updateUserDTO.Address;
            user.PhoneNumber = user.PhoneNumber.ToLower() == updateUserDTO.PhoneNumber.ToLower().Trim() ? user.PhoneNumber : updateUserDTO.PhoneNumber;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        private string GenerateAccountNumber()
        {
            var now = DateTime.Now;
            var year = now.Year.ToString().Substring(2); // Last two digits of the year
            var month = now.Month.ToString("D2"); // Two digits of the month
            var day = now.Day.ToString("D2"); // Two digits of the day
            //var minute = now.Minute.ToString("D2"); // Two digits of the minutes
            //var second = now.Second.ToString("D2"); // Two digits of the seconds
            var random = new Random().Next(1000, 10000).ToString(); // Four random numbers

            return $"{year}{month}{day}{random}";
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim("UserId", user.Id.ToString()) // Add the user ID as a claim
            };

            var keyBytes = Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value);
            if (keyBytes.Length < 64)
            {
                throw new ArgumentException("The key must be at least 64 bytes long.");
            }

            var key = new SymmetricSecurityKey(keyBytes);

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        public async Task<bool> CreatePin(int userId, string pin)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            CreatePinHash(pin, out byte[] pinHash, out byte[] pinSalt);
            user.PinHash = pinHash;
            user.PinSalt = pinSalt;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePin(int userId, string oldPin, string newPin)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null || !VerifyPinHash(oldPin, user.PinHash, user.PinSalt))
            {
                return false;
            }

            CreatePinHash(newPin, out byte[] pinHash, out byte[] pinSalt);
            user.PinHash = pinHash;
            user.PinSalt = pinSalt;

            await _context.SaveChangesAsync();
            return true;
        }

        private void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        private bool VerifyPinHash(string pin, byte[] pinHash, byte[] pinSalt)
        {
            using (var hmac = new HMACSHA512(pinSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
                return computedHash.SequenceEqual(pinHash);
            }
        }
    }

}