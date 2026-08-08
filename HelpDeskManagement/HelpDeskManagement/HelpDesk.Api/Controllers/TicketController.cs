using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpDesk.Api.Models;
using HelpDesk.Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // GET: api/Ticket/All
        [HttpGet("All")]
        public async Task<ActionResult<List<Ticket>>> GetAll()
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync();
            return Ok(tickets);
        }

        // GET: api/Ticket/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Ticket>> GetById(int id)
        {
            var ticket = await _ticketRepository.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound($"Ticket with Id {id} was not found.");
            }

            return Ok(ticket);
        }

        // POST: api/Ticket
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] Ticket ticket)
        {
            if (ticket == null)
            {
                return BadRequest("Ticket data is required.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newId = await _ticketRepository.CreateTicketAsync(ticket);
            return Ok(newId);
        }

        // PUT: api/Ticket/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ticket ticket)
        {
            if (ticket == null || id != ticket.Id)
            {
                return BadRequest("Ticket Id mismatch or missing data.");
            }

            try
            {
                await _ticketRepository.UpdateTicketAsync(ticket);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Ticket with Id {id} was not found.");
            }
        }

        // DELETE: api/Ticket/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _ticketRepository.DeleteTicketAsync(id);
                return Ok();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Ticket with Id {id} was not found.");
            }
        }

        // GET: api/Ticket/Status/{status}
        [HttpGet("Status/{status}")]
        public async Task<ActionResult<List<Ticket>>> GetByStatus(string status)
        {
            var tickets = await _ticketRepository.GetTicketsByStatusAsync(status);
            return Ok(tickets);
        }
    }
}
