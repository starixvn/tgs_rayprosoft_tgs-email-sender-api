using FluentValidation;

namespace TGS.Core.Infrastructures.Mail
{
    public class SendEmailRequestValidator :　AbstractValidator<SendEmailRequest>
    {
        public SendEmailRequestValidator()
        {
            RuleFor(x => x.Receivers).NotEmpty()
                .WithMessage("{PropertyName} cannot be empty");

            RuleFor(x => x.Subject).NotNull()
                .WithMessage("{PropertyName} cannot be null");

            RuleFor(x => x.Content).NotNull()
               .WithMessage("{PropertyName} cannot be null");
        }
    }
}
