using CVTool.Data;
using CVTool.Models.EditResume;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CVTool.Validators
{
    public class EditResumeRequestDTOValidator: CustomAbstractValidator<EditResumeRequestDTO>
    {
        public EditResumeRequestDTOValidator(DataContext context)
        {
            RuleFor(r => r.Id)
               .NotNull()
               .NotEmpty()
               .MustAsync(async (ownerId, cancellation) =>
               {
                   bool exists = await context.Resumes.AnyAsync(u => u.Id == ownerId);
                   return exists;
               }).WithMessage("Resume does not exist.");

            RuleFor(r => r.Title)
                .NotNull()
                .NotEmpty().WithMessage("Title cannot be null or empty");

            RuleFor(r => r.Components)
                .NotNull().WithMessage("Components cannot be null");
        }
    }
}
