using System.Threading.Tasks;

namespace Af.Core.Common.Hubs
{
    public interface IChatClient
    {
        Task ReceiveMessage(object message);

        Task ReceiveMessage(string user, string message);

        Task ReceiveUpdate(object message);
    }
}
