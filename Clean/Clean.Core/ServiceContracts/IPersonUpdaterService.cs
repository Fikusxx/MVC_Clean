using Clean.Core.DTO.PersonDTO;

namespace Clean.Core.ServiceContracts;


public interface IPersonUpdaterService
{
    public Task<PersonResponse> UpdatePersonAsync(PersonUpdateRequest? request);
}
