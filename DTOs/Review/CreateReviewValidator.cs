using FluentValidation;

namespace Herfa_back.DTOs.Review
{
    public class CreateReviewValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewValidator()
        {
            RuleFor(r => r.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("Rating must be between 1 and 5.");

            RuleFor(r => r.Comment)
                .MaximumLength(1000)
                .WithMessage("Comment cannot exceed 1000 characters.")
                .When(r => !string.IsNullOrEmpty(r.Comment));
        }
    }
}
