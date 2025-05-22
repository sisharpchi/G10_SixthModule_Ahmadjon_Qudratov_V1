using ContactSystem.Bll.Dtos;

namespace ContactSystem.Bll.Services;

public interface IContactService
{
    Task<List<ContactDto>> GetAllContactsAsync(long userId);
    Task<ContactDto> GetContactByIdAsync(long contactId, long userId);
    Task DeleteContactAsync(long contactId, long userId);
    Task<long> AddContactAsync(ContactCreateDto contactCreateDto, long userId);
    Task UpdateContactAsync(ContactDto contactDto, long userId);
}