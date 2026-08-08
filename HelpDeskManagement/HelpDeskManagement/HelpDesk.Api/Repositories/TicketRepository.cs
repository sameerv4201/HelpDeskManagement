using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Api.Data;
using HelpDesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Api.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            return await _context.Tickets
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            return await _context.Tickets
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<int> CreateTicketAsync(Ticket ticket)
        {
            ticket.CreatedDate = DateTime.Now;
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket.Id;
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            var existing = await _context.Tickets.FindAsync(ticket.Id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Ticket with Id {ticket.Id} was not found.");
            }

            existing.Title = ticket.Title;
            existing.Description = ticket.Description;
            existing.Priority = ticket.Priority;
            existing.Status = ticket.Status;
            existing.RaisedBy = ticket.RaisedBy;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteTicketAsync(int id)
        {
            var existing = await _context.Tickets.FindAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException($"Ticket with Id {id} was not found.");
            }

            _context.Tickets.Remove(existing);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Ticket>> GetTicketsByStatusAsync(string status)
        {
            return await _context.Tickets
                .AsNoTracking()
                .Where(t => t.Status == status)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
}
