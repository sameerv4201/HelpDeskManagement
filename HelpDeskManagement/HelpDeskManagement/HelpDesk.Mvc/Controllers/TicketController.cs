using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Mvc.Models;
using HelpDesk.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HelpDesk.Mvc.Controllers
{
    /// <summary>
    /// "My Tickets" for the signed-in user: they can only ever see, create,
    /// edit, close or delete tickets they personally raised. Admin-wide
    /// control over every ticket lives in AdminController instead.
    /// </summary>
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;

        private static readonly List<string> PriorityOptions = new() { "Low", "Medium", "High" };
        private static readonly List<string> AllStatusOptions = new() { "Open", "In Progress", "Closed" };

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: /Ticket -> only tickets raised by the current user
        public async Task<IActionResult> Index()
        {
            var allTickets = await _ticketService.GetAllTicketsAsync();
            var myTickets = allTickets.Where(t => t.RaisedBy == User.Identity.Name).ToList();
            return View(myTickets);
        }

        // GET: /Ticket/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null || !CanAccess(ticket))
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: /Ticket/Create
        public IActionResult Create()
        {
            ViewBag.PriorityOptions = new SelectList(PriorityOptions);
            return View();
        }

        // POST: /Ticket/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Ticket ticket)
        {
            // Status always starts Open; RaisedBy is always the logged-in user - never client-editable.
            ticket.Status = "Open";
            ticket.RaisedBy = User.Identity.Name;
            ModelState.Remove(nameof(Ticket.Status));
            ModelState.Remove(nameof(Ticket.RaisedBy));

            if (!ModelState.IsValid)
            {
                ViewBag.PriorityOptions = new SelectList(PriorityOptions, ticket.Priority);
                return View(ticket);
            }

            await _ticketService.CreateTicketAsync(ticket);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Ticket/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null || !CanAccess(ticket))
            {
                return NotFound();
            }

            ViewBag.PriorityOptions = new SelectList(PriorityOptions, ticket.Priority);
            ViewBag.StatusOptions = new SelectList(GetAllowedStatusOptions(ticket), ticket.Status);
            ViewBag.IsAdmin = User.IsInRole("Admin");
            return View(ticket);
        }

        // POST: /Ticket/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return BadRequest();
            }

            var existing = await _ticketService.GetTicketByIdAsync(id);
            if (existing == null || !CanAccess(existing))
            {
                return NotFound();
            }

            // A non-admin owner may only move their ticket to "Closed" -
            // they cannot reopen it or set it to "In Progress" (that's Admin-only).
            if (!User.IsInRole("Admin") && ticket.Status != existing.Status && ticket.Status != "Closed")
            {
                ModelState.AddModelError(string.Empty, "You can only close your own tickets.");
            }

            // RaisedBy can never be changed from the Edit form.
            ticket.RaisedBy = existing.RaisedBy;

            if (!ModelState.IsValid)
            {
                ViewBag.PriorityOptions = new SelectList(PriorityOptions, ticket.Priority);
                ViewBag.StatusOptions = new SelectList(GetAllowedStatusOptions(existing), ticket.Status);
                ViewBag.IsAdmin = User.IsInRole("Admin");
                return View(ticket);
            }

            await _ticketService.UpdateTicketAsync(ticket);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Ticket/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null || !CanAccess(ticket))
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: /Ticket/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null || !CanAccess(ticket))
            {
                return NotFound();
            }

            await _ticketService.DeleteTicketAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Ticket/CloseTicket/5 - quick action for the owning user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CloseTicket(int id)
        {
            var ticket = await _ticketService.GetTicketByIdAsync(id);
            if (ticket == null || !CanAccess(ticket))
            {
                return NotFound();
            }

            ticket.Status = "Closed";
            await _ticketService.UpdateTicketAsync(ticket);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Ticket/FilterByStatus?status=Open - scoped to the current user's own tickets
        public async Task<IActionResult> FilterByStatus(string status)
        {
            ViewBag.StatusOptions = new SelectList(AllStatusOptions, status);
            ViewBag.SelectedStatus = status;

            if (string.IsNullOrEmpty(status))
            {
                return View(new List<Ticket>());
            }

            var allTickets = await _ticketService.GetTicketsByStatusAsync(status);
            var myTickets = allTickets.Where(t => t.RaisedBy == User.Identity.Name).ToList();
            return View(myTickets);
        }

        private bool CanAccess(Ticket ticket)
        {
            return User.IsInRole("Admin") || ticket.RaisedBy == User.Identity.Name;
        }

        private List<string> GetAllowedStatusOptions(Ticket ticket)
        {
            if (User.IsInRole("Admin"))
            {
                return AllStatusOptions;
            }

            // Non-admin owners can only move a ticket to Closed.
            return new List<string> { ticket.Status, "Closed" }.Distinct().ToList();
        }
    }
}
