namespace API.DTOs.Requests.Users;

public class CreateUserOrCustomerAdmin
{
    public CreateUserRequest UserModel{ get; set; }
    
    public int? CompanyId { get; set; }

    public bool isCompanyAdmin { get; set; }

}
