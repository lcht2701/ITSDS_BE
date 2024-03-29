﻿using API.DTOs.Requests.Messagings;
using API.DTOs.Responses.Dashboards.Accountants;
using API.Services.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

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
    [SwaggerResponse(200, "Get Notification", typeof(List<Domain.Models.Messaging>))]
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
    [HttpPost("send-notification")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest model)
    {
        try
        {
            await _messagingService.SendNotification("ITSDS", model.Message, CurrentUserID);
            return Ok("Send Notification Successfully");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPost("send-chat-notification")]
    public async Task<IActionResult> SendChatNotification([FromBody] SendChatNotificationRequest model)
    {
        try
        {
            await _messagingService.SendChatNotification(model);
            return Ok("Send Chat Notification Successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        try
        {
            var result = await _messagingService.MarkAsRead(id);
            return Ok(new
            {
                Message = "Mark As Read Successfully",
                Data = result
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize]
    [HttpPatch("read-all")]
    public async Task<IActionResult> ReadAll()
    {
        try
        {
            await _messagingService.MarkAsReadAll(CurrentUserID);
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
        catch (Exception ex)
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
