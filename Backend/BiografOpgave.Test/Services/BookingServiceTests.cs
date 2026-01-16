using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BiografOpgave.Application.DTOs;
using BiografOpgave.Application.Services;
using BiografOpgave.Domain.Interfaces;
using BiografOpgave.Domain.Models;
using Moq;
using Xunit;

namespace BiografOpgave.Test.Services;

public class BookingServiceTests
{
    [Fact]
    public async Task Create_ReturnsNull_WhenUserIdInvalid()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var ticketRepo = new Mock<ITicketRepository>();
        var service = new BookingService(bookingRepo.Object, ticketRepo.Object);

        var request = new BookingDTORequest
        {
            UserId = 0,
            ShowtimeId = 10,
            Seats = new[]
            {
                new BookingSeatDTO { Row = 1, Number = 1, Price = 100m }
            }
        };

        var result = await service.Create(request);

        Assert.Null(result);
        bookingRepo.Verify(repo => repo.GetSeatsForShowtime(It.IsAny<int>()), Times.Never);
        bookingRepo.Verify(repo => repo.Create(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task Create_ReturnsNull_WhenNoSeats()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var ticketRepo = new Mock<ITicketRepository>();
        var service = new BookingService(bookingRepo.Object, ticketRepo.Object);

        var request = new BookingDTORequest
        {
            UserId = 12,
            ShowtimeId = 10,
            Seats = Array.Empty<BookingSeatDTO>()
        };

        var result = await service.Create(request);

        Assert.Null(result);
        bookingRepo.Verify(repo => repo.GetSeatsForShowtime(It.IsAny<int>()), Times.Never);
        bookingRepo.Verify(repo => repo.Create(It.IsAny<Booking>()), Times.Never);
    }

    [Fact]
    public async Task Create_ReturnsNull_WhenSeatAlreadyBooked()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var ticketRepo = new Mock<ITicketRepository>();
        var service = new BookingService(bookingRepo.Object, ticketRepo.Object);

        bookingRepo.Setup(repo => repo.GetSeatsForShowtime(5))
            .ReturnsAsync(new[]
            {
                new BookingSeat { Row = 3, Number = 7, Price = 90m }
            });

        var request = new BookingDTORequest
        {
            UserId = 6,
            ShowtimeId = 5,
            Seats = new[]
            {
                new BookingSeatDTO { Row = 3, Number = 7, Price = 90m }
            }
        };

        var result = await service.Create(request);

        Assert.Null(result);
        bookingRepo.Verify(repo => repo.Create(It.IsAny<Booking>()), Times.Never);
        ticketRepo.Verify(repo => repo.Create(It.IsAny<Ticket>()), Times.Never);
    }

    [Fact]
    public async Task Create_CreatesTicketsAndTotals_WhenValid()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var ticketRepo = new Mock<ITicketRepository>();
        var service = new BookingService(bookingRepo.Object, ticketRepo.Object);

        bookingRepo.Setup(repo => repo.GetSeatsForShowtime(20))
            .ReturnsAsync(Array.Empty<BookingSeat>());

        Booking? capturedBooking = null;
        bookingRepo.Setup(repo => repo.Create(It.IsAny<Booking>()))
            .ReturnsAsync((Booking booking) =>
            {
                booking.Id = 42;
                capturedBooking = booking;
                return booking;
            });

        var createdTickets = new List<Ticket>();
        ticketRepo.Setup(repo => repo.Create(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket ticket) =>
            {
                createdTickets.Add(ticket);
                return ticket;
            });

        var detailed = new Booking
        {
            Id = 42,
            UserId = 7,
            ShowtimeId = 20,
            TotalPrice = 220m,
            Status = BookingStatus.Confirmed,
            Seats = new List<BookingSeat>
            {
                new BookingSeat { Row = 1, Number = 2, Price = 100m },
                new BookingSeat { Row = 1, Number = 3, Price = 120m }
            },
            Tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    BookingId = 42,
                    Code = "ABCDEF12",
                    SeatLabel = "Row 1 Seat 2",
                    IssuedAt = DateTime.UtcNow
                },
                new Ticket
                {
                    Id = 2,
                    BookingId = 42,
                    Code = "ABCDEF34",
                    SeatLabel = "Row 1 Seat 3",
                    IssuedAt = DateTime.UtcNow
                }
            }
        };

        bookingRepo.Setup(repo => repo.GetDetailed(42)).ReturnsAsync(detailed);

        var request = new BookingDTORequest
        {
            UserId = 7,
            ShowtimeId = 20,
            Seats = new[]
            {
                new BookingSeatDTO { Row = 1, Number = 2, Price = 100m },
                new BookingSeatDTO { Row = 1, Number = 3, Price = 120m }
            }
        };

        var result = await service.Create(request);

        Assert.NotNull(result);
        Assert.Equal(220m, result.TotalPrice);
        Assert.Equal(BookingStatus.Confirmed, result.Status);
        Assert.Equal(2, result.Seats.Count());
        Assert.Equal(2, result.Tickets.Count());

        Assert.NotNull(capturedBooking);
        Assert.Equal(220m, capturedBooking.TotalPrice);
        Assert.Equal(BookingStatus.Confirmed, capturedBooking.Status);
        Assert.Equal(2, capturedBooking.Seats.Count);

        Assert.Equal(2, createdTickets.Count);
        Assert.All(createdTickets, ticket =>
        {
            Assert.Equal(42, ticket.BookingId);
            Assert.Equal(8, ticket.Code.Length);
            Assert.Equal(ticket.Code, ticket.Code.ToUpperInvariant());
        });
        Assert.Contains(createdTickets, ticket => ticket.SeatLabel == "Row 1 Seat 2");
        Assert.Contains(createdTickets, ticket => ticket.SeatLabel == "Row 1 Seat 3");
    }

    [Fact]
    public async Task UpdateStatus_ReturnsNull_WhenBookingMissing()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var ticketRepo = new Mock<ITicketRepository>();
        var service = new BookingService(bookingRepo.Object, ticketRepo.Object);

        bookingRepo.Setup(repo => repo.GetDetailed(88)).ReturnsAsync((Booking?)null);

        var result = await service.UpdateStatus(88, BookingStatus.Cancelled);

        Assert.Null(result);
        bookingRepo.Verify(repo => repo.Update(It.IsAny<Booking>()), Times.Never);
        ticketRepo.Verify(repo => repo.Create(It.IsAny<Ticket>()), Times.Never);
    }

    [Fact]
    public async Task UpdateStatus_IssuesTickets_WhenConfirmedWithSeats()
    {
        var bookingRepo = new Mock<IBookingRepository>();
        var ticketRepo = new Mock<ITicketRepository>();
        var service = new BookingService(bookingRepo.Object, ticketRepo.Object);

        var booking = new Booking
        {
            Id = 10,
            UserId = 4,
            ShowtimeId = 5,
            Seats = new List<BookingSeat>
            {
                new BookingSeat { Row = 2, Number = 4, Price = 80m }
            },
            Tickets = new List<Ticket>()
        };

        var detailed = new Booking
        {
            Id = 10,
            UserId = 4,
            ShowtimeId = 5,
            Status = BookingStatus.Confirmed,
            Seats = booking.Seats,
            Tickets = new List<Ticket>
            {
                new Ticket
                {
                    Id = 1,
                    BookingId = 10,
                    Code = "ABCDEF99",
                    SeatLabel = "Row 2 Seat 4",
                    IssuedAt = DateTime.UtcNow
                }
            }
        };

        bookingRepo.SetupSequence(repo => repo.GetDetailed(10))
            .ReturnsAsync(booking)
            .ReturnsAsync(detailed);
        bookingRepo.Setup(repo => repo.Update(It.IsAny<Booking>()))
            .ReturnsAsync((Booking updated) => updated);
        ticketRepo.Setup(repo => repo.Create(It.IsAny<Ticket>()))
            .ReturnsAsync((Ticket ticket) => ticket);

        var result = await service.UpdateStatus(10, BookingStatus.Confirmed);

        Assert.NotNull(result);
        Assert.Equal(BookingStatus.Confirmed, result.Status);
        Assert.Single(result.Tickets);
        ticketRepo.Verify(repo => repo.Create(It.IsAny<Ticket>()), Times.Once);
    }
}
