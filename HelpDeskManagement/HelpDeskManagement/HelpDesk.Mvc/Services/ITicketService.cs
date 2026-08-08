using System.Collections.Generic;
using System.Threading.Tasks;
using HelpDesk.Mvc.Models;

namespace HelpDesk.Mvc.Services
{
    public interface ITicketService
    {
        Task<List<Ticket>> GetAllTicketsAsync();

        Task<Ticket> GetTicketByIdAsync(int id);

        Task<int> CreateTicketAsync(Ticket ticket);

        Task<bool> UpdateTicketAsync(Ticket ticket);

        Task<bool> DeleteTicketAsync(int id);

        Task<List<Ticket>> GetTicketsByStatusAsync(string status);
    }
}
