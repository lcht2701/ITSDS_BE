namespace API.DTOs.Requests.Tickets
{
    public class GetTicketStatusesRequest
    {
        public int Id { get; set; }
        public string StatusName { get; set; }
    }
}
