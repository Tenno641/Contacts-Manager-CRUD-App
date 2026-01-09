namespace ContactsManager.Core.ServiceContracts.Persons;
public interface IPersonsDeleteService
{
    Task<bool> DeleteAsync(Guid? id);
}
