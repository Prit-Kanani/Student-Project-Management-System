import { useState, type FormEvent } from "react";
import Checkbox from "@/components/form/input/Checkbox";
import Label from "@/components/form/Label";
import Button from "@/components/ui/button/Button";
import { validateProjectGroupByProjectForm } from "@/ProjectGroupServices/services";
import type {
  LookupOption,
  ProjectGroupByProjectCreateInput,
  ProjectGroupByProjectFormErrors,
  ProjectGroupByProjectFormValues,
  ProjectGroupByProjectUpdateInput,
} from "@/ProjectGroupServices/types";

interface ProjectGroupByProjectFormProps {
  mode: "create" | "edit";
  projectGroupByProjectId?: number;
  currentUserId: number;
  projectGroupOptions: LookupOption[];
  projectOptions: LookupOption[];
  initialValues?: Partial<ProjectGroupByProjectFormValues>;
  isSubmitting?: boolean;
  onSubmit: (
    payload: ProjectGroupByProjectCreateInput | ProjectGroupByProjectUpdateInput,
  ) => Promise<void> | void;
}

const buildInitialState = (
  initialValues?: Partial<ProjectGroupByProjectFormValues>,
): ProjectGroupByProjectFormValues => ({
  projectGroupId: initialValues?.projectGroupId ?? "",
  projectId: initialValues?.projectId ?? "",
  isActive: initialValues?.isActive ?? true,
});

const selectClassName =
  "h-11 w-full appearance-none rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 shadow-theme-xs focus:border-brand-300 focus:outline-hidden focus:ring-3 focus:ring-brand-500/20 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90 dark:focus:border-brand-800";

const ProjectGroupByProjectForm = ({
  mode,
  projectGroupByProjectId,
  currentUserId,
  projectGroupOptions,
  projectOptions,
  initialValues,
  isSubmitting = false,
  onSubmit,
}: ProjectGroupByProjectFormProps) => {
  const [values, setValues] = useState<ProjectGroupByProjectFormValues>(() =>
    buildInitialState(initialValues),
  );
  const [errors, setErrors] = useState<ProjectGroupByProjectFormErrors>({});

  const updateField = <TField extends keyof ProjectGroupByProjectFormValues>(
    field: TField,
    value: ProjectGroupByProjectFormValues[TField],
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

    const validationResult = validateProjectGroupByProjectForm(values);
    if (!validationResult.success) {
      setErrors(validationResult.errors);
      return;
    }

    if (currentUserId <= 0) {
      setErrors({
        form: "The authenticated user id is missing. Sign in again before saving the mapping.",
      });
      return;
    }

    const validatedValues = validationResult.data;
    const sharedPayload = {
      projectGroupId: Number(validatedValues.projectGroupId),
      projectId: Number(validatedValues.projectId),
      isActive: validatedValues.isActive,
    };

    if (mode === "edit") {
      await onSubmit({
        projectGroupByProjectId: projectGroupByProjectId ?? 0,
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
      <div className="grid gap-5 md:grid-cols-2">
        <div>
          <Label htmlFor="project-group-id">Project Group</Label>
          <select
            id="project-group-id"
            name="projectGroupId"
            value={values.projectGroupId}
            onChange={(event) =>
              updateField("projectGroupId", event.target.value)
            }
            className={selectClassName}
          >
            <option value="">Select a project group</option>
            {projectGroupOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
          {errors.projectGroupId ? (
            <p className="mt-1.5 text-xs text-error-500">{errors.projectGroupId}</p>
          ) : null}
        </div>

        <div>
          <Label htmlFor="project-id">Project</Label>
          <select
            id="project-id"
            name="projectId"
            value={values.projectId}
            onChange={(event) => updateField("projectId", event.target.value)}
            className={selectClassName}
          >
            <option value="">Select a project</option>
            {projectOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
          {errors.projectId ? (
            <p className="mt-1.5 text-xs text-error-500">{errors.projectId}</p>
          ) : null}
        </div>
      </div>

      <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 dark:border-gray-800 dark:bg-gray-900/50">
        <p className="text-xs font-semibold uppercase tracking-[0.16em] text-gray-500 dark:text-gray-400">
          Mapping Status
        </p>
        <div className="mt-3">
          <Checkbox
            checked={values.isActive}
            onChange={(checked) => updateField("isActive", checked)}
            label={values.isActive ? "Active mapping" : "Inactive mapping"}
          />
        </div>
      </div>

      {errors.form ? (
        <p className="text-sm text-error-500">{errors.form}</p>
      ) : null}

      <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 text-xs text-gray-500 dark:border-gray-800 dark:bg-gray-900/50 dark:text-gray-400">
        {mode === "edit"
          ? `Editing mapping #${projectGroupByProjectId ?? 0}. The backend validates that the selected ProjectID exists before update.`
          : "Creating a group-to-project mapping. The backend rejects duplicate or invalid group and project combinations."}
      </div>

      <Button className="w-full sm:w-auto" size="sm" disabled={isSubmitting}>
        {isSubmitting
          ? mode === "edit"
            ? "Saving mapping..."
            : "Creating mapping..."
          : mode === "edit"
            ? "Save mapping"
            : "Create mapping"}
      </Button>
    </form>
  );
};

export const buildProjectGroupByProjectFormValues = (
  input?: Partial<ProjectGroupByProjectUpdateInput>,
): Partial<ProjectGroupByProjectFormValues> | undefined => {
  if (!input) {
    return undefined;
  }

  return {
    projectGroupId:
      input.projectGroupId && input.projectGroupId > 0
        ? String(input.projectGroupId)
        : "",
    projectId:
      input.projectId && input.projectId > 0 ? String(input.projectId) : "",
    isActive: input.isActive ?? true,
  };
};

export default ProjectGroupByProjectForm;
