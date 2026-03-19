import { useCallback, useEffect, useState } from "react";
import { Link, useParams } from "react-router";
import { useAuth } from "@/AuthServices/components";
import ComponentCard from "@/components/common/ComponentCard";
import {
  ApprovalStatusBadge,
  CompletionBadge,
  DetailGrid,
  FeedbackStack,
  PageShell,
  StatusBadge,
} from "@/ProjectServices/components";
import {
  formatDateTime,
  formatOptionalText,
  getExceptionFeedback,
  getProjectView,
} from "@/ProjectServices/services";
import type { ExceptionFeedback } from "@/ProjectServices/services";
import type { ProjectView } from "@/ProjectServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const ProjectDetailPage = () => {
  const params = useParams();
  const { hasAnyRole } = useAuth();
  const canManageProjects = hasAnyRole(["Admin", "Faculty"]);
  const projectId = Number(params.projectId ?? "0");
  const [projectRecord, setProjectRecord] = useState<ProjectView | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const loadProject = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      setProjectRecord(await getProjectView(projectId));
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [projectId]);

  useEffect(() => {
    void loadProject();
  }, [loadProject]);

  return (
    <PageShell
      title="Project Details"
      description="Read-only project view resolved from the Project/View endpoint, including approval and audit metadata."
      actions={
        <>
          <Link to="/projects" className={linkButtonClass}>Back to Projects</Link>
          {canManageProjects ? (
            <Link to={`/projects/${projectId}/edit`} className={linkButtonClass}>
              Edit Project
            </Link>
          ) : null}
        </>
      }
    >
      <FeedbackStack error={error} />
      <ComponentCard
        title="Project Record"
        desc="This page maps to the backend Project/View route."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading project details...</p>
        ) : projectRecord ? (
          <DetailGrid
            items={[
              { label: "Project ID", value: projectRecord.projectId },
              { label: "Project Name", value: projectRecord.projectName },
              {
                label: "Description",
                value: formatOptionalText(projectRecord.description),
              },
              {
                label: "Approval Status",
                value: <ApprovalStatusBadge value={projectRecord.isApproved} />,
              },
              {
                label: "Completion",
                value: <CompletionBadge completed={projectRecord.isCompleted} />,
              },
              {
                label: "Record Status",
                value: <StatusBadge active={projectRecord.isActive} />,
              },
              { label: "Created By", value: formatOptionalText(projectRecord.createdBy) },
              {
                label: "Modified By",
                value: formatOptionalText(projectRecord.modifiedBy),
              },
              { label: "Created", value: formatDateTime(projectRecord.created) },
              { label: "Modified", value: formatDateTime(projectRecord.modified) },
            ]}
          />
        ) : (
          <p className="text-sm text-gray-500 dark:text-gray-400">No project details were returned.</p>
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default ProjectDetailPage;
