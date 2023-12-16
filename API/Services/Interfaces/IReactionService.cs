

namespace API.Services.Interfaces
{
    public interface IReactionService
    {
        Task<string> Like(int solutionId, int userId);
        Task<string> Dislike(int solutionId, int userId);
    }
}
