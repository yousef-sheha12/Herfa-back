using FluentValidation;
using Herfa_back.DTOs.Auth;

namespace Herfa_back.DTOs.Auth.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("اسم المستخدم مطلوب")
                .MaximumLength(50).WithMessage("اسم المستخدم يجب ألا يتجاوز 50 حرفاً");

            RuleFor(x => x.Email).NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
                .EmailAddress().WithMessage("صيغة البريد الإلكتروني غير صحيحة");

            RuleFor(x => x.Password).NotEmpty().WithMessage("كلمة المرور مطلوبة")
                .MinimumLength(6).WithMessage("كلمة المرور يجب أن تكون 6 أحرف على الأقل");

            RuleFor(x => x.role).IsInEnum().WithMessage("الدور المحدد غير صالح");
        }
    }
}
