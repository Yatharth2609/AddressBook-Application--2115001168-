using BusinessLayer.Interface;
using BusinessLayer.Service;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;

namespace AddressBookApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressBookController : ControllerBase
    {
        IAddressBookBL _service;
        public AddressBookController(IAddressBookBL service) 
        {
            _service = service;
        }


        /// <summary>
        /// This Method is used to fetch all the AddressBook Entries
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressBookEntryEntity>>> GetAllContacts()
        {
            var contacts = await _service.GetAllContacts();
            return Ok(contacts);
        }

        /// <summary>
        /// This method is used to fetch a single Address Book entry by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>?</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressBookEntryEntity>> GetContactById(int id)
        {
            var contact = await _service.GetContactById(id);
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        /// <summary>
        /// This method is used to add an entry to Address Book
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>?</returns>
        [HttpPost]
        public async Task<ActionResult<AddressBookEntryEntity>> AddContact([FromBody] AddressBookEntryEntity contact)
        {
            if(contact == null)
            {
                return BadRequest("Invalid contact data.");
            }

            var addedContact = await _service.AddContact(contact);

            return CreatedAtAction(nameof(GetContactById), new { id = addedContact.Id }, addedContact);
        }

        /// <summary>
        /// This method is used to update any field of a particular Address Book Entry
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns>?</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] AddressBookEntryEntity contact)
        {
            var updatedContact = await _service.UpdateContact(id, contact);
            if (updatedContact == null) return NotFound("Contact not found.");
            return Ok(updatedContact);
        }

        /// <summary>
        /// This method is used to delete an entry from Address Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>?</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var deleted = await _service.DeleteContact(id);
            if (!deleted) return NotFound("Contact not found.");
            return NoContent();
        }
    }
}
