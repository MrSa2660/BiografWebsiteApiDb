namespace BiografOpgave.Application.Interfaces;

public interface IBookingService
{
    Task<IEnumerable<BookingDTOResponse>> GetAll();
    Task<BookingDTOResponse?> GetById(int id);
    Task<IEnumerable<BookingDTOResponse>> GetForUser(int userId);
    Task<IEnumerable<BookingSeatDTO>> GetSeatsForShowtime(int showtimeId);
    Task<BookingDTOResponse?> Create(BookingDTORequest booking);
    Task<BookingDTOResponse?> UpdateStatus(int id, BookingStatus status);
    Task<bool> Delete(int id);
}
