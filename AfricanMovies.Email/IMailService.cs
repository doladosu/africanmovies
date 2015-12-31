using System.Collections.Generic;
using System.Threading.Tasks;

namespace AfricanMovies.Email
{
    public interface IMailService
    {
        Task<bool> SendMail(string fromEmail, List<string> toEmails, string subject, string body);
    }
}