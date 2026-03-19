import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router";
import type { OptionDTO } from "@/CommonLibrary/types";
import ComponentCard from "@/components/common/ComponentCard";
import { FeedbackStack, PageShell, UserForm } from "@/UserServices/components";
import {
  createUser,
  getExceptionFeedback,
  getRoleOptions,
  getUserPrimaryKey,
  updateUser,
} from "@/UserServices/services";
import type { ExceptionFeedback } from "@/UserServices/services";
import type {
  PageNotice,
  UserCreateInput,
  UserFormValues,
  UserUpdateInput,
} from "@/UserServices/types";

interface UserFormPageProps {
  mode: "create" | "edit";
}

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const UserFormPage = ({ mode }: UserFormPageProps) => {
  const navigate = useNavigate();
  const params = useParams();
  const userId = Number(params.userId ?? "0");
  const [roleOptions, setRoleOptions] = useState<OptionDTO[]>([]);
  const [initialValues, setInitialValues] = useState<Partial<UserFormValues>>();
  const [isLoading, setIsLoading] = useState(mode === "edit");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [notice, setNotice] = useState<PageNotice | null>(null);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const loadForm = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [roles, userPrimaryKey] = await Promise.all([
        getRoleOptions(),
        mode === "edit" ? getUserPrimaryKey(userId) : Promise.resolve(null),
      ]);

      setRoleOptions(roles);

      if (userPrimaryKey) {
        setInitialValues({
          name: userPrimaryKey.name,
          email: userPrimaryKey.email,
          roleId: String(userPrimaryKey.roleId),
          isActive: userPrimaryKey.isActive,
        });
      }
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [mode, userId]);

  useEffect(() => {
    void loadForm();
  }, [loadForm]);

  const handleSubmit = async (payload: UserCreateInput | UserUpdateInput) => {
    setIsSubmitting(true);
    setNotice(null);
    setError(null);

    try {
      if (mode === "edit") {
        await updateUser(payload as UserUpdateInput);
        navigate("/users", {
          replace: true,
          state: {
            notice: {
              variant: "success",
              title: "User updated",
              message: "The user record has been updated successfully.",
            },
          },
        });
        return;
      }

      await createUser(payload as UserCreateInput);
      navigate("/users", {
        replace: true,
        state: {
          notice: {
            variant: "success",
            title: "User created",
            message: "The user account has been created successfully.",
          },
        },
      });
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <PageShell
      title={mode === "edit" ? "Edit User" : "Create User"}
      description={
        mode === "edit"
          ? "Update user identity, role assignment, and active state."
          : "Create a new authenticated user record and assign the appropriate role."
      }
      actions={<Link to="/users" className={linkButtonClass}>Back to Users</Link>}
    >
      <FeedbackStack notice={notice} error={error} />
      <ComponentCard
        title={mode === "edit" ? "User Editor" : "New User"}
        desc="The payload maps to UserCreateDTO and UserUpdateDTO in the backend service."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading form data...</p>
        ) : (
          <UserForm
            mode={mode}
            userId={userId}
            roleOptions={roleOptions}
            initialValues={initialValues}
            isSubmitting={isSubmitting}
            onSubmit={handleSubmit}
          />
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default UserFormPage;
