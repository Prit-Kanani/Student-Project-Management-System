import { useState, type FormEvent } from "react";
import Checkbox from "@/components/form/input/Checkbox";
import Input from "@/components/form/input/InputField";
import Label from "@/components/form/Label";
import Button from "@/components/ui/button/Button";
import {
  getApprovalStatusFromValue,
  getApprovalStatusValue,
  validateProjectGroupForm,
} from "@/ProjectGroupServices/services";
import type {
  ProjectGroupCreateInput,
  ProjectGroupFormErrors,
  ProjectGroupFormValues,
  ProjectGroupUpdateInput,
} from "@/ProjectGroupServices/types";

interface ProjectGroupFormProps {
  mode: "create" | "edit";
  projectGroupId?: number;
  createdById?: number;
  currentUserId: number;
  initialValues?: Partial<ProjectGroupFormValues>;
  isSubmitting?: boolean;
  onSubmit: (
    payload: ProjectGroupCreateInput | ProjectGroupUpdateInput,
  ) => Promise<void> | void;
}

const buildInitialState = (
  initialValues?: Partial<ProjectGroupFormValues>,
): ProjectGroupFormValues => ({
  projectGroupName: initialValues?.projectGroupName ?? "",
  approvalStatus: initialValues?.approvalStatus ?? "pending",
  isActive: initialValues?.isActive ?? true,
});

const selectClassName =
  "h-11 w-full appearance-none rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 shadow-theme-xs focus:border-brand-300 focus:outline-hidden focus:ring-3 focus:ring-brand-500/20 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90 dark:focus:border-brand-800";

const ProjectGroupForm = ({
  mode,
  projectGroupId,
  createdById,
  currentUserId,
  initialValues,
  isSubmitting = false,
  onSubmit,
}: ProjectGroupFormProps) => {
  const [values, setValues] = useState<ProjectGroupFormValues>(() =>
    buildInitialState(initialValues),
  );
  const [errors, setErrors] = useState<ProjectGroupFormErrors>({});

  const updateField = <TField extends keyof ProjectGroupFormValues>(
    field: TField,
    value: ProjectGroupFormValues[TField],
  ) => {
    setValues((currentValues) => ({
      ...currentValues,
      [field]: value,
    }));

    setErrors((currentErrors) => ({
      ...currentErrors,
      [field]: undefined,
      form: undefined,
    }));
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const validationResult = validateProjectGroupForm(values);
    if (!validationResult.success) {
      setErrors(validationResult.errors);
      return;
    }

    if (currentUserId <= 0) {
      setErrors({
        form: "The authenticated user id is missing. Sign in again before saving a project group.",
      });
      return;
    }

    if (mode === "edit" && (!createdById || createdById <= 0)) {
      setErrors({
        form: "The original creator id was not returned by the PK endpoint. Reload the page and try again.",
      });
      return;
    }

    const validatedValues = validationResult.data;
    const approvalValue = getApprovalStatusValue(validatedValues.approvalStatus);
    const approvedById =
      validatedValues.approvalStatus === "pending" ? null : currentUserId;

    if (mode === "edit") {
      await onSubmit({
        projectGroupId: projectGroupId ?? 0,
        projectGroupName: validatedValues.projectGroupName,
        isApproved: approvalValue,
        approvedById,
        isActive: validatedValues.isActive,
        createdById: createdById ?? 0,
        modifiedById: currentUserId,
      });
      return;
    }

    await onSubmit({
      projectGroupName: validatedValues.projectGroupName,
      isApproved: approvalValue,
      isActive: validatedValues.isActive,
      approvedById,
      createdById: currentUserId,
    });
  };

  return (
    <form className="space-y-5" onSubmit={handleSubmit}>
      <div>
        <Label htmlFor="project-group-name">Project Group Name</Label>
        <Input
          id="project-group-name"
          name="projectGroupName"
          placeholder="Enter the project group name"
          value={values.projectGroupName}
          onChange={(event) =>
            updateField("projectGroupName", event.target.value)
          }
          error={Boolean(errors.projectGroupName)}
          hint={errors.projectGroupName}
        />
      </div>

      <div className="grid gap-5 md:grid-cols-2">
        <div>
          <Label htmlFor="project-group-approval">Approval Status</Label>
          <select
            id="project-group-approval"
            name="approvalStatus"
            value={values.approvalStatus}
            onChange={(event) =>
              updateField(
                "approvalStatus",
                event.target.value as ProjectGroupFormValues["approvalStatus"],
              )
            }
            className={selectClassName}
          >
            <option value="pending">Pending review</option>
            <option value="approved">Approved</option>
            <option value="rejected">Rejected</option>
          </select>
          {errors.approvalStatus ? (
            <p className="mt-1.5 text-xs text-error-500">{errors.approvalStatus}</p>
          ) : null}
        </div>

        <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 dark:border-gray-800 dark:bg-gray-900/50">
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-gray-500 dark:text-gray-400">
            Record Status
          </p>
          <div className="mt-3">
            <Checkbox
              checked={values.isActive}
              onChange={(checked) => updateField("isActive", checked)}
              label={values.isActive ? "Active and assignable" : "Inactive"}
            />
          </div>
        </div>
      </div>

      {errors.form ? (
        <p className="text-sm text-error-500">{errors.form}</p>
      ) : null}

      <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 text-xs text-gray-500 dark:border-gray-800 dark:bg-gray-900/50 dark:text-gray-400">
        {mode === "edit"
          ? `Editing project group #${projectGroupId ?? 0}. When you choose Approved or Rejected, the current signed-in user is sent as ApprovedByID.`
          : "Creating a project group. Pending review maps to a null IsApproved value in the backend."}
      </div>

      <Button className="w-full sm:w-auto" size="sm" disabled={isSubmitting}>
        {isSubmitting
          ? mode === "edit"
            ? "Saving project group..."
            : "Creating project group..."
          : mode === "edit"
            ? "Save project group"
            : "Create project group"}
      </Button>
    </form>
  );
};

export const buildProjectGroupFormValues = (
  input?: Partial<ProjectGroupUpdateInput>,
): Partial<ProjectGroupFormValues> | undefined => {
  if (!input) {
    return undefined;
  }

  return {
    projectGroupName: input.projectGroupName ?? "",
    approvalStatus: getApprovalStatusFromValue(input.isApproved),
    isActive: input.isActive ?? true,
  };
};

export default ProjectGroupForm;
