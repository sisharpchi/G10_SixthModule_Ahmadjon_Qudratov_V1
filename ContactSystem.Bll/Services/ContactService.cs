using ContactSystem.Bll.Dtos;
using ContactSystem.Core.Errors;
using ContactSystem.Dal;
using ContactSystem.Dal.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ContactSystem.Bll.Services;

public class ContactService(MainContext mainContext,IValidator<ContactCreateDto> createDtoValidator, IValidator<ContactDto> updateDtoValidator) : IContactService
{
    public async Task DeleteContactAsync(long contactId, long userId) => await RemoveContactAsync(contactId, userId);
    public async Task<List<ContactDto>> GetAllContactsAsync(long userId)
    {
        var contacts = await SelectAllContactsAsync(userId);
        return contacts.Select(c => Converter(c)).ToList();
    }
    public async Task<long> AddContactAsync(ContactCreateDto contactCreateDto, long userId)
    {
        var res = createDtoValidator.Validate(contactCreateDto);
        if (!res.IsValid)
        {
            string errorMessages = string.Join("; ", res.Errors.Select(e => e.ErrorMessage));
            throw new NotAllowedException(errorMessages);
        }
        var contactEntity = Converter(contactCreateDto);
        contactEntity.UserId = userId;
        contactEntity.CreatedAt = DateTime.UtcNow;
        return await InsertContactAsync(contactEntity);
    }
    public async Task<ContactDto> GetContactByIdAsync(long contactId, long userId)
    {
        var contact = await SelectContactByIdAsync(contactId, userId);
        return Converter(contact);
    }
    public async Task UpdateContactAsync(ContactDto contactDto, long userId)
    {
        var res = updateDtoValidator.Validate(contactDto);
        if (!res.IsValid)
        {
            string errorMessages = string.Join("; ", res.Errors.Select(e => e.ErrorMessage));
            throw new NotAllowedException(errorMessages);
        }
        var contact = await SelectContactByIdAsync(contactDto.Id, userId);
        contact.Email = contactDto.Email;
        contact.FirstName = contactDto.FirstName;
        contact.LastName = contactDto.LastName;
        contact.PhoneNumber = contactDto.PhoneNumber;
        contact.Address = contactDto.Address;
        await UpdateDbContactAsync(contact);
    }
    private Contact Converter(ContactCreateDto contactCreateDto)
    {
        return new Contact
        {
            Address = contactCreateDto.Address,
            Email = contactCreateDto.Email,
            FirstName = contactCreateDto.FirstName,
            LastName = contactCreateDto.LastName,
            PhoneNumber = contactCreateDto.PhoneNumber,
        };
    }
    private ContactDto Converter(Contact contact)
    {
        return new ContactDto
        {
            Address = contact.Address,
            Email = contact.Email,
            FirstName = contact.FirstName,
            Id = contact.Id,
            PhoneNumber = contact.PhoneNumber,
            LastName = contact.LastName,
        };
    }


    private async Task RemoveContactAsync(long contactId, long userId)
    {
        var contact = await SelectContactByIdAsync(contactId, userId);
        mainContext.Contacts.Remove(contact);
        await mainContext.SaveChangesAsync();
    }
    private async Task<List<Contact>> SelectAllContactsAsync(long userId) => await mainContext.Contacts.Where(c => c.UserId == userId).ToListAsync();
    private async Task<Contact> SelectContactByIdAsync(long contactId, long userId) => await mainContext.Contacts.FindAsync(contactId) ?? throw new ForbiddenException("User id not allowed");
    private async Task<long> InsertContactAsync(Contact contact)
    {
        await mainContext.Contacts.AddAsync(contact);
        await mainContext.SaveChangesAsync();
        return contact.Id;
    }
    private async Task UpdateDbContactAsync(Contact contact)
    {
        mainContext.Contacts.Update(contact);
        await mainContext.SaveChangesAsync();
    }
}
