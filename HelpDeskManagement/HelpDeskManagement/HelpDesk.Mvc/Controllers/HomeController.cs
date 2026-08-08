using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HelpDesk.Mvc.Models;
using HelpDesk.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpDesk.Mvc.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITicketService _ticketService;

        public HomeController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // GET: / (Dashboard) - Admin sees totals across everyone, users see only their own.
        public async Task<IActionResult> Index()
        {
            var allTickets = await _ticketService.GetAllTicketsAsync();

            var relevantTickets = User.IsInRole("Admin")
                ? allTickets
                : allTickets.Where(t => t.RaisedBy == User.Identity.Name).ToList();

            var viewModel = new DashboardViewModel
            {
                TotalTickets = relevantTickets.Count,
                OpenTickets = relevantTickets.Count(t => t.Status == "Open"),
                InProgressTickets = relevantTickets.Count(t => t.Status == "In Progress"),
                ClosedTickets = relevantTickets.Count(t => t.Status == "Closed")
            };

            return View(viewModel);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
