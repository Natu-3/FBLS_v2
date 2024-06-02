using System;
using System.Threading.Tasks;

public interface IMessageSource
{
    event Func<string, Task> OnMessageRecieved;
    void Send(string message);
}
