using Clean.Core.DTO.PersonDTO;


namespace Clean.Core.ServiceContracts;


public interface IPersonAdderService
{
    public Task<PersonResponse> AddPersonAsync(PersonAddRequest? request);
}
