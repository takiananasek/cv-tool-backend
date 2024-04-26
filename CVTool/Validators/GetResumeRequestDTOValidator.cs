using CVTool.Data;
using CVTool.Models.GetResume;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CVTool.Validators
{
    public class GetResumeRequestDTOValidator: CustomAbstractValidator<GetResumeRequestDTO>
    {
        public GetResumeRequestDTOValidator(DataContext _context)
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
