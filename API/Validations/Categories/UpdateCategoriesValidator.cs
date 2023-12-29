using API.DTOs.Requests.Categories;
using FluentValidation;

namespace API.Validations.Categories
{
    public class UpdateCategoriesValidator : AbstractValidator<UpdateCategoriesRequest>
    {
        public UpdateCategoriesValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(255).WithMessage("Name should not exceed 255 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description should not exceed 500 characters.");
        }
    }
}
