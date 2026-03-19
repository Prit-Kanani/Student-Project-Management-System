import { useState, type FormEvent } from "react";
import Checkbox from "@/components/form/input/Checkbox";
import Label from "@/components/form/Label";
import Button from "@/components/ui/button/Button";
import { validateGroupWiseStudentForm } from "@/ProjectGroupServices/services";
import type {
  GroupWiseStudentCreateInput,
  GroupWiseStudentFormErrors,
  GroupWiseStudentFormValues,
  GroupWiseStudentUpdateInput,
  LookupOption,
} from "@/ProjectGroupServices/types";

interface GroupWiseStudentFormProps {
  mode: "create" | "edit";
  studentWiseGroupId?: number;
  createdById?: number;
  currentUserId: number;
  projectGroupOptions: LookupOption[];
  userOptions: LookupOption[];
  initialValues?: Partial<GroupWiseStudentFormValues>;
  isSubmitting?: boolean;
  onSubmit: (
    payload: GroupWiseStudentCreateInput | GroupWiseStudentUpdateInput,
  ) => Promise<void> | void;
}

const buildInitialState = (
  initialValues?: Partial<GroupWiseStudentFormValues>,
): GroupWiseStudentFormValues => ({
  projectGroupId: initialValues?.projectGroupId ?? "",
  studentId: initialValues?.studentId ?? "",
  isActive: initialValues?.isActive ?? true,
});

const selectClassName =
  "h-11 w-full appearance-none rounded-lg border border-gray-300 bg-transparent px-4 py-2.5 text-sm text-gray-800 shadow-theme-xs focus:border-brand-300 focus:outline-hidden focus:ring-3 focus:ring-brand-500/20 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90 dark:focus:border-brand-800";

const GroupWiseStudentForm = ({
  mode,
  studentWiseGroupId,
  createdById,
  currentUserId,
  projectGroupOptions,
  userOptions,
  initialValues,
  isSubmitting = false,
  onSubmit,
}: GroupWiseStudentFormProps) => {
  const [values, setValues] = useState<GroupWiseStudentFormValues>(() =>
    buildInitialState(initialValues),
  );
  const [errors, setErrors] = useState<GroupWiseStudentFormErrors>({});

  const updateField = <TField extends keyof GroupWiseStudentFormValues>(
    field: TField,
    value: GroupWiseStudentFormValues[TField],
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

    const validationResult = validateGroupWiseStudentForm(values);
    if (!validationResult.success) {
      setErrors(validationResult.errors);
      return;
    }

    if (currentUserId <= 0) {
      setErrors({
        form: "The authenticated user id is missing. Sign in again before saving the student assignment.",
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
    const sharedPayload = {
      projectGroupId: Number(validatedValues.projectGroupId),
      studentId: Number(validatedValues.studentId),
      isActive: validatedValues.isActive,
    };

    if (mode === "edit") {
      await onSubmit({
        studentWiseGroupId: studentWiseGroupId ?? 0,
        createdById: createdById ?? 0,
        modifiedById: currentUserId,
        ...sharedPayload,
      });
      return;
    }

    await onSubmit({
      createdById: currentUserId,
      modifiedById: null,
      ...sharedPayload,
    });
  };

  return (
    <form className="space-y-5" onSubmit={handleSubmit}>
      <div className="grid gap-5 md:grid-cols-2">
        <div>
          <Label htmlFor="student-project-group-id">Project Group</Label>
          <select
            id="student-project-group-id"
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
          <Label htmlFor="student-id">Student / User</Label>
          <select
            id="student-id"
            name="studentId"
            value={values.studentId}
            onChange={(event) => updateField("studentId", event.target.value)}
            className={selectClassName}
          >
            <option value="">Select a user record</option>
            {userOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
          {errors.studentId ? (
            <p className="mt-1.5 text-xs text-error-500">{errors.studentId}</p>
          ) : (
            <p className="mt-1.5 text-xs text-gray-500 dark:text-gray-400">
              The backend accepts a numeric StudentID. This list reuses the user directory so you can pick the correct record id.
            </p>
          )}
        </div>
      </div>

      <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 dark:border-gray-800 dark:bg-gray-900/50">
        <p className="text-xs font-semibold uppercase tracking-[0.16em] text-gray-500 dark:text-gray-400">
          Assignment Status
        </p>
        <div className="mt-3">
          <Checkbox
            checked={values.isActive}
            onChange={(checked) => updateField("isActive", checked)}
            label={values.isActive ? "Active assignment" : "Inactive assignment"}
          />
        </div>
      </div>

      {errors.form ? (
        <p className="text-sm text-error-500">{errors.form}</p>
      ) : null}

      <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 text-xs text-gray-500 dark:border-gray-800 dark:bg-gray-900/50 dark:text-gray-400">
        {mode === "edit"
          ? `Editing student assignment #${studentWiseGroupId ?? 0}. The update payload preserves the original CreatedByID and adds the current user as ModifiedByID.`
          : "Creating a student assignment against a project group. Choose the user record whose numeric id should be sent as StudentID."}
      </div>

      <Button className="w-full sm:w-auto" size="sm" disabled={isSubmitting}>
        {isSubmitting
          ? mode === "edit"
            ? "Saving assignment..."
            : "Creating assignment..."
          : mode === "edit"
            ? "Save assignment"
            : "Create assignment"}
      </Button>
    </form>
  );
};

export const buildGroupWiseStudentFormValues = (
  input?: Partial<GroupWiseStudentUpdateInput>,
): Partial<GroupWiseStudentFormValues> | undefined => {
  if (!input) {
    return undefined;
  }

  return {
    projectGroupId:
      input.projectGroupId && input.projectGroupId > 0
        ? String(input.projectGroupId)
        : "",
    studentId:
      input.studentId && input.studentId > 0 ? String(input.studentId) : "",
    isActive: input.isActive ?? true,
  };
};

export default GroupWiseStudentForm;
