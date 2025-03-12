using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;
using RepositoryLayer.Service;

namespace BusinessLayer.Interface
{
    //This is the interface for AddressBook BL.
    public interface IAddressBookBL
    {
        Task<IEnumerable<AddressBookEntryEntity>> GetAllContacts();
        Task<AddressBookEntryEntity?> GetContactById(int id);
        Task<AddressBookEntryEntity> AddContact(AddressBookEntryEntity contact);
        Task<AddressBookEntryEntity?> UpdateContact(int id, AddressBookEntryEntity contact);
        Task<bool> DeleteContact(int id);
    }
}
