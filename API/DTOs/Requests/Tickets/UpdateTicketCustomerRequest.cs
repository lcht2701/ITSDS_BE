﻿using API.Mappings;
using Domain.Constants.Enums;
using Domain.Models.Tickets;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Requests.Tickets
{
    public class UpdateTicketCustomerRequest : IMapTo<Ticket>
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public Priority? Priority { get; set; }

        public int? ServiceId { get; set; }

        public string? Type { get; set; }

        public string? Street { get; set; }

        public string? Ward { get; set; }

        public string? District { get; set; }

        public string? City { get; set; }

        public string? AttachmentUrl { get; set; }
    }
}
