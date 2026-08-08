using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HelpDesk.Mvc.Models;

namespace HelpDesk.Mvc.Services
{
    public class TicketService : ITicketService
    {
        private readonly HttpClient _httpClient;

        public TicketService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Ticket>> GetAllTicketsAsync()
        {
            var tickets = await _httpClient.GetFromJsonAsync<List<Ticket>>("api/Ticket/All");
            return tickets ?? new List<Ticket>();
        }

        public async Task<Ticket> GetTicketByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"api/Ticket/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            return await response.Content.ReadFromJsonAsync<Ticket>();
        }

        public async Task<int> CreateTicketAsync(Ticket ticket)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Ticket", ticket);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<bool> UpdateTicketAsync(Ticket ticket)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/Ticket/{ticket.Id}", ticket);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/Ticket/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<Ticket>> GetTicketsByStatusAsync(string status)
        {
            var tickets = await _httpClient.GetFromJsonAsync<List<Ticket>>($"api/Ticket/Status/{status}");
            return tickets ?? new List<Ticket>();
        }
    }
}
