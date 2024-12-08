using Identity.Utilities.EmailHandler.Models;

namespace Identity.Utilities.EmailHandler.Abstract
{
    public interface IEmailService
    {
        void SendMessage(Message message);
    }
}
