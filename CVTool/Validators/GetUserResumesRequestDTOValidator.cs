using CVTool.Data;
using CVTool.Models.AddResume;
using CVTool.Models.GetUserResumes;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CVTool.Validators
{
    public class GetUserResumesRequestDTOValidator: CustomAbstractValidator<GetResumeByUserRequestDTO>
    {
        public GetUserResumesRequestDTOValidator(DataContext _context)
        {
            RuleFor(r => r.UserId)
                .NotNull().WithMessage("UserId cannot be null.")
                .MustAsync(async (userId, cancellation) =>
                {
                    bool exists = await _context.Users.AnyAsync(u => u.Id == userId);
                    return exists;
                }).WithMessage("User does not exist.");
        }
    }
}
