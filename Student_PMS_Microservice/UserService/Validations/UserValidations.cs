using FluentValidation;
using UserService.DTOs;

namespace ProjectGroup.Validations;

public class CreateUserValidator : AbstractValidator<UserCreateDTO>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Must be in proper format");

        RuleFor(x => x.Password)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.RoleID)
            .NotNull().WithMessage("Role is required");
    }
}

public class UpdateUserValidator : AbstractValidator<UserUpdateDTO>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.UserID)
            .GreaterThan(0).WithMessage("UserID must be greater than 0");

        RuleFor(x => x.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Must be in proper format");

        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Name is required")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters long")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters");

        RuleFor(x => x.RoleID)
            .NotNull().WithMessage("Role is required");
    }
}

public class CreateRoleValidator : AbstractValidator<RoleCreateDTO>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.RoleName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Role name is required")
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description must not exceed 250 characters");

        RuleFor(x => x.CreatedByID)
            .GreaterThan(0).WithMessage("CreatedByID must be greater than 0");
    }
}

public class UpdateRoleValidator : AbstractValidator<RoleUpdateDTO>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.RoleID)
            .GreaterThan(0).WithMessage("RoleID must be greater than 0");

        RuleFor(x => x.RoleName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Role name is required")
            .MaximumLength(50).WithMessage("Role name must not exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description must not exceed 250 characters");
    }
}

public class CreateUserProfileValidator : AbstractValidator<UserProfileCreateDTO>
{
    public CreateUserProfileValidator()
    {
        RuleFor(x => x.UserID)
            .GreaterThan(0).WithMessage("UserID must be greater than 0");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required")
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");

        RuleFor(x => x.ProfileImageUrl)
            .MaximumLength(500).WithMessage("Profile image URL must not exceed 500 characters");

        RuleFor(x => x.Gender)
            .MaximumLength(30).WithMessage("Gender must not exceed 30 characters");
    }
}

public class UpdateUserProfileValidator : AbstractValidator<UserProfileUpdateDTO>
{
    public UpdateUserProfileValidator()
    {
        RuleFor(x => x.UserProfileID)
            .GreaterThan(0).WithMessage("UserProfileID must be greater than 0");

        RuleFor(x => x.UserID)
            .GreaterThan(0).WithMessage("UserID must be greater than 0");

        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required")
            .MaximumLength(100).WithMessage("Display name must not exceed 100 characters");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters");

        RuleFor(x => x.ProfileImageUrl)
            .MaximumLength(500).WithMessage("Profile image URL must not exceed 500 characters");

        RuleFor(x => x.Gender)
            .MaximumLength(30).WithMessage("Gender must not exceed 30 characters");
    }
}
