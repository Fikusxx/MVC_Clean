using Clean.Core.Domain.RepositoryContracts;
using Clean.Core.DTO.PersonDTO;
using Clean.Core.Helpers;
using Clean.Core.ServiceContracts;
using Serilog;

namespace Clean.Core.Services;


public class PersonAdderService : IPersonAdderService
{
    private readonly IPersonRepository personRepository;
    private readonly IDiagnosticContext diagnosticContext;

    public PersonAdderService(IPersonRepository personRepository, IDiagnosticContext diagnosticContext)
    {
        this.personRepository = personRepository;
        this.diagnosticContext = diagnosticContext;
    }

    public async Task<PersonResponse> AddPersonAsync(PersonAddRequest? request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        ValidationHelper.ModelValidation(request);

        var person = request.ToPerson();
        person.PersonId = Guid.NewGuid();
        await personRepository.AddAsync(person);

        var personResponse = person.ToPersonResponse();

        return personResponse;
    }
}
