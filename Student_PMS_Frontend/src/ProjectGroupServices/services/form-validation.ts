import { z, type ZodError } from "zod";
import type {
  GroupWiseStudentFormErrors,
  GroupWiseStudentFormValues,
  ProjectGroupByProjectFormErrors,
  ProjectGroupByProjectFormValues,
  ProjectGroupFormErrors,
  ProjectGroupFormValues,
} from "@/ProjectGroupServices/types";

type ValidationResult<TData, TErrors> =
  | { success: true; data: TData }
  | { success: false; errors: TErrors };

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

const projectGroupFieldErrors = ["projectGroupName", "approvalStatus"] as const;
const projectGroupByProjectFieldErrors = ["projectGroupId", "projectId"] as const;
const groupWiseStudentFieldErrors = ["projectGroupId", "studentId"] as const;

const mapProjectGroupErrors = createFieldErrorMapper(projectGroupFieldErrors);
const mapProjectGroupByProjectErrors = createFieldErrorMapper(
  projectGroupByProjectFieldErrors,
);
const mapGroupWiseStudentErrors = createFieldErrorMapper(groupWiseStudentFieldErrors);

const numericIdentifier = z
  .string()
  .trim()
  .min(1, "A selection is required.")
  .refine((value) => Number.isInteger(Number(value)) && Number(value) > 0, {
    message: "Choose a valid record.",
  });

const projectGroupFormSchema = z.object({
  projectGroupName: z
    .string()
    .trim()
    .min(1, "Project group name is required.")
    .min(5, "Project group name must be at least 5 characters long.")
    .max(100, "Project group name must not exceed 100 characters."),
  approvalStatus: z.enum(["pending", "approved", "rejected"]),
  isActive: z.boolean(),
});

const projectGroupByProjectFormSchema = z.object({
  projectGroupId: numericIdentifier,
  projectId: numericIdentifier,
  isActive: z.boolean(),
});

const groupWiseStudentFormSchema = z.object({
  projectGroupId: numericIdentifier,
  studentId: numericIdentifier,
  isActive: z.boolean(),
});

export const validateProjectGroupForm = (
  values: ProjectGroupFormValues,
): ValidationResult<ProjectGroupFormValues, ProjectGroupFormErrors> => {
  const result = projectGroupFormSchema.safeParse(values);

  if (!result.success) {
    return {
      success: false,
      errors: mapProjectGroupErrors(result.error) as ProjectGroupFormErrors,
    };
  }

  return {
    success: true,
    data: result.data,
  };
};

export const validateProjectGroupByProjectForm = (
  values: ProjectGroupByProjectFormValues,
): ValidationResult<
  ProjectGroupByProjectFormValues,
  ProjectGroupByProjectFormErrors
> => {
  const result = projectGroupByProjectFormSchema.safeParse(values);

  if (!result.success) {
    return {
      success: false,
      errors:
        mapProjectGroupByProjectErrors(
          result.error,
        ) as ProjectGroupByProjectFormErrors,
    };
  }

  return {
    success: true,
    data: result.data,
  };
};

export const validateGroupWiseStudentForm = (
  values: GroupWiseStudentFormValues,
): ValidationResult<GroupWiseStudentFormValues, GroupWiseStudentFormErrors> => {
  const result = groupWiseStudentFormSchema.safeParse(values);

  if (!result.success) {
    return {
      success: false,
      errors: mapGroupWiseStudentErrors(result.error) as GroupWiseStudentFormErrors,
    };
  }

  return {
    success: true,
    data: result.data,
  };
};
