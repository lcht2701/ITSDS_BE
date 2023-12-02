﻿using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;

namespace API.DTOs.Requests.Tickets
{
    public class CreateTicketCustomerRequest : IMapTo<Ticket>
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public int? ServiceId { get; set; }

        public string? Type { get; set; }

        public string? Location { get; set; }

        public Priority? Priority { get; set; }

        public string? AttachmentUrl { get; set; }
    }
}
