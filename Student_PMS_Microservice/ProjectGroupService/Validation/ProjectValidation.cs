using FluentValidation;
using ProjectGroupService.DTOs;

namespace ProjectGroupService.Validation;

public class InsertValidation : AbstractValidator<ProjectGroupCreateDTO>
{
    public InsertValidation()
    {
        #region Name
        RuleFor(x => x.ProjectGroupName)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(5).WithMessage("Project Name must be at least 5 characters long")
            .MaximumLength(100).WithMessage("Project Name must not exceed 100 characters");
        #endregion
    }
}

public class UpdateValidation : AbstractValidator<ProjectGroupUpdateDTO>
{
    public UpdateValidation()
    {
        #region ProjectGroupID
        RuleFor(x => x.ProjectGroupID)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("ProjectID is required")
            .GreaterThan(0).WithMessage("ProjectID must be greater than 0");
        #endregion

        #region Name
        RuleFor(x => x.ProjectGroupName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Project Name is required")
            .MinimumLength(5).WithMessage("Project Name must be at least 5 characters long")
            .MaximumLength(100).WithMessage("Project Name must not exceed 100 characters");
        #endregion
    }
}