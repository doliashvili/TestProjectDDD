using System.Threading;
using System.Threading.Tasks;

namespace App.Core.Commands
{
    /// <summary>
    /// Async handler for commands 
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface ICommandHandler<in TCommand> 
        where TCommand: ICommand
    {
        /// <summary>
        /// Handle command
        /// </summary>
        /// <param name="command">ICommand type</param>
        /// <param name="cancellationToken">CancellationToken(optional)</param>
        /// <returns>Task</returns>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default);
    }
}