using Clean.Core.Domain.RepositoryContracts;
using Clean.Core.DTO.PersonDTO;
using Clean.Core.Helpers;
using Clean.Core.ServiceContracts;
using Serilog;

namespace Clean.Core.Services;


public class PersonUpdaterService : IPersonUpdaterService
{
    private readonly IPersonRepository personRepository;
    private readonly IDiagnosticContext diagnosticContext;

    public PersonUpdaterService(IPersonRepository personRepository, IDiagnosticContext diagnosticContext)
    {
        this.personRepository = personRepository;
        this.diagnosticContext = diagnosticContext;
    }

    public async Task<PersonResponse> UpdatePersonAsync(PersonUpdateRequest? request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidationHelper.ModelValidation(request);

        var person = await personRepository.GetByIdAsync(request.PersonId);

        if (person == null)
            throw new ArgumentException(nameof(request.PersonId));

        var personChanges = request.ToPerson();
        personChanges = await personRepository.UpdateAsync(personChanges);

        return personChanges.ToPersonResponse();
    }
}
