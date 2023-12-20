using Microsoft.AspNetCore.Mvc;
using TrampolineCenterAPI.Data;
using TrampolineCenterAPI.Models;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using System;
using System.Threading.Tasks;

namespace TrampolineCenterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        private static readonly Counter getContactsCounter = Metrics.CreateCounter("clients_controller_get_contacts_total", "Total number of GET requests to /api/clients");
        private static readonly Counter addContactCounter = Metrics.CreateCounter("clients_controller_add_contact_total", "Total number of POST requests to /api/clients");
        private static readonly Counter updateContactCounter = Metrics.CreateCounter("clients_controller_update_contact_total", "Total number of PUT requests to /api/clients");
        private static readonly Counter deleteContactCounter = Metrics.CreateCounter("clients_controller_delete_contact_total", "Total number of DELETE requests to /api/clients");

        public ClientsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            getContactsCounter.Inc();

            using (Metrics.CreateHistogram("clients_controller_get_contacts_duration_seconds", "Duration of ClientsController.GetContacts method").NewTimer())
            {
                return Ok(await dbContext.Clients.ToListAsync());
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetContact([FromRoute] Guid id)
        {
            var contact = await dbContext.Clients.FindAsync(id);

            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(AddClientRequest addContactRequest)
        {
            addContactCounter.Inc();

            using (Metrics.CreateHistogram("clients_controller_add_contact_duration_seconds", "Duration of ClientsController.AddContact method").NewTimer())
            {
                var contact = new Client()
                {
                    Id = Guid.NewGuid(),
                    BirthDate = addContactRequest.BirthDate,
                    Email = addContactRequest.Email,
                    FullName = addContactRequest.FullName,
                    Phone = addContactRequest.Phone,
                };

                await dbContext.Clients.AddAsync(contact);
                await dbContext.SaveChangesAsync();
                return Ok(contact);
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateContact([FromRoute] Guid id, UpdateClientRequest updateContactRequest)
        {
            updateContactCounter.Inc();

            using (Metrics.CreateHistogram("clients_controller_update_contact_duration_seconds", "Duration of ClientsController.UpdateContact method").NewTimer())
            {
                var contact = await dbContext.Clients.FindAsync(id);

                if (contact != null)
                {
                    contact.FullName = updateContactRequest.FullName;
                    contact.BirthDate = updateContactRequest.BirthDate;
                    contact.Phone = updateContactRequest.Phone;
                    contact.Email = updateContactRequest.Email;

                    await dbContext.SaveChangesAsync();

                    return Ok(contact);
                }

                return NotFound();
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteContact([FromRoute] Guid id)
        {
            deleteContactCounter.Inc();

            using (Metrics.CreateHistogram("clients_controller_delete_contact_duration_seconds", "Duration of ClientsController.DeleteContact method").NewTimer())
            {
                var contact = await dbContext.Clients.FindAsync(id);

                if (contact != null)
                {
                    dbContext.Remove(contact);
                    await dbContext.SaveChangesAsync();
                    return Ok(contact);
                }

                return NotFound();
            }
        }
    }
}
