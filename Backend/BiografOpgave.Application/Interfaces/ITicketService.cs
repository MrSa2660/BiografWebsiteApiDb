namespace BiografOpgave.Application.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<TicketDTOResponse>> GetForBooking(int bookingId);
    Task<IEnumerable<TicketDTOResponse>> GetForUser(int userId);
    Task<TicketDTOResponse?> GetById(int id);
    Task<TicketDTOResponse?> Create(TicketDTORequest ticket);
    Task<bool> Delete(int id);
}
