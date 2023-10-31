using API.DTOs.Requests.Messagings;
using API.Services.Interfaces;
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
            var result = await _messagingService.GetNotification(CurrentUserID);
            return Ok(result);
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> GetToken([FromBody] GetTokenRequest model)
    {
        try
        {
            await _messagingService.GetToken(CurrentUserID, model.Token);
            return Ok("Get token successfully");
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpDelete]
    public async Task<IActionResult> RemoveToken([FromBody] GetTokenRequest model)
    {
        try
        {
            await _messagingService.RemoveToken(CurrentUserID, model.Token);
            return Ok("Remove token successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
