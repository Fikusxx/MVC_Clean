using Clean.Core.Domain.RepositoryContracts;
using Clean.Core.ServiceContracts;
using Serilog;

namespace Clean.Core.Services;


public class PersonDeleterService : IPersonDeleterService
{
    private readonly IPersonRepository personRepository;
    private readonly IDiagnosticContext diagnosticContext;

    public PersonDeleterService(IPersonRepository personRepository, IDiagnosticContext diagnosticContext)
    {
        this.personRepository = personRepository;
        this.diagnosticContext = diagnosticContext;
    }

    public async Task<bool> DeletePersonAsync(Guid? personId)
    {
        if (personId == null)
            throw new ArgumentNullException(nameof(personId));

        var person = await personRepository.GetByIdAsync(personId.GetValueOrDefault());

        if (person == null)
            return false;

        await personRepository.DeleteAsync(person.PersonId);

        return true;
    }
}
