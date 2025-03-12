using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using RepositoryLayer.Service;

namespace BusinessLayer.Service
{
    //These are all the methods for CRUD operation.
    public class AddressBookBL : IAddressBookBL
    {
        IAddressBookRL _repository;

        public AddressBookBL(IAddressBookRL repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AddressBookEntryEntity>> GetAllContacts() => await _repository.GetAllContacts();
        public async Task<AddressBookEntryEntity?> GetContactById(int id) => await _repository.GetContactById(id);
        public async Task<AddressBookEntryEntity> AddContact(AddressBookEntryEntity contact) => await _repository.AddContact(contact);
        public async Task<AddressBookEntryEntity?> UpdateContact(int id, AddressBookEntryEntity contact) => await _repository.UpdateContact(id, contact);
        public async Task<bool> DeleteContact(int id) => await _repository.DeleteContact(id);
    }
}
