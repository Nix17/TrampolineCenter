using Microsoft.AspNetCore.Mvc;
using TrampolineCenterAPI.Data;
using TrampolineCenterAPI.Models;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using System;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.Logging;
using Sentry;

namespace TrampolineCenterAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<ClientsController> _logger;

        // Sentry
        private readonly IHub _sentryHub;

        private static readonly Counter getContactsCounter = Metrics.CreateCounter("clients_controller_get_contacts_total", "Total number of GET requests to /api/clients");
        private static readonly Counter addContactCounter = Metrics.CreateCounter("clients_controller_add_contact_total", "Total number of POST requests to /api/clients");
        private static readonly Counter updateContactCounter = Metrics.CreateCounter("clients_controller_update_contact_total", "Total number of PUT requests to /api/clients");
        private static readonly Counter deleteContactCounter = Metrics.CreateCounter("clients_controller_delete_contact_total", "Total number of DELETE requests to /api/clients");

        // Метрика с информацией о количестве клиентов
        private static readonly Gauge totalClientsGauge = Metrics.CreateGauge("clients_controller_total_clients", "Total number of clients");

        public ClientsController(ApplicationDbContext dbContext, ILogger<ClientsController> logger, IHub sentryHub)
        {
            this.dbContext = dbContext;
            this._logger = logger;
            this._sentryHub = sentryHub;
        }

        [HttpGet]
        public async Task<IActionResult> GetContacts()
        {
            getContactsCounter.Inc();

            var childSpan = _sentryHub.GetSpan()?.StartChild("additional-work");

            using (Metrics.CreateHistogram("clients_controller_get_contacts_duration_seconds", "Duration of ClientsController.GetContacts method").NewTimer())
            {
                try
                {
                    var clients = await dbContext.Clients.ToListAsync();
                    totalClientsGauge.Set(clients.Count); // Устанавливаем значение метрики

                    // Добавляем лог в Loki
                    _logger.LogInformation("GetContacts method called.");

                    childSpan?.Finish(SpanStatus.Ok);

                    return Ok(clients);
                }
                catch (Exception ex)
                {
                    childSpan?.Finish(ex);
                    // Добавляем лог в Loki при ошибке
                    _logger.LogError(ex, "Error in GetContacts method.");
                    return StatusCode(500, "Internal Server Error");
                }
            }
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetContact([FromRoute] Guid id)
        {
            var childSpan = _sentryHub.GetSpan()?.StartChild("additional-work");
            try
            {
                var contact = await dbContext.Clients.FindAsync(id);

                if (contact == null)
                {
                    return NotFound();
                }

                // Добавляем лог в Loki
                _logger.LogInformation($"GetContact method called for contact with Id: {id}");

                childSpan?.Finish(SpanStatus.Ok);

                return Ok(contact);
            }
            catch (Exception ex)
            {
                childSpan?.Finish(ex);
                // Добавляем лог в Loki при ошибке
                _logger.LogError(ex, "Error in GetContact method.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddContact(AddClientRequest addContactRequest)
        {
            addContactCounter.Inc();

            var childSpan = _sentryHub.GetSpan()?.StartChild("additional-work");

            using (Metrics.CreateHistogram("clients_controller_add_contact_duration_seconds", "Duration of ClientsController.AddContact method").NewTimer())
            {
                try
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

                    // Обновляем значение метрики после добавления клиента
                    var clients = await dbContext.Clients.ToListAsync();
                    totalClientsGauge.Set(clients.Count);

                    // Добавляем лог в Loki
                    _logger.LogInformation($"AddContact method called for contact with Id: {contact.Id}");

                    childSpan?.Finish(SpanStatus.Ok);

                    return Ok(contact);
                }
                catch (Exception ex)
                {
                    childSpan?.Finish(ex);
                    // Добавляем лог в Loki при ошибке
                    _logger.LogError(ex, "Error in AddContact method.");
                    return StatusCode(500, "Internal Server Error");
                }
            }
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateContact([FromRoute] Guid id, UpdateClientRequest updateContactRequest)
        {
            updateContactCounter.Inc();

            var childSpan = _sentryHub.GetSpan()?.StartChild("additional-work");

            using (Metrics.CreateHistogram("clients_controller_update_contact_duration_seconds", "Duration of ClientsController.UpdateContact method").NewTimer())
            {
                try
                {
                    var contact = await dbContext.Clients.FindAsync(id);

                    if (contact != null)
                    {
                        contact.FullName = updateContactRequest.FullName;
                        contact.BirthDate = updateContactRequest.BirthDate;
                        contact.Phone = updateContactRequest.Phone;
                        contact.Email = updateContactRequest.Email;

                        await dbContext.SaveChangesAsync();

                        // Обновляем значение метрики после обновления клиента
                        var clients = await dbContext.Clients.ToListAsync();
                        totalClientsGauge.Set(clients.Count);

                        // Добавляем лог в Loki
                        _logger.LogInformation($"UpdateContact method called for contact with Id: {id}");

                        childSpan?.Finish(SpanStatus.Ok);

                        return Ok(contact);
                    }

                    return NotFound();
                }
                catch (Exception ex)
                {
                    childSpan?.Finish(ex);
                    // Добавляем лог в Loki при ошибке
                    _logger.LogError(ex, "Error in UpdateContact method.");
                    return StatusCode(500, "Internal Server Error");
                }
            }
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteContact([FromRoute] Guid id)
        {
            deleteContactCounter.Inc();

            var childSpan = _sentryHub.GetSpan()?.StartChild("additional-work");

            using (Metrics.CreateHistogram("clients_controller_delete_contact_duration_seconds", "Duration of ClientsController.DeleteContact method").NewTimer())
            {
                try
                {
                    var contact = await dbContext.Clients.FindAsync(id);

                    if (contact != null)
                    {
                        dbContext.Remove(contact);
                        await dbContext.SaveChangesAsync();

                        // Обновляем значение метрики после удаления клиента
                        var clients = await dbContext.Clients.ToListAsync();
                        totalClientsGauge.Set(clients.Count);

                        // Добавляем лог в Loki
                        _logger.LogInformation($"DeleteContact method called for contact with Id: {id}");

                        childSpan?.Finish(SpanStatus.Ok);

                        return Ok(contact);
                    }

                    return NotFound();
                }
                catch (Exception ex)
                {
                    childSpan?.Finish(ex);
                    // Добавляем лог в Loki при ошибке
                    _logger.LogError(ex, "Error in DeleteContact method.");
                    return StatusCode(500, "Internal Server Error");
                }
            }
        }
    }
}
