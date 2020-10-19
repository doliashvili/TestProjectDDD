using System.Threading;
using System.Threading.Tasks;

namespace App.Core.Commands
{
    public interface ICommandSender
    {
        /// <summary>
        /// Sends a command to CommandHandler
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken">Cancellation token(optional)</param>
        /// <typeparam name="T">ICommand type</typeparam>
        /// <returns></returns>
        Task SendAsync<T>(T command, CancellationToken cancellationToken = default)
            where T: class, ICommand;
    }
}