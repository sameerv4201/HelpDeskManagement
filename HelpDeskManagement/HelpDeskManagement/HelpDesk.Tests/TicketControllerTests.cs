using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpDesk.Api.Controllers;
using HelpDesk.Api.Models;
using HelpDesk.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HelpDesk.Tests
{
    public class TicketControllerTests
    {
        private readonly Mock<ITicketRepository> _mockRepo;
        private readonly TicketController _controller;

        public TicketControllerTests()
        {
            _mockRepo = new Mock<ITicketRepository>();
            _controller = new TicketController(_mockRepo.Object);
        }

        private static Ticket SampleTicket(int id = 1, string status = "Open") => new Ticket
        {
            Id = id,
            Title = "Sample Ticket",
            Description = "Sample Description",
            Priority = "Medium",
            Status = status,
            RaisedBy = "test.user",
            CreatedDate = DateTime.Now
        };

        // ---------- Mandatory Test Cases ----------

        // 1. GetAllTickets_ReturnsOkResult_WhenTicketsExist
        [Fact]
        public async Task GetAllTickets_ReturnsOkResult_WhenTicketsExist()
        {
            var tickets = new List<Ticket> { SampleTicket(1), SampleTicket(2) };
            _mockRepo.Setup(r => r.GetAllTicketsAsync()).ReturnsAsync(tickets);

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTickets = Assert.IsAssignableFrom<List<Ticket>>(okResult.Value);
            Assert.Equal(2, returnedTickets.Count);
        }

        // 2. GetTicketById_ReturnsOkResult_WhenTicketExists
        [Fact]
        public async Task GetTicketById_ReturnsOkResult_WhenTicketExists()
        {
            var ticket = SampleTicket(1);
            _mockRepo.Setup(r => r.GetTicketByIdAsync(1)).ReturnsAsync(ticket);

            var result = await _controller.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTicket = Assert.IsType<Ticket>(okResult.Value);
            Assert.Equal(1, returnedTicket.Id);
        }

        // 3. GetTicketById_ReturnsNotFound_WhenTicketDoesNotExist
        [Fact]
        public async Task GetTicketById_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            _mockRepo.Setup(r => r.GetTicketByIdAsync(It.IsAny<int>())).ReturnsAsync((Ticket)null);

            var result = await _controller.GetById(99);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        // 4. CreateTicket_ReturnsOkResult_WhenTicketIsCreatedSuccessfully
        [Fact]
        public async Task CreateTicket_ReturnsOkResult_WhenTicketIsCreatedSuccessfully()
        {
            var ticket = SampleTicket(0);
            _mockRepo.Setup(r => r.CreateTicketAsync(It.IsAny<Ticket>())).ReturnsAsync(1);

            var result = await _controller.Create(ticket);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(1, okResult.Value);
        }

        // 5. CreateTicket_ReturnsBadRequest_WhenTicketIsNull
        [Fact]
        public async Task CreateTicket_ReturnsBadRequest_WhenTicketIsNull()
        {
            var result = await _controller.Create(null);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        // 6. GetTicketsByStatus_ReturnsOkResult_WhenMatchingTicketsExist
        [Fact]
        public async Task GetTicketsByStatus_ReturnsOkResult_WhenMatchingTicketsExist()
        {
            var tickets = new List<Ticket> { SampleTicket(1, "Open"), SampleTicket(2, "Open") };
            _mockRepo.Setup(r => r.GetTicketsByStatusAsync("Open")).ReturnsAsync(tickets);

            var result = await _controller.GetByStatus("Open");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTickets = Assert.IsAssignableFrom<List<Ticket>>(okResult.Value);
            Assert.All(returnedTickets, t => Assert.Equal("Open", t.Status));
        }

        // ---------- Optional Test Cases ----------

        // 7. UpdateTicket_ReturnsOkResult_WhenUpdateIsSuccessful
        [Fact]
        public async Task UpdateTicket_ReturnsOkResult_WhenUpdateIsSuccessful()
        {
            var ticket = SampleTicket(1);
            _mockRepo.Setup(r => r.UpdateTicketAsync(ticket)).Returns(Task.CompletedTask);

            var result = await _controller.Update(1, ticket);

            Assert.IsType<OkResult>(result);
        }

        // 8. UpdateTicket_ReturnsNotFound_WhenTicketDoesNotExist
        [Fact]
        public async Task UpdateTicket_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            var ticket = SampleTicket(99);
            _mockRepo.Setup(r => r.UpdateTicketAsync(ticket)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Update(99, ticket);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // 9. DeleteTicket_ReturnsOkResult_WhenTicketIsDeletedSuccessfully
        [Fact]
        public async Task DeleteTicket_ReturnsOkResult_WhenTicketIsDeletedSuccessfully()
        {
            _mockRepo.Setup(r => r.DeleteTicketAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);

            Assert.IsType<OkResult>(result);
        }

        // 10. DeleteTicket_ReturnsNotFound_WhenTicketDoesNotExist
        [Fact]
        public async Task DeleteTicket_ReturnsNotFound_WhenTicketDoesNotExist()
        {
            _mockRepo.Setup(r => r.DeleteTicketAsync(99)).ThrowsAsync(new KeyNotFoundException());

            var result = await _controller.Delete(99);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        // 11. GetAllTickets_ReturnsEmptyList_WhenNoTicketsExist
        [Fact]
        public async Task GetAllTickets_ReturnsEmptyList_WhenNoTicketsExist()
        {
            _mockRepo.Setup(r => r.GetAllTicketsAsync()).ReturnsAsync(new List<Ticket>());

            var result = await _controller.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTickets = Assert.IsAssignableFrom<List<Ticket>>(okResult.Value);
            Assert.Empty(returnedTickets);
        }

        // 12. GetTicketsByStatus_ReturnsEmptyList_WhenNoMatchingTicketsExist
        [Fact]
        public async Task GetTicketsByStatus_ReturnsEmptyList_WhenNoMatchingTicketsExist()
        {
            _mockRepo.Setup(r => r.GetTicketsByStatusAsync("Closed")).ReturnsAsync(new List<Ticket>());

            var result = await _controller.GetByStatus("Closed");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTickets = Assert.IsAssignableFrom<List<Ticket>>(okResult.Value);
            Assert.Empty(returnedTickets);
        }
    }
}
