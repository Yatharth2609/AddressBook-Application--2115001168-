using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLayer.Helper;
using BusinessLayer.Helpers;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using StackExchange.Redis;

namespace BusinessLayer.Service
{
    public class UserBL : IUserBL
    {
        private readonly IUserRL _userRepository;
        private readonly JwtTokenGenerator _jwtTokenGenerator;
        private readonly EmailService _emailService;
        private readonly IDatabase _cache;
        private readonly TimeSpan _cacheExpiration;
        public UserBL(IUserRL userRepository, JwtTokenGenerator jwtTokenGenerator, EmailService emailService, IConnectionMultiplexer redis, IConfiguration config)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
            _cache = redis.GetDatabase();
            _cacheExpiration = TimeSpan.FromMinutes(int.Parse(config["Redis:CacheExpirationMinutes"] ?? "10"));
        }

        public async Task Register(UserEntity user)
        {
            await _userRepository.RegisterUser(user);
            string cacheKey = $"User:{user.Email}";
            await _cache.KeyDeleteAsync(cacheKey);
        }

        public async Task<UserEntity> Login(UserDTO userDTO)
        {
            string cacheKey = $"User:{userDTO.Email}";

            // Check if user data is cached
            var cachedData = await _cache.StringGetAsync(cacheKey);
            if (!cachedData.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<UserEntity>(cachedData);
            }

            // Fetch user from database
            var user = await _userRepository.GetUserByEmail(userDTO.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDTO.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            // Cache user data
            await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(user), _cacheExpiration);

            return user;
        }

        public async Task<string> GenerateResetTokenAsync(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null) throw new Exception("User not found.");

            string resetToken = Guid.NewGuid().ToString();
            string cacheKey = $"ResetToken:{email}";

            // Store reset token in Redis with expiration
            await _cache.StringSetAsync(cacheKey, resetToken, TimeSpan.FromMinutes(15));

            return resetToken;
        }

        public async Task<bool> VerifyResetTokenAsync(string email, string token)
        {
            string cacheKey = $"ResetToken:{email}";

            // Fetch token from cache
            var cachedToken = await _cache.StringGetAsync(cacheKey);
            if (cachedToken.IsNullOrEmpty || cachedToken != token)
            {
                return false; // Invalid or expired token
            }

            // Remove token from cache after verification
            await _cache.KeyDeleteAsync(cacheKey);
            return true;
        }
    }
