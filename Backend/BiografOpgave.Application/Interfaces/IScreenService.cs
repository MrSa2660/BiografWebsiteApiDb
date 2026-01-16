namespace BiografOpgave.Application.Interfaces;

public interface IScreenService
{
    Task<IEnumerable<ScreenDTOResponse>> GetAll();
    Task<ScreenDTOResponse?> GetById(int id);
    Task<ScreenDTOResponse?> Create(ScreenDTORequest screen);
    Task<ScreenDTOResponse?> Update(ScreenDTORequest screen);
    Task<bool> Delete(int id);
}
