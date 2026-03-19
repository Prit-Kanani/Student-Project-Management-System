import { useMemo, useState, type FormEvent } from "react";
import type { OptionDTO } from "@/CommonLibrary/types";
import Checkbox from "@/components/form/input/Checkbox";
import Input from "@/components/form/input/InputField";
import Label from "@/components/form/Label";
import Button from "@/components/ui/button/Button";
import { validateUserForm } from "@/UserServices/services/form-validation";
import type {
  UserCreateInput,
  UserFormErrors,
  UserFormValues,
  UserUpdateInput,
} from "@/UserServices/types";

interface UserFormProps {
  mode: "create" | "edit";
  userId?: number;
  roleOptions: OptionDTO[];
  initialValues?: Partial<UserFormValues>;
  isSubmitting?: boolean;
  onSubmit: (payload: UserCreateInput | UserUpdateInput) => Promise<void> | void;
}

const buildInitialState = (
  initialValues?: Partial<UserFormValues>,
): UserFormValues => ({
  name: initialValues?.name ?? "",
  email: initialValues?.email ?? "",
  password: initialValues?.password ?? "",
  roleId: initialValues?.roleId ?? "",
  isActive: initialValues?.isActive ?? true,
});

const UserForm = ({
  mode,
  userId,
  roleOptions,
  initialValues,
  isSubmitting = false,
  onSubmit,
}: UserFormProps) => {
  const [values, setValues] = useState<UserFormValues>(() =>
    buildInitialState(initialValues),
  );
  const [errors, setErrors] = useState<UserFormErrors>({});

  const isEditMode = mode === "edit";

  const roleHint = useMemo(() => {
    return roleOptions.find((option) => String(option.id) === values.roleId)?.name;
  }, [roleOptions, values.roleId]);

  const updateField = <TField extends keyof UserFormValues>(
    field: TField,
    value: UserFormValues[TField],
  ) => {
    setValues((currentValues) => ({
      ...currentValues,
      [field]: value,
    }));

    setErrors((currentErrors) => ({
      ...currentErrors,
      [field]: undefined,
    }));
  };

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const validationResult = validateUserForm(mode, values);
    if (!validationResult.success) {
      setErrors(validationResult.errors);
      return;
    }

    const validatedValues = validationResult.data;

    if (isEditMode) {
      await onSubmit({
        userId: userId ?? 0,
        name: validatedValues.name,
        email: validatedValues.email,
        roleId: Number(validatedValues.roleId),
        isActive: validatedValues.isActive,
      });
      return;
    }

    await onSubmit({
      name: validatedValues.name,
      email: validatedValues.email,
      password: validatedValues.password,
      roleId: Number(validatedValues.roleId),
      isActive: validatedValues.isActive,
    });
  };

  return (
    <form className="space-y-5" onSubmit={handleSubmit}>
      <div className="grid gap-5 md:grid-cols-2">
        <div>
          <Label htmlFor="user-name">Full Name</Label>
          <Input
            id="user-name"
            name="name"
            placeholder="Enter the full name"
            value={values.name}
            onChange={(event) => updateField("name", event.target.value)}
            error={Boolean(errors.name)}
            hint={errors.name}
          />
        </div>

        <div>
          <Label htmlFor="user-email">Email</Label>
          <Input
            id="user-email"
            name="email"
            type="email"
            placeholder="name@college.edu"
            value={values.email}
            onChange={(event) => updateField("email", event.target.value)}
            error={Boolean(errors.email)}
            hint={errors.email}
          />
        </div>
      </div>

      {!isEditMode && (
        <div>
          <Label htmlFor="user-password">Password</Label>
          <Input
            id="user-password"
            name="password"
            type="password"
            placeholder="Set a secure password"
            value={values.password}
            onChange={(event) => updateField("password", event.target.value)}
            error={Boolean(errors.password)}
            hint={errors.password}
          />
        </div>
      )}

      <div className="grid gap-5 md:grid-cols-[minmax(0,1fr)_240px] md:items-start">
        <div>
          <Label htmlFor="user-role">Role</Label>
          <select
            id="user-role"
            name="roleId"
            value={values.roleId}
            onChange={(event) => updateField("roleId", event.target.value)}
            className={`h-11 w-full appearance-none rounded-lg border bg-transparent px-4 py-2.5 text-sm shadow-theme-xs focus:outline-hidden focus:ring-3 dark:bg-gray-900 dark:text-white/90 ${
              errors.roleId
                ? "border-error-500 focus:border-error-300 focus:ring-error-500/20 dark:border-error-500"
                : "border-gray-300 text-gray-800 focus:border-brand-300 focus:ring-brand-500/20 dark:border-gray-700 dark:text-white/90 dark:focus:border-brand-800"
            }`}
          >
            <option value="">Select a role</option>
            {roleOptions.map((roleOption) => (
              <option key={roleOption.id} value={roleOption.id}>
                {roleOption.name}
              </option>
            ))}
          </select>
          <p className={`mt-1.5 text-xs ${errors.roleId ? "text-error-500" : "text-gray-500 dark:text-gray-400"}`}>
            {errors.roleId || roleHint || "Assign the role that controls this user's access."}
          </p>
        </div>

        <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 dark:border-gray-800 dark:bg-gray-900/50">
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-gray-500 dark:text-gray-400">
            Account Status
          </p>
          <div className="mt-3">
            <Checkbox
              checked={values.isActive}
              onChange={(checked) => updateField("isActive", checked)}
              label={values.isActive ? "Active and usable" : "Inactive"}
            />
          </div>
        </div>
      </div>

      <Button className="w-full sm:w-auto" size="sm" disabled={isSubmitting}>
        {isSubmitting
          ? isEditMode
            ? "Saving user..."
            : "Creating user..."
          : isEditMode
            ? "Save user"
            : "Create user"}
      </Button>
    </form>
  );
};

export default UserForm;
