using FluentValidation;
using UserService.DTOs;

namespace ProjectGroup.Validations;

public class CreateUserValidator : AbstractValidator<UserCreateDTO>
{
    public CreateUserValidator()
    {
        #region Name
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithErrorCode("Name must be at least 3 characters long")
            .MaximumLength(50).WithErrorCode("Name must not exceed 50 characters");
        #endregion

        #region Email
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Must be in proper Formate");
        #endregion

        #region Password
        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");
        #endregion

        #region Phone
        RuleFor(x => x.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Phone is required")
            .Matches("^[0-9]{10}$").WithMessage("Phone must be a valid 10-digit number");
        #endregion

        #region RoleID
        RuleFor(x => x.RoleID)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Role is required");
        #endregion
    }
}

public class UpdateUserValidator : AbstractValidator<UserUpdateDTO>
{
    public UpdateUserValidator()
    {
        #region UserID
        RuleFor(x => x.UserID)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("UserID is required")
            .GreaterThan(0).WithMessage("UserID must be greater than 0");
        #endregion

        #region Email
        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Must be in proper Formate");
        #endregion

        #region Name
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithErrorCode("Name must be at least 3 characters long")
            .MaximumLength(50).WithErrorCode("Name must not exceed 50 characters");
        #endregion

        #region Phone
        RuleFor(x => x.Phone)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Phone is required")
            .Matches("^[0-9]{10}$").WithMessage("Phone must be a valid 10-digit number");
        #endregion

        #region RoleID
        RuleFor(x => x.RoleID)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("Role is required");
        #endregion
    }
}