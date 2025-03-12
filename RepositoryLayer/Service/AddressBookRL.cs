using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Model;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AddressBookRL : IAddressBookRL
    {
        private readonly AdressBookDBContext _context;

        public AddressBookRL(AdressBookDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AddressBookEntryEntity>> GetAllContacts()
        {
            return await _context.AddressBookEntries.ToListAsync();
        }

        public async Task<AddressBookEntryEntity?> GetContactById(int id)
        {
            return await _context.AddressBookEntries.FindAsync(id);
        }

        public async Task<AddressBookEntryEntity> AddContact(AddressBookEntryEntity contact)
        {
            _context.AddressBookEntries.Add(contact);
            await _context.SaveChangesAsync();
            return contact;
        }

        public async Task<AddressBookEntryEntity?> UpdateContact(int id, AddressBookEntryEntity contact)
        {
            var existingContact = await _context.AddressBookEntries.FindAsync(id);
            if (existingContact == null) return null;

            existingContact.Name = contact.Name;
            existingContact.Email = contact.Email;
            existingContact.PhoneNumber = contact.PhoneNumber;
            existingContact.Address = contact.Address;

            await _context.SaveChangesAsync();
            return existingContact;
        }

        public async Task<bool> DeleteContact(int id)
        {
            var contact = await _context.AddressBookEntries.FindAsync(id);
            if (contact == null) return false;

            _context.AddressBookEntries.Remove(contact);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
