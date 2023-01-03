

namespace Clean.Core.ServiceContracts;


public interface IPersonDeleterService
{
    public Task<bool> DeletePersonAsync(Guid? personId);
}
