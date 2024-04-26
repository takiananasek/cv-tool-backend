using CVTool.Data;
using CVTool.Data.Model;
using CVTool.Models.AddResume;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CVTool.Validators
{
    public class AddResumeRequestDTOValidator: CustomAbstractValidator<AddResumeRequestDTO>
    {
        public AddResumeRequestDTOValidator(DataContext _context)
        {
            RuleFor(r => r.OwnerId)
                .NotNull()
                .NotEmpty()
                .MustAsync(async (ownerId, cancellation) =>
                {
                    bool exists = await _context.Users.AnyAsync(u => u.Id == ownerId);
                    return exists;
                }).WithMessage("User does not exist.");

            RuleFor(r => r.Title)
                .NotNull()
                .NotEmpty().WithMessage("Title cannot be null or empty");

            RuleFor(r => r.Components)
                .NotNull().WithMessage("Components cannot be null");
        }
    }
}
