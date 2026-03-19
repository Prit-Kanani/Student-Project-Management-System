import { useState, type FormEvent } from "react";
import Checkbox from "@/components/form/input/Checkbox";
import Input from "@/components/form/input/InputField";
import Label from "@/components/form/Label";
import Button from "@/components/ui/button/Button";
import {
  getApprovalStatusFromValue,
  getApprovalStatusValue,
  validateProjectForm,
} from "@/ProjectServices/services";
import type {
  ProjectCreateInput,
  ProjectFormErrors,
  ProjectFormValues,
  ProjectUpdateInput,
} from "@/ProjectServices/types";

interface ProjectFormProps {
  mode: "create" | "edit";
  projectId?: number;
  currentUserId: number;
  initialValues?: Partial<ProjectFormValues>;
  isSubmitting?: boolean;
  onSubmit: (
    payload: ProjectCreateInput | ProjectUpdateInput,
  ) => Promise<void> | void;
}

const buildInitialState = (
  initialValues?: Partial<ProjectFormValues>,
): ProjectFormValues => ({
  projectName: initialValues?.projectName ?? "",
  description: initialValues?.description ?? "",
  approvalStatus: initialValues?.approvalStatus ?? "pending",
  isActive: initialValues?.isActive ?? true,
  isCompleted: initialValues?.isCompleted ?? false,
});

const selectClassName =
  "h-11 w-full appearance-none rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 shadow-theme-xs focus:border-brand-300 focus:outline-hidden focus:ring-3 focus:ring-brand-500/20 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90 dark:focus:border-brand-800";

const ProjectForm = ({
  mode,
  projectId,
  currentUserId,
  initialValues,
  isSubmitting = false,
  onSubmit,
}: ProjectFormProps) => {
  const [values, setValues] = useState<ProjectFormValues>(() =>
    buildInitialState(initialValues),
  );
  const [errors, setErrors] = useState<ProjectFormErrors>({});

  const updateField = <TField extends keyof ProjectFormValues>(
    field: TField,
    value: ProjectFormValues[TField],
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

    const validationResult = validateProjectForm(values);
    if (!validationResult.success) {
      setErrors(validationResult.errors);
      return;
    }

    if (currentUserId <= 0) {
      setErrors({
        form: "The authenticated user id is missing. Sign in again before saving a project.",
      });
      return;
    }

    const validatedValues = validationResult.data;
    const sharedPayload = {
      projectName: validatedValues.projectName,
      description: validatedValues.description,
      isApproved: getApprovalStatusValue(validatedValues.approvalStatus),
      isActive: validatedValues.isActive,
      isCompleted: validatedValues.isCompleted,
    };

    if (mode === "edit") {
      await onSubmit({
        projectId: projectId ?? 0,
        modifiedById: currentUserId,
        ...sharedPayload,
      });
      return;
    }

    await onSubmit({
      createdById: currentUserId,
      ...sharedPayload,
    });
  };

  return (
    <form className="space-y-5" onSubmit={handleSubmit}>
      <div>
        <Label htmlFor="project-name">Project Name</Label>
        <Input
          id="project-name"
          name="projectName"
          placeholder="Enter the project name"
          value={values.projectName}
          onChange={(event) => updateField("projectName", event.target.value)}
          error={Boolean(errors.projectName)}
          hint={errors.projectName}
        />
      </div>

      <div>
        <Label htmlFor="project-description">Description</Label>
        <textarea
          id="project-description"
          name="description"
          rows={5}
          value={values.description}
          onChange={(event) => updateField("description", event.target.value)}
          placeholder="Describe the project scope, deliverables, or review notes"
          className="w-full rounded-lg border border-gray-300 bg-transparent px-4 py-3 text-sm text-gray-800 shadow-theme-xs placeholder:text-gray-400 focus:border-brand-300 focus:outline-hidden focus:ring-3 focus:ring-brand-500/20 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90 dark:focus:border-brand-800"
        />
        {errors.description ? (
          <p className="mt-1.5 text-xs text-error-500">{errors.description}</p>
        ) : null}
      </div>

      <div className="grid gap-5 md:grid-cols-3">
        <div>
          <Label htmlFor="project-approval">Approval Status</Label>
          <select
            id="project-approval"
            name="approvalStatus"
            value={values.approvalStatus}
            onChange={(event) =>
              updateField(
                "approvalStatus",
                event.target.value as ProjectFormValues["approvalStatus"],
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
            Project Status
          </p>
          <div className="mt-3">
            <Checkbox
              checked={values.isActive}
              onChange={(checked) => updateField("isActive", checked)}
              label={values.isActive ? "Active and visible" : "Inactive"}
            />
          </div>
        </div>

        <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 dark:border-gray-800 dark:bg-gray-900/50">
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-gray-500 dark:text-gray-400">
            Delivery Status
          </p>
          <div className="mt-3">
            <Checkbox
              checked={values.isCompleted}
              onChange={(checked) => updateField("isCompleted", checked)}
              label={values.isCompleted ? "Marked completed" : "Still in progress"}
            />
          </div>
        </div>
      </div>

      {errors.form ? (
        <p className="text-sm text-error-500">{errors.form}</p>
      ) : null}

      <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 text-xs text-gray-500 dark:border-gray-800 dark:bg-gray-900/50 dark:text-gray-400">
        {mode === "edit"
          ? `Editing project #${projectId ?? 0}. The approval selector maps to the backend nullable IsApproved value.`
          : "Creating a new project. Pending review maps to a null IsApproved value in the backend."}
      </div>

      <Button className="w-full sm:w-auto" size="sm" disabled={isSubmitting}>
        {isSubmitting
          ? mode === "edit"
            ? "Saving project..."
            : "Creating project..."
          : mode === "edit"
            ? "Save project"
            : "Create project"}
      </Button>
    </form>
  );
};

export const buildProjectFormValues = (
  input?: Partial<ProjectUpdateInput>,
): Partial<ProjectFormValues> | undefined => {
  if (!input) {
    return undefined;
  }

  return {
    projectName: input.projectName ?? "",
    description: input.description ?? "",
    approvalStatus: getApprovalStatusFromValue(input.isApproved),
    isActive: input.isActive ?? true,
    isCompleted: input.isCompleted ?? false,
  };
};

export default ProjectForm;
