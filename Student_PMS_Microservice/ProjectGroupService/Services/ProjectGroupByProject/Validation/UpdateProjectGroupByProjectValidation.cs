using FluentValidation;
using ProjectGroupService.DTOs;

namespace ProjectGroupService.Services.ProjectGroupByProject.Validation
{
    public class UpdateGroupWiseStudentValidation : AbstractValidator<ProjectGroupByProjectUpdateDTO>
    {
        public UpdateGroupWiseStudentValidation() 
        {
            RuleFor(x => x.ProjectGroupID)
                .NotEmpty().WithMessage("Project Group is required.");
            RuleFor(x => x.ProjectID)
                .GreaterThan(0).WithMessage("Project ID must be greater than zero.");
        }
    }
}
