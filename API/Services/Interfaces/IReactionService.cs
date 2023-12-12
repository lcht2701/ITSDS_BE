

namespace API.Services.Interfaces
{
    public interface IReactionService
    {
        Task Like(int solutionId, int userId);
        Task Dislike(int solutionId, int userId);
    }
}
