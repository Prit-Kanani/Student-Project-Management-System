import { useState, type FormEvent } from "react";
import Checkbox from "@/components/form/input/Checkbox";
import Input from "@/components/form/input/InputField";
import Label from "@/components/form/Label";
import Button from "@/components/ui/button/Button";
import { validateRoleForm } from "@/UserServices/services/form-validation";
import type {
  RoleCreateInput,
  RoleFormErrors,
  RoleFormValues,
  RoleUpdateInput,
} from "@/UserServices/types";

interface RoleFormProps {
  mode: "create" | "edit";
  roleId?: number;
  createdById?: number;
  initialValues?: Partial<RoleFormValues>;
  isSubmitting?: boolean;
  onSubmit: (payload: RoleCreateInput | RoleUpdateInput) => Promise<void> | void;
}

const buildInitialState = (
  initialValues?: Partial<RoleFormValues>,
): RoleFormValues => ({
  roleName: initialValues?.roleName ?? "",
  description: initialValues?.description ?? "",
  isActive: initialValues?.isActive ?? true,
});

const RoleForm = ({
  mode,
  roleId,
  createdById,
  initialValues,
  isSubmitting = false,
  onSubmit,
}: RoleFormProps) => {
  const [values, setValues] = useState<RoleFormValues>(() =>
    buildInitialState(initialValues),
  );
  const [errors, setErrors] = useState<RoleFormErrors>({});

  const updateField = <TField extends keyof RoleFormValues>(
    field: TField,
    value: RoleFormValues[TField],
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

    const validationResult = validateRoleForm(values);
    if (!validationResult.success) {
      setErrors(validationResult.errors);
      return;
    }

    const validatedValues = validationResult.data;

    if (mode === "edit") {
      await onSubmit({
        roleId: roleId ?? 0,
        roleName: validatedValues.roleName,
        description: validatedValues.description,
        isActive: validatedValues.isActive,
      });
      return;
    }

    await onSubmit({
      roleName: validatedValues.roleName,
      description: validatedValues.description,
      isActive: validatedValues.isActive,
      createdById: createdById ?? 0,
    });
  };

  return (
    <form className="space-y-5" onSubmit={handleSubmit}>
      <div>
        <Label htmlFor="role-name">Role Name</Label>
        <Input
          id="role-name"
          name="roleName"
          placeholder="Enter role name"
          value={values.roleName}
          onChange={(event) => updateField("roleName", event.target.value)}
          error={Boolean(errors.roleName)}
          hint={errors.roleName}
        />
      </div>

      <div>
        <Label htmlFor="role-description">Description</Label>
        <textarea
          id="role-description"
          name="description"
          rows={5}
          value={values.description}
          onChange={(event) => updateField("description", event.target.value)}
          placeholder="Describe how this role is expected to be used"
          className="w-full rounded-lg border border-gray-300 bg-transparent px-4 py-3 text-sm text-gray-800 shadow-theme-xs placeholder:text-gray-400 focus:border-brand-300 focus:outline-hidden focus:ring-3 focus:ring-brand-500/20 dark:border-gray-700 dark:bg-gray-900 dark:text-white/90 dark:focus:border-brand-800"
        />
        {errors.description ? (
          <p className="mt-1.5 text-xs text-error-500">{errors.description}</p>
        ) : null}
      </div>

      <div className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 dark:border-gray-800 dark:bg-gray-900/50">
        <p className="text-xs font-semibold uppercase tracking-[0.16em] text-gray-500 dark:text-gray-400">
          Role Status
        </p>
        <div className="mt-3">
          <Checkbox
            checked={values.isActive}
            onChange={(checked) => updateField("isActive", checked)}
            label={values.isActive ? "Role is active" : "Role is inactive"}
          />
        </div>
      </div>

      <Button className="w-full sm:w-auto" size="sm" disabled={isSubmitting}>
        {isSubmitting
          ? mode === "edit"
            ? "Saving role..."
            : "Creating role..."
          : mode === "edit"
            ? "Save role"
            : "Create role"}
      </Button>
    </form>
  );
};

export default RoleForm;
