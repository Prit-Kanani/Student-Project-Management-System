import { z, type ZodError } from "zod";
import type {
  RoleFormErrors,
  RoleFormValues,
  UserFormErrors,
  UserFormValues,
  UserProfileFormErrors,
  UserProfileFormValues,
} from "@/UserServices/types";

type ValidationResult<TData, TErrors> =
  | { success: true; data: TData }
  | { success: false; errors: TErrors };

const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const PASSWORD_UPPERCASE_PATTERN = /[A-Z]/;
const PASSWORD_LOWERCASE_PATTERN = /[a-z]/;
const PASSWORD_DIGIT_PATTERN = /[0-9]/;
const PASSWORD_SPECIAL_CHARACTER_PATTERN = /[^a-zA-Z0-9]/;

const userErrorFields = ["name", "email", "password", "roleId"] as const;
const roleErrorFields = ["roleName", "description"] as const;
const userProfileErrorFields = [
  "userId",
  "displayName",
  "phoneNumber",
  "profileImageUrl",
  "gender",
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

const mapUserErrors = createFieldErrorMapper(userErrorFields);
const mapRoleErrors = createFieldErrorMapper(roleErrorFields);
const mapUserProfileErrors = createFieldErrorMapper(userProfileErrorFields);

const requiredRoleIdSchema = z
  .string()
  .trim()
  .min(1, "Select a role.")
  .refine((value) => Number.isFinite(Number(value)) && Number(value) > 0, {
    message: "Select a role.",
  });

const requiredUserIdSchema = z
  .string()
  .trim()
  .min(1, "Select a user account.")
  .refine((value) => Number.isFinite(Number(value)) && Number(value) > 0, {
    message: "Select a user account.",
  });

const userNameSchema = z
  .string()
  .trim()
  .min(1, "Name is required.")
  .min(3, "Name must be at least 3 characters long.")
  .max(50, "Name must not exceed 50 characters.");

const userEmailSchema = z
  .string()
  .trim()
  .min(1, "Email is required.")
  .regex(EMAIL_PATTERN, "Enter a valid email address.");

const userPasswordSchema = z
  .string()
  .min(1, "Password is required.")
  .min(8, "Password must be at least 8 characters long.")
  .regex(PASSWORD_UPPERCASE_PATTERN, "Password must contain at least one uppercase letter.")
  .regex(PASSWORD_LOWERCASE_PATTERN, "Password must contain at least one lowercase letter.")
  .regex(PASSWORD_DIGIT_PATTERN, "Password must contain at least one digit.")
  .regex(
    PASSWORD_SPECIAL_CHARACTER_PATTERN,
    "Password must contain at least one special character.",
  );

const createUserFormSchema = z.object({
  name: userNameSchema,
  email: userEmailSchema,
  password: userPasswordSchema,
  roleId: requiredRoleIdSchema,
  isActive: z.boolean(),
});

const editUserFormSchema = z.object({
  name: userNameSchema,
  email: userEmailSchema,
  password: z.string(),
  roleId: requiredRoleIdSchema,
  isActive: z.boolean(),
});

const roleFormSchema = z.object({
  roleName: z
    .string()
    .trim()
    .min(1, "Role name is required.")
    .max(50, "Role name must not exceed 50 characters."),
  description: z
    .string()
    .trim()
    .max(250, "Description must not exceed 250 characters."),
  isActive: z.boolean(),
});

const userProfileFormSchema = z.object({
  userId: requiredUserIdSchema,
  displayName: z
    .string()
    .trim()
    .min(1, "Display name is required.")
    .max(100, "Display name must not exceed 100 characters."),
  firstName: z.string().trim(),
  lastName: z.string().trim(),
  phoneNumber: z
    .string()
    .trim()
    .max(20, "Phone number must not exceed 20 characters."),
  profileImageUrl: z
    .string()
    .trim()
    .max(500, "Profile image URL must not exceed 500 characters."),
  dateOfBirth: z.string(),
  gender: z
    .string()
    .trim()
    .max(30, "Gender must not exceed 30 characters."),
});

export const validateUserForm = (
  mode: "create" | "edit",
  values: UserFormValues,
): ValidationResult<UserFormValues, UserFormErrors> => {
  const result =
    mode === "create"
      ? createUserFormSchema.safeParse(values)
      : editUserFormSchema.safeParse(values);

  if (!result.success) {
    return {
      success: false,
      errors: mapUserErrors(result.error) as UserFormErrors,
    };
  }

  return {
    success: true,
    data: result.data,
  };
};

export const validateRoleForm = (
  values: RoleFormValues,
): ValidationResult<RoleFormValues, RoleFormErrors> => {
  const result = roleFormSchema.safeParse(values);

  if (!result.success) {
    return {
      success: false,
      errors: mapRoleErrors(result.error) as RoleFormErrors,
    };
  }

  return {
    success: true,
    data: result.data,
  };
};

export const validateUserProfileForm = (
  values: UserProfileFormValues,
): ValidationResult<UserProfileFormValues, UserProfileFormErrors> => {
  const result = userProfileFormSchema.safeParse(values);

  if (!result.success) {
    return {
      success: false,
      errors: mapUserProfileErrors(result.error) as UserProfileFormErrors,
    };
  }

  return {
    success: true,
    data: result.data,
  };
};
