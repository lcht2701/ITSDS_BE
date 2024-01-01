using API.DTOs.Requests.Categories;
using FluentValidation;

namespace API.Validations.Categories
{
    public class CreateCategoriesValidator : AbstractValidator<CreateCategoriesRequest>
    {
        public CreateCategoriesValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");
        }
    }
}
