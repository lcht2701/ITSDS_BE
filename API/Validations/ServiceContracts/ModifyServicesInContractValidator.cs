using API.DTOs.Requests.ServiceContracts;
using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace API.Validations.ServiceContracts
{
    public class ModifyServicesInContractValidator : AbstractValidator<ModifyServicesInContract>
    {
        public ModifyServicesInContractValidator()
        {
            RuleFor(x => x.ContractId)
                .NotEmpty().WithMessage("ContractId is required.");
        }
    }
}
