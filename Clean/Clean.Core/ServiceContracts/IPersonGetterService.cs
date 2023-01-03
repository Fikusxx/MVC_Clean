using Clean.Core.DTO.PersonDTO;

namespace Clean.Core.ServiceContracts;


public interface IPersonGetterService
{
    public Task<List<PersonResponse>> GetAllPersonsAsync();
    public Task<PersonResponse?> GetPersonByIdAsync(Guid? personId);
    public Task<List<PersonResponse>> GetFilteredPersonsAsync(string searchBy, string? searchString);
    public Task<MemoryStream> GetPersonsCSV();
    public Task<MemoryStream> GetPersonsExcel();
}
