using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;
using StackExchange.Redis;

namespace BusinessLayer.Service
{
    //These are all the methods for CRUD operation.
    public class AddressBookBL : IAddressBookBL
    {
        IAddressBookRL _repository;
        private readonly IDatabase _cache;
        private readonly TimeSpan _cacheExpiration;

        public AddressBookBL(IAddressBookRL repository, IConnectionMultiplexer redis, IConfiguration config)
        {
            _repository = repository;
            _cache = redis.GetDatabase();
            _cacheExpiration = TimeSpan.FromMinutes(int.Parse(config["Redis:CacheExpirationMinutes"] ?? "10"));
        }

        public async Task<IEnumerable<AddressBookEntryEntity>> GetAllContacts() 
        {
            string cacheKey = "AddressBook:AllContacts";

            // Check if data exists in Redis cache
            var cachedData = await _cache.StringGetAsync(cacheKey);
            if (!cachedData.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<List<AddressBookEntryEntity>>(cachedData);
            }

            // If not in cache, fetch from database
            var contacts = await _repository.GetAllContacts();

            // Store data in cache
            await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(contacts), _cacheExpiration);

            return contacts;
        }
        public async Task<AddressBookEntryEntity?> GetContactById(int id) => await _repository.GetContactById(id);
        public async Task AddContact(AddressBookEntryEntity contact)
        {
            await _repository.AddContact(contact);
            await _cache.KeyDeleteAsync("AddressBook:AllContacts"); // Clear cache
        }
        public async Task<AddressBookEntryEntity?> UpdateContact(int id, AddressBookEntryEntity contact) => await _repository.UpdateContact(id, contact);
        public async Task<bool> DeleteContact(int id) => await _repository.DeleteContact(id);
    }
}
