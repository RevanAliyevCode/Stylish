using System;
using IdentityApp.Utilities.Email.Concrets;

namespace IdentityApp.Utilities.Email.Abstracts;

public interface IEmailSender
{
    void SendEmail(Message message);
}
