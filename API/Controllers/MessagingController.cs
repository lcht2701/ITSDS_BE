using API.DTOs.Requests.Notifications;
using API.Services.Interfaces;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("/v1/itsds/notification")]
public class MessagingController : BaseController
{
    private readonly IMessagingService _messagingService;

    public MessagingController(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        try
        {
            var result = await _messagingService.Get(CurrentUserID);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest model)
    {
        try
        {
            await _messagingService.SendNotification(model, CurrentUserID);
            return Ok("Notification sent successfully");
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch]
    public async Task<IActionResult> MarkAsRead()
    {
        try
        {
            await _messagingService.MarkAsRead(CurrentUserID);
            return Ok("Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
