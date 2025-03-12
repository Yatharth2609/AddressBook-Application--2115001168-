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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressBookEntryEntity>>> GetAllContacts()
        {
            var contacts = await _service.GetAllContacts();
            return Ok(contacts);
        }

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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] AddressBookEntryEntity contact)
        {
            var updatedContact = await _service.UpdateContact(id, contact);
            if (updatedContact == null) return NotFound("Contact not found.");
            return Ok(updatedContact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var deleted = await _service.DeleteContact(id);
            if (!deleted) return NotFound("Contact not found.");
            return NoContent();
        }
    }
}
