﻿using API.DTOs.Requests.Feedbacks;
using API.DTOs.Responses.Feedbacks;

namespace API.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<List<GetFeedbackResponse>> Get(int solutionId, int userId);
        Task<object> GetById(int id);
        Task<object> Create(CreateFeedbackRequest model, int userId);
        Task<object> CreateReply(CreateReplyRequest model, int userId);
        Task<object> Update(int id, UpdateFeedbackRequest model, int userId);
        Task Delete(int id);

    }
}
