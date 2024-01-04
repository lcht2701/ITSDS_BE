namespace API.DTOs.Requests.Users;

public class CreateUserOrCustomerAdmin
{
    public CreateUserRequest UserModel{ get; set; }
    
    public int? CompanyId { get; set; }

    public int? DepartmentId { get; set; }

    public bool IsCompanyAdmin { get; set; }

}
