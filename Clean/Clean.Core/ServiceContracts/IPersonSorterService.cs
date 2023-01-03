using Clean.Core.DTO.PersonDTO;
using Clean.Core.Enums;

namespace Clean.Core.ServiceContracts;


public interface IPersonSorterService
{
    public Task<List<PersonResponse>> GetSortedPersonsAsync(List<PersonResponse> persons, string sortBy, SortOrderOptions options);
}
