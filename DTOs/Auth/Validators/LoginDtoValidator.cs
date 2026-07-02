using FluentValidation;
using Herfa_back.DTOs.Auth;

namespace Herfa_back.DTOs.Auth.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public LoginDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة");

            RuleFor(x => x.Password).NotEmpty().WithMessage("كلمة المرور مطلوبة");
        }
    }
}
