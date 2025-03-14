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
            ResponseModel<IEnumerable<AddressBookEntryEntity>> response = new ResponseModel<IEnumerable<AddressBookEntryEntity>>();
            var contacts = await _service.GetAllContacts();
            response.Success = true;
            response.Message = "All Entries fetched successfully";
            response.Data = contacts;
            
            return Ok(response);
        }

        /// <summary>
        /// This method is used to fetch a single Address Book entry by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>?</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AddressBookEntryEntity>> GetContactById(int id)
        {
            ResponseModel<AddressBookEntryEntity> response = new ResponseModel<AddressBookEntryEntity>();
            var contact = await _service.GetContactById(id);
            if (contact == null)
            {
                response.Success = false;
                response.Message = $"No contact with {id} is found!";
                response.Data = null;
                return NotFound(response);
            }

            response.Success = true;
            response.Message = $"Here are the contact details of ID {id}";
            response.Data = contact;
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
            ResponseModel<AddressBookEntryEntity> response = new ResponseModel<AddressBookEntryEntity>();
            if (contact == null)
            {
                response.Success = false;
                response.Message = "Invalid contact data.";
                response.Data = null;
                return BadRequest(response);
            }

            await _service.AddContact(contact);

            response.Success = true;
            response.Message = "Contact Added Succedssfully.";
            response.Data = contact;
            return Ok(response);
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
            ResponseModel<AddressBookEntryEntity> response = new ResponseModel<AddressBookEntryEntity>();
            var updatedContact = await _service.UpdateContact(id, contact);
            if (updatedContact == null)
            {
                response.Success = false;
                response.Message = "Contact Not Found.";
                response.Data = null;
                return NotFound(response);
            }

            response.Success = true;
            response.Message = "Contact Updated Succedssfully.";
            response.Data = updatedContact;
            return Ok(response);
        }

        /// <summary>
        /// This method is used to delete an entry from Address Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>?</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            ResponseModel<bool> response = new ResponseModel<bool>();
            var deleted = await _service.DeleteContact(id);
            if (!deleted)
            {
                response.Success = false;
                response.Message = "Contact Not Found.";
                response.Data = false;
                return NotFound(response);
            }
            response.Success = true;
            response.Message = "Contact Deleted Successfully.";
            response.Data = true;
            return Ok(response);
        }
    }
}
