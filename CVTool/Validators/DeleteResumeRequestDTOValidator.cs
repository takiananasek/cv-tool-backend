using CVTool.Data;
using CVTool.Models.AddResume;
using CVTool.Models.DeleteResume;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CVTool.Validators
{
    public class DeleteResumeRequestDTOValidator: CustomAbstractValidator<DeleteResumeRequestDTO>
    {
        public DeleteResumeRequestDTOValidator(DataContext _context)
        {
            RuleFor(r => r.Id)
                .NotNull().WithMessage("Resume Id cannot be null.")
                .MustAsync(async (id, cancellation) =>
                {
                    bool exists = await _context.Resumes.AnyAsync(u => u.Id == id);
                    return exists;
                }).WithMessage("Resume does not exist or has already been deleted.");
        }
    }
}
