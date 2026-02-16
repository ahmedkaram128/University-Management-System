using System.Threading.Tasks;

namespace University_HR_ManagementSystem.Services
{
    public interface ILoginRepository
    {
        Task<bool> validateHRLogin(string username, string password);
        Task<bool> validateEmpLogin(string username, string password);

    }
}
