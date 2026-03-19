import { useCallback, useEffect, useState } from "react";
import { Link, useParams } from "react-router";
import { useAuth } from "@/AuthServices/components";
import ComponentCard from "@/components/common/ComponentCard";
import {
  DetailGrid,
  FeedbackStack,
  PageShell,
  ProjectGroupApprovalStatusBadge,
  StatusBadge,
} from "@/ProjectGroupServices/components";
import {
  formatDateTime,
  formatOptionalText,
  getExceptionFeedback,
  getProjectGroupView,
} from "@/ProjectGroupServices/services";
import type { ExceptionFeedback } from "@/ProjectGroupServices/services";
import type { ProjectGroupView } from "@/ProjectGroupServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const ProjectGroupDetailPage = () => {
  const params = useParams();
  const { hasAnyRole } = useAuth();
  const canManageProjectGroups = hasAnyRole(["Admin", "Faculty"]);
  const projectGroupId = Number(params.projectGroupId ?? "0");
  const [projectGroupRecord, setProjectGroupRecord] = useState<ProjectGroupView | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const loadProjectGroup = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      setProjectGroupRecord(await getProjectGroupView(projectGroupId));
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [projectGroupId]);

  useEffect(() => {
    void loadProjectGroup();
  }, [loadProjectGroup]);

  return (
    <PageShell
      title="Project Group Details"
      description="Read-only project group view resolved from the ProjectGroup/View endpoint, including approval and audit metadata."
      actions={
        <>
          <Link to="/project-groups" className={linkButtonClass}>Back to Project Groups</Link>
          {canManageProjectGroups ? (
            <Link to={`/project-groups/${projectGroupId}/edit`} className={linkButtonClass}>
              Edit Project Group
            </Link>
          ) : null}
        </>
      }
    >
      <FeedbackStack error={error} />
      <ComponentCard
        title="Project Group Record"
        desc="This page maps to the backend ProjectGroup/View route."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading project group details...</p>
        ) : projectGroupRecord ? (
          <DetailGrid
            items={[
              { label: "Project Group ID", value: projectGroupRecord.projectGroupId },
              { label: "Project Group Name", value: projectGroupRecord.projectGroupName },
              {
                label: "Approval Status",
                value: (
                  <ProjectGroupApprovalStatusBadge
                    value={projectGroupRecord.approvalStatusString}
                  />
                ),
              },
              {
                label: "Approved By",
                value: formatOptionalText(projectGroupRecord.approvedBy),
              },
              {
                label: "Record Status",
                value: <StatusBadge active={projectGroupRecord.isActive} />,
              },
              {
                label: "Created By",
                value: formatOptionalText(projectGroupRecord.createdBy),
              },
              {
                label: "Modified By",
                value: formatOptionalText(projectGroupRecord.modifiedBy),
              },
              { label: "Created", value: formatDateTime(projectGroupRecord.created) },
              { label: "Modified", value: formatDateTime(projectGroupRecord.modified) },
            ]}
          />
        ) : (
          <p className="text-sm text-gray-500 dark:text-gray-400">No project group details were returned.</p>
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default ProjectGroupDetailPage;
