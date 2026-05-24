using LobasOrdersApi.Models;

namespace LobasOrdersApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();

        User? GetById(int id);

        User? GetByEmail(string email);

        User? GetByProvider(string authProvider, string providerUserId);

        int Create(User user);

        bool Update(int id, User user);

        bool UpdateName(int id, string name, DateTime changedAt);

        bool Delete(int id);

        bool UpdateBiometricStatus(int id, bool biometricEnabled);
    }
}
