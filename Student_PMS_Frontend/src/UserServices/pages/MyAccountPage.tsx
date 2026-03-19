import { useCallback, useEffect, useState } from "react";
import { useAuth } from "@/AuthServices/components";
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

const MyAccountPage = () => {
  const { user } = useAuth();
  const [userRecord, setUserRecord] = useState<UserView | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const loadAccount = useCallback(async () => {
    if (!user?.userId) {
      setError({
        message: "The current session does not expose a user identifier. Account data cannot be resolved.",
        detailLines: [],
      });
      setIsLoading(false);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      setUserRecord(await getUserView(user.userId));
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [user?.userId]);

  useEffect(() => {
    void loadAccount();
  }, [loadAccount]);

  return (
    <PageShell
      title="My Account"
      description="Read-only account view for the currently authenticated user. Admin users can manage records from the directory pages, while faculty and students are limited to this summary."
    >
      <FeedbackStack error={error} />

      <ComponentCard title="Account Summary" desc="User information resolved from the User/View endpoint.">
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading account details...</p>
        ) : userRecord ? (
          <DetailGrid
            items={[
              { label: "User ID", value: userRecord.userId },
              { label: "Name", value: userRecord.name },
              { label: "Email", value: userRecord.email },
              { label: "Role", value: formatOptionalText(userRecord.roleName) },
              { label: "Status", value: <StatusBadge active={userRecord.isActive} /> },
              { label: "Created", value: formatDateTime(userRecord.created) },
              { label: "Modified", value: formatDateTime(userRecord.modified) },
              { label: "Created By", value: formatOptionalText(userRecord.createdBy) },
              { label: "Modified By", value: formatOptionalText(userRecord.modifiedBy) },
            ]}
          />
        ) : (
          <p className="text-sm text-gray-500 dark:text-gray-400">No account details were returned.</p>
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default MyAccountPage;
