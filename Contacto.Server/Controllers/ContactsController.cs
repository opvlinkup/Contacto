using Contacto.Server.Models.DTOs;
using Contacto.Server.Models.Entities;
using Contacto.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Contacto.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContactsController(IContactService contactService, ILogger<ContactsController> logger) : ControllerBase
{
    private readonly IContactService _contactService = contactService;
    private readonly ILogger<ContactsController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        try
        {
            var contacts = await _contactService.GetAllAsync(cancellationToken);
            return Ok(contacts);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to retrieve all contacts.");
            return StatusCode(500, "An error occurred while retrieving contacts.");
        }
    }

    [HttpGet("p")]
    public async Task<IActionResult> GetPaginated(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var contacts =
                await _contactService.GetPaginatedAsync(skip, take, search, sortBy, ascending, cancellationToken);
            return Ok(contacts);
        }
        catch (OperationCanceledException)
        {
            return StatusCode(499, "Client closed request.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to retrieve paginated contacts.");
            return StatusCode(500, "An error occurred while retrieving contacts.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactService.GetAsync(id, cancellationToken);
            return Ok(contact);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Contact with id {id} not found.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to retrieve contact {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the contact.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Contact contact, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _contactService.AddAsync(contact, cancellationToken);
            return CreatedAtAction(nameof(Get), new { id = contact.Id }, contact);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create contact.");
            return StatusCode(500, "An error occurred while creating the contact.");
        }
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ContactUpdateDto updateDto,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            Console.WriteLine($"Received birthDate: {updateDto.BirthDate}");

            var updatedContact = await _contactService.UpdateAsync(updateDto, cancellationToken);
            return Ok(updatedContact);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (KeyNotFoundException)
        {
            return NotFound($"Contact with id {id} not found.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update contact {Id}", id);
            return StatusCode(500, "An error occurred while updating the contact.");
        }
    }

    [HttpDelete("{guid}")]
    public async Task<IActionResult> Delete(Guid guid, CancellationToken cancellationToken)
    {
        try
        {
            await _contactService.DeleteAsync(guid, cancellationToken);
            return NoContent();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound($"Contact with guid {guid} not found or already deleted.");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete contact {Id}", guid);
            return StatusCode(500, "An error occurred while deleting the contact.");
        }
    }
}