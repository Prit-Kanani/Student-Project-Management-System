import { z, type ZodError } from "zod";
import type {
  ProjectFormErrors,
  ProjectFormValues,
} from "@/ProjectServices/types";

type ValidationResult<TData, TErrors> =
  | { success: true; data: TData }
  | { success: false; errors: TErrors };

const projectErrorFields = [
  "projectName",
  "description",
  "approvalStatus",
] as const;

const createFieldErrorMapper =
  <TField extends string>(allowedFields: readonly TField[]) =>
  (error: ZodError): Partial<Record<TField, string>> => {
    const nextErrors: Partial<Record<TField, string>> = {};
    const allowedFieldSet = new Set(allowedFields);

    for (const issue of error.issues) {
      const field = issue.path[0];

      if (
        typeof field !== "string" ||
        !allowedFieldSet.has(field as TField) ||
        nextErrors[field as TField]
      ) {
        continue;
      }

      nextErrors[field as TField] = issue.message;
    }

    return nextErrors;
  };

const mapProjectErrors = createFieldErrorMapper(projectErrorFields);

const projectFormSchema = z.object({
  projectName: z
    .string()
    .trim()
    .min(1, "Project name is required.")
    .min(5, "Project name must be at least 5 characters long.")
    .max(100, "Project name must not exceed 100 characters."),
  description: z
    .string()
    .trim()
    .max(500, "Description must not exceed 500 characters."),
  approvalStatus: z.enum(["pending", "approved", "rejected"]),
  isActive: z.boolean(),
  isCompleted: z.boolean(),
});

export const validateProjectForm = (
  values: ProjectFormValues,
): ValidationResult<ProjectFormValues, ProjectFormErrors> => {
  const result = projectFormSchema.safeParse(values);

  if (!result.success) {
    return {
      success: false,
      errors: mapProjectErrors(result.error) as ProjectFormErrors,
    };
  }

  return {
    success: true,
    data: result.data,
  };
};
