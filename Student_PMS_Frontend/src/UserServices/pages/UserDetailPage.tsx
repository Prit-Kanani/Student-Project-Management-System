import { useCallback, useEffect, useState } from "react";
import { Link, useParams } from "react-router";
import ComponentCard from "@/components/common/ComponentCard";
import { DetailGrid, FeedbackStack, PageShell, StatusBadge } from "@/UserServices/components";
import {
  formatDateTime,
  formatOptionalText,
  getExceptionFeedback,
  getUserView,
} from "@/UserServices/services";
import type { ExceptionFeedback } from "@/UserServices/services";
import type { UserView } from "@/UserServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const UserDetailPage = () => {
  const params = useParams();
  const userId = Number(params.userId ?? "0");
  const [userRecord, setUserRecord] = useState<UserView | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const loadUser = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      setUserRecord(await getUserView(userId));
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [userId]);

  useEffect(() => {
    void loadUser();
  }, [loadUser]);

  return (
    <PageShell
      title="User Details"
      description="Read-only view of the selected user record, including role and audit metadata returned by the backend."
      actions={
        <>
          <Link to="/users" className={linkButtonClass}>Back to Users</Link>
          <Link to={`/users/${userId}/edit`} className={linkButtonClass}>Edit User</Link>
        </>
      }
    >
      <FeedbackStack error={error} />
      <ComponentCard title="User Record" desc="This view uses the View endpoint from the User controller.">
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading user details...</p>
        ) : userRecord ? (
          <DetailGrid
            items={[
              { label: "User ID", value: userRecord.userId },
              { label: "Name", value: userRecord.name },
              { label: "Email", value: userRecord.email },
              { label: "Role", value: formatOptionalText(userRecord.roleName) },
              { label: "Status", value: <StatusBadge active={userRecord.isActive} /> },
              { label: "Created By", value: formatOptionalText(userRecord.createdBy) },
              { label: "Modified By", value: formatOptionalText(userRecord.modifiedBy) },
              { label: "Created", value: formatDateTime(userRecord.created) },
              { label: "Modified", value: formatDateTime(userRecord.modified) },
            ]}
          />
        ) : (
          <p className="text-sm text-gray-500 dark:text-gray-400">No user details were returned.</p>
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default UserDetailPage;
