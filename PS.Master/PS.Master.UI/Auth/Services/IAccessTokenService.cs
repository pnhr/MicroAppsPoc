using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PS.Master.UI.Auth.Services
{
    public interface IAccessTokenService
    {
        Task<string> GetAccessTokenAsync(string tokenName);
        Task SetAccessTokenAsync(string tokenName, string tokenValue);
        Task RemoveAccessTokenAsync(string tokenName);
    }
}
