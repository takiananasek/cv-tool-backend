using CVTool.Data;
using CVTool.Models.Files;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CVTool.Validators
{
    public class GetFileByKeyRequestDTOValidator: CustomAbstractValidator<GetFileByKeyRequestDTO>
    {
        public GetFileByKeyRequestDTOValidator(DataContext context)
        {
            RuleFor(r => r.Key)
                .MustAsync(async (k, cancellation) =>
                {
                    return await context.ImageMetaDatas.AnyAsync(i => i.FileName.Equals(k));
                }).WithMessage("Image with a given identifier does not exist or has been deleted.");
        }
    }
}
