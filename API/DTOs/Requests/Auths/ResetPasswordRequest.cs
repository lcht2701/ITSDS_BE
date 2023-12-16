namespace API.DTOs.Requests.Auths
{
    public class ResetPasswordRequest
    {
        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
