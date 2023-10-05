namespace Domain.Constants;
public static class Roles
{
    public const string ADMIN = nameof(Role.Admin);
    public const string CUSTOMER = nameof(Role.Customer);
    public const string MANAGER = nameof(Role.Manager);
    public const string TECHNICIAN = nameof(Role.Technician);
    public const string ACCOUNTANT = nameof(Role.Accountant);
    public const string CUSTOMERADMIN = nameof(Role.CustomerAdmin);

    //Combo for ticket flow
    public const string COMPANYMEMBERS = nameof(Role.Customer) + "," + nameof(Role.CustomerAdmin);
    public const string TICKETSUPPORTMEMBERS = nameof(Role.Manager) + "," + nameof(Role.Technician);
    public const string TICKETPARTICIPANTS = nameof(Role.Customer) + "," + nameof(Role.CustomerAdmin) + "," + nameof(Role.Manager) + "," + nameof(Role.Technician);
}

