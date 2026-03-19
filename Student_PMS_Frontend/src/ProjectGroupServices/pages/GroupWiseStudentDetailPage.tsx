import { useCallback, useEffect, useMemo, useState } from "react";
import { Link, useParams } from "react-router";
import { useAuth } from "@/AuthServices/components";
import ComponentCard from "@/components/common/ComponentCard";
import {
  DetailGrid,
  FeedbackStack,
  PageShell,
  StatusBadge,
} from "@/ProjectGroupServices/components";
import {
  buildLookupMap,
  formatDateTime,
  formatOptionalText,
  getExceptionFeedback,
  getGroupWiseStudentView,
  getProjectGroupOptions,
  getUserOptions,
} from "@/ProjectGroupServices/services";
import type { ExceptionFeedback } from "@/ProjectGroupServices/services";
import type { GroupWiseStudentView, LookupOption } from "@/ProjectGroupServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const GroupWiseStudentDetailPage = () => {
  const params = useParams();
  const { hasAnyRole } = useAuth();
  const canManageAssignments = hasAnyRole(["Admin", "Faculty"]);
  const studentWiseGroupId = Number(params.groupWiseStudentId ?? "0");
  const [assignmentRecord, setAssignmentRecord] = useState<GroupWiseStudentView | null>(null);
  const [projectGroupOptions, setProjectGroupOptions] = useState<LookupOption[]>([]);
  const [userOptions, setUserOptions] = useState<LookupOption[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const projectGroupMap = useMemo(
    () => buildLookupMap(projectGroupOptions),
    [projectGroupOptions],
  );
  const userMap = useMemo(() => buildLookupMap(userOptions), [userOptions]);

  const loadAssignment = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [assignmentView, nextProjectGroupOptions, nextUserOptions] = await Promise.all([
        getGroupWiseStudentView(studentWiseGroupId),
        getProjectGroupOptions(),
        getUserOptions(),
      ]);

      setAssignmentRecord(assignmentView);
      setProjectGroupOptions(nextProjectGroupOptions);
      setUserOptions(nextUserOptions);
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [studentWiseGroupId]);

  useEffect(() => {
    void loadAssignment();
  }, [loadAssignment]);

  return (
    <PageShell
      title="Student Assignment Details"
      description="Read-only view of a group-wise student record, including audit metadata."
      actions={
        <>
          <Link to="/group-wise-students" className={linkButtonClass}>Back to Group Wise Students</Link>
          {canManageAssignments ? (
            <Link
              to={`/group-wise-students/${studentWiseGroupId}/edit`}
              className={linkButtonClass}
            >
              Edit Assignment
            </Link>
          ) : null}
        </>
      }
    >
      <FeedbackStack error={error} />
      <ComponentCard
        title="Group Wise Student Record"
        desc="This page maps to the backend GroupWiseStudent/View route."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading student assignment details...</p>
        ) : assignmentRecord ? (
          <DetailGrid
            items={[
              { label: "Assignment ID", value: assignmentRecord.studentWiseGroupId },
              {
                label: "Project Group",
                value: formatOptionalText(
                  projectGroupMap.get(assignmentRecord.projectGroupId),
                  `Project Group #${assignmentRecord.projectGroupId}`,
                ),
              },
              {
                label: "Student",
                value: formatOptionalText(
                  userMap.get(assignmentRecord.studentId),
                  `Student #${assignmentRecord.studentId}`,
                ),
              },
              {
                label: "Record Status",
                value: <StatusBadge active={assignmentRecord.isActive} />,
              },
              {
                label: "Created By",
                value: formatOptionalText(assignmentRecord.createdBy),
              },
              {
                label: "Modified By",
                value: formatOptionalText(assignmentRecord.modifiedBy),
              },
              { label: "Created", value: formatDateTime(assignmentRecord.created) },
              { label: "Modified", value: formatDateTime(assignmentRecord.modified) },
            ]}
          />
        ) : (
          <p className="text-sm text-gray-500 dark:text-gray-400">No assignment details were returned.</p>
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default GroupWiseStudentDetailPage;
