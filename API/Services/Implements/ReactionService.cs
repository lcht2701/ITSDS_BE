
using API.Services.Interfaces;
using Domain.Models.Tickets;
using Persistence.Repositories.Interfaces;
using System.Diagnostics;

namespace API.Services.Implements
{
    public class ReactionService : IReactionService
    {
        private readonly IRepositoryBase<Reaction> _reactRepo;

        public ReactionService(IRepositoryBase<Reaction> reactRepo)
        {
            _reactRepo = reactRepo;
        }

        public async Task Dislike(int solutionId, int userId)
        {
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
                return;
            }
            else
                switch (reaction.ReactionType)
                {
                    case 0:
                        {
                            reaction.ReactionType = 1;
                            await _reactRepo.UpdateAsync(reaction);
                            break;
                        }
                    case 1:
                        {
                            await _reactRepo.DeleteAsync(reaction);
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }
        }

        public async Task Like(int solutionId, int userId)
        {
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
                return;
            }
            else
                switch (reaction.ReactionType)
                {
                    case 0:
                        {
                            await _reactRepo.DeleteAsync(reaction);
                            break;
                        }
                    case 1:
                        {
                            reaction.ReactionType = 0;
                            await _reactRepo.UpdateAsync(reaction);
                            break;
                        }
                    default:
                        {
                            return;
                        }
                }
        }
    }
}
