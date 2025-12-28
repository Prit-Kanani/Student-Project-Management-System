using FluentValidation;
using ProjectService.DTOs;

namespace ProjectService.Validation;

public class InsertValidation : AbstractValidator<ProjectCreateDTO>
{
    public InsertValidation()
    {
        #region Name
        RuleFor(x => x.ProjectName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(5).WithMessage("Project Name must be at least 5 characters long")
            .MaximumLength(100).WithMessage("Project Name must not exceed 100 characters");
        #endregion

        #region Description
        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
        #endregion
    }
}

public class UpdateValidation : AbstractValidator<ProjectUpdateDTO>
{
    public UpdateValidation()
    {
        #region ProjectID
        RuleFor(x => x.ProjectID)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ProjectID is required")
            .GreaterThan(0).WithMessage("ProjectID must be greater than 0");
        #endregion

        #region Name
        RuleFor(x => x.ProjectName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(5).WithMessage("Project Name must be at least 5 characters long")
            .MaximumLength(100).WithMessage("Project Name must not exceed 100 characters");
        #endregion

        #region Description
        RuleFor(x => x.Description)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
        #endregion
    }
}