import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router";
import { useAuth } from "@/AuthServices/components";
import ComponentCard from "@/components/common/ComponentCard";
import { FeedbackStack, PageShell, RoleForm } from "@/UserServices/components";
import {
  createRole,
  getExceptionFeedback,
  getRolePrimaryKey,
  updateRole,
} from "@/UserServices/services";
import type { ExceptionFeedback } from "@/UserServices/services";
import type {
  PageNotice,
  RoleCreateInput,
  RoleFormValues,
  RoleUpdateInput,
} from "@/UserServices/types";

interface RoleFormPageProps {
  mode: "create" | "edit";
}

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const RoleFormPage = ({ mode }: RoleFormPageProps) => {
  const navigate = useNavigate();
  const params = useParams();
  const roleId = Number(params.roleId ?? "0");
  const { user } = useAuth();
  const [initialValues, setInitialValues] = useState<Partial<RoleFormValues>>();
  const [isLoading, setIsLoading] = useState(mode === "edit");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [notice, setNotice] = useState<PageNotice | null>(null);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const loadForm = useCallback(async () => {
    if (mode !== "edit") {
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const rolePrimaryKey = await getRolePrimaryKey(roleId);
      setInitialValues({
        roleName: rolePrimaryKey.roleName,
        description: rolePrimaryKey.description,
        isActive: rolePrimaryKey.isActive,
      });
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [mode, roleId]);

  useEffect(() => {
    void loadForm();
  }, [loadForm]);

  const handleSubmit = async (payload: RoleCreateInput | RoleUpdateInput) => {
    setIsSubmitting(true);
    setNotice(null);
    setError(null);

    try {
      if (mode === "edit") {
        await updateRole(payload as RoleUpdateInput);
        navigate("/roles", {
          replace: true,
          state: {
            notice: {
              variant: "success",
              title: "Role updated",
              message: "The role has been updated successfully.",
            },
          },
        });
        return;
      }

      await createRole({
        ...(payload as RoleCreateInput),
        createdById: user?.userId ?? 0,
      });
      navigate("/roles", {
        replace: true,
        state: {
          notice: {
            variant: "success",
            title: "Role created",
            message: "The role has been created successfully.",
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
      title={mode === "edit" ? "Edit Role" : "Create Role"}
      description={
        mode === "edit"
          ? "Update role metadata and active state."
          : "Create a new role entry that can be assigned across user and project workflows."
      }
      actions={<Link to="/roles" className={linkButtonClass}>Back to Roles</Link>}
    >
      <FeedbackStack notice={notice} error={error} />
      <ComponentCard
        title={mode === "edit" ? "Role Editor" : "New Role"}
        desc="The payload maps to RoleCreateDTO and RoleUpdateDTO in the backend service."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading role data...</p>
        ) : (
          <RoleForm
            mode={mode}
            roleId={roleId}
            createdById={user?.userId ?? 0}
            initialValues={initialValues}
            isSubmitting={isSubmitting}
            onSubmit={handleSubmit}
          />
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default RoleFormPage;
