import { useCallback, useEffect, useState } from "react";
import { Link, useParams } from "react-router";
import ComponentCard from "@/components/common/ComponentCard";
import { DetailGrid, FeedbackStack, PageShell, StatusBadge } from "@/UserServices/components";
import {
  formatDateTime,
  formatOptionalText,
  getExceptionFeedback,
  getRoleView,
} from "@/UserServices/services";
import type { ExceptionFeedback } from "@/UserServices/services";
import type { RoleView } from "@/UserServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const RoleDetailPage = () => {
  const params = useParams();
  const roleId = Number(params.roleId ?? "0");
  const [roleRecord, setRoleRecord] = useState<RoleView | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const loadRole = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      setRoleRecord(await getRoleView(roleId));
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [roleId]);

  useEffect(() => {
    void loadRole();
  }, [loadRole]);

  return (
    <PageShell
      title="Role Details"
      description="Read-only view of the selected role, including description and audit metadata."
      actions={
        <>
          <Link to="/roles" className={linkButtonClass}>Back to Roles</Link>
          <Link to={`/roles/${roleId}/edit`} className={linkButtonClass}>Edit Role</Link>
        </>
      }
    >
      <FeedbackStack error={error} />
      <ComponentCard title="Role Record" desc="This view uses the Role/View endpoint from the backend.">
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading role details...</p>
        ) : roleRecord ? (
          <DetailGrid
            items={[
              { label: "Role ID", value: roleRecord.roleId },
              { label: "Role Name", value: roleRecord.roleName },
              { label: "Description", value: formatOptionalText(roleRecord.description) },
              { label: "Status", value: <StatusBadge active={roleRecord.isActive} /> },
              { label: "Created By", value: formatOptionalText(roleRecord.createdBy) },
              { label: "Modified By", value: formatOptionalText(roleRecord.modifiedBy) },
              { label: "Created", value: formatDateTime(roleRecord.created) },
              { label: "Modified", value: formatDateTime(roleRecord.modified) },
            ]}
          />
        ) : (
          <p className="text-sm text-gray-500 dark:text-gray-400">No role details were returned.</p>
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default RoleDetailPage;
