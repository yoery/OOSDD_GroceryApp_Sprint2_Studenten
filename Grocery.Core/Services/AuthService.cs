using Grocery.Core.Helpers;
using Grocery.Core.Interfaces.Services;
using Grocery.Core.Models;

namespace Grocery.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IClientService _clientService;
        public AuthService(IClientService clientService)
        {
            _clientService = clientService;
        }

        public Client? Login(string email, string password)
        {
            var client = _clientService.Get(email);
            if (client == null) return null;

            // Simpel wachtwoord ophalen
            var wachtwoord = typeof(Client)
                .GetProperty("_password", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(client) as string;

            // Simpel vergelijken
            if (wachtwoord != null && PasswordHelper.VerifyPassword(password, wachtwoord))
                return client;

            return null;
        }
    }
}
