using Clean.Core.Domain.RepositoryContracts;
using Clean.Core.DTO.PersonDTO;
using Clean.Core.Enums;
using Clean.Core.ServiceContracts;
using Serilog;

namespace Clean.Core.Services;


public class PersonSorterService : IPersonSorterService
{
    private readonly IPersonRepository personRepository;
    private readonly IDiagnosticContext diagnosticContext;

    public PersonSorterService(IPersonRepository personRepository, IDiagnosticContext diagnosticContext)
    {
        this.personRepository = personRepository;
        this.diagnosticContext = diagnosticContext;
    }

    public async Task<List<PersonResponse>> GetSortedPersonsAsync(List<PersonResponse> persons, string sortBy, SortOrderOptions options)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return persons;

        persons = (sortBy, options) switch
        {
            (nameof(PersonResponse.Name), SortOrderOptions.ASC) => persons.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Name), SortOrderOptions.DESC) => persons.OrderByDescending(x => x.Name, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Email), SortOrderOptions.ASC) => persons.OrderBy(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Email), SortOrderOptions.DESC) => persons.OrderByDescending(x => x.Email, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Gender), SortOrderOptions.ASC) => persons.OrderBy(x => x.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Gender), SortOrderOptions.DESC) => persons.OrderByDescending(x => x.Gender, StringComparer.OrdinalIgnoreCase).ToList(),
            (nameof(PersonResponse.Age), SortOrderOptions.ASC) => persons.OrderBy(x => x.Age).ToList(),
            (nameof(PersonResponse.Age), SortOrderOptions.DESC) => persons.OrderByDescending(x => x.Age).ToList(),
            (_, _) => persons,
        };

        return persons;
    }
}
