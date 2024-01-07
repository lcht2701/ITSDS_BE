using API.Services.Interfaces;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;

namespace API.Services.Implements
{
    public class ReactionService : IReactionService
    {
        private readonly IRepositoryBase<Reaction> _reactRepo;

        public ReactionService(IRepositoryBase<Reaction> reactRepo)
        {
            _reactRepo = reactRepo;
        }

        public async Task<string> Dislike(int solutionId, int userId)
        {
            string response;
            var reaction = await _reactRepo.FirstOrDefaultAsync(x => x.SolutionId == solutionId && x.UserId == userId);
            if (reaction == null)
            {
                Reaction newReaction = new Reaction()
                {
                    SolutionId = solutionId,
                    UserId = userId,
                    ReactionType = 1,
                };
                await _reactRepo.CreateAsync(newReaction);
                response = "Your reaction has been successfully recorded!";
            }
            else
                switch (reaction.ReactionType)
                {
                    case 0:
                        {
                            reaction.ReactionType = 1;
                            await _reactRepo.UpdateAsync(reaction);
                            response = "Your reaction has been updated successfully!";
                            break;
                        }
                    case 1:
                        {
                            await _reactRepo.DeleteAsync(reaction);
                            response = "Your reaction has been removed.";
                            break;
                        }
                    default:
                        {
                            response = "No new updated!";
                            break;
                        }
                }
            return response;
        }

        public async Task<string> Like(int solutionId, int userId)
        {
            string response;
            var reaction = await _reactRepo.FirstOrDefaultAsync(x => x.SolutionId == solutionId && x.UserId == userId);
            if (reaction == null)
            {
                Reaction newReaction = new Reaction()
                {
                    SolutionId = solutionId,
                    UserId = userId,
                    ReactionType = 0,
                };
                await _reactRepo.CreateAsync(newReaction);
                response = "Your reaction has been successfully recorded!";
            }
            else
                switch (reaction.ReactionType)
                {
                    case 0:
                        {
                            await _reactRepo.DeleteAsync(reaction);
                            response = "Your reaction has been removed.";
                            break;
                        }
                    case 1:
                        {
                            reaction.ReactionType = 0;
                            await _reactRepo.UpdateAsync(reaction);
                            response = "Your reaction has been updated successfully!";
                            break;
                        }
                    default:
                        {
                            response = "No new updated!";
                            break;
                        }
                }
            return response;
        }
    }

}
