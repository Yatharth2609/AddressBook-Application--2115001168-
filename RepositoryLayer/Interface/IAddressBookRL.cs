using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;

namespace RepositoryLayer.Interface
{
    public interface IAddressBookRL
    {
        Task<IEnumerable<AddressBookEntryEntity>> GetAllContacts();
        Task<AddressBookEntryEntity?> GetContactById(int id);
        Task<AddressBookEntryEntity> AddContact(AddressBookEntryEntity contact);
        Task<AddressBookEntryEntity?> UpdateContact(int id, AddressBookEntryEntity contact);
        Task<bool> DeleteContact(int id);
    }
}
