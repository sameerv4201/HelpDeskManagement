using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ITicketService _ticketService;

        public AdminController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: /Admin  -> every ticket in the system, regardless of who raised it
        public async Task<IActionResult> Index()
        {
            var tickets = await _ticketService.GetAllTicketsAsync();
            return View(tickets.OrderByDescending(t => t.CreatedDate).ToList());
        }

        // POST: /Admin/SetInProgress/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetInProgress(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = "In Progress";
            await _ticketService.UpdateTicketAsync(ticket);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/SetClosed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetClosed(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = "Closed";
            await _ticketService.UpdateTicketAsync(ticket);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Reopen/5  (convenience: move a ticket back to Open)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reopen(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ticket.Status = "Open";
            await _ticketService.UpdateTicketAsync(ticket);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
