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
  getProjectGroupByProjectView,
  getProjectOptions,
} from "@/ProjectGroupServices/services";
import type { ExceptionFeedback } from "@/ProjectGroupServices/services";
import type { LookupOption, ProjectGroupByProjectView } from "@/ProjectGroupServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const ProjectGroupByProjectDetailPage = () => {
  const params = useParams();
  const { hasAnyRole } = useAuth();
  const canManageMappings = hasAnyRole(["Admin", "Faculty"]);
  const projectGroupByProjectId = Number(params.projectGroupByProjectId ?? "0");
  const [mappingRecord, setMappingRecord] = useState<ProjectGroupByProjectView | null>(null);
  const [projectOptions, setProjectOptions] = useState<LookupOption[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const projectMap = useMemo(() => buildLookupMap(projectOptions), [projectOptions]);

  const loadMapping = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [mappingView, nextProjectOptions] = await Promise.all([
        getProjectGroupByProjectView(projectGroupByProjectId),
        getProjectOptions(),
      ]);

      setMappingRecord(mappingView);
      setProjectOptions(nextProjectOptions);
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [projectGroupByProjectId]);

  useEffect(() => {
    void loadMapping();
  }, [loadMapping]);

  return (
    <PageShell
      title="Group Project Link Details"
      description="Read-only view of a project-group-to-project mapping, including audit metadata."
      actions={
        <>
          <Link to="/project-group-projects" className={linkButtonClass}>Back to Group Project Links</Link>
          {canManageMappings ? (
            <Link
              to={`/project-group-projects/${projectGroupByProjectId}/edit`}
              className={linkButtonClass}
            >
              Edit Mapping
            </Link>
          ) : null}
        </>
      }
    >
      <FeedbackStack error={error} />
      <ComponentCard
        title="Project Group By Project Record"
        desc="This page maps to the backend ProjectGroupByProject/View route."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading mapping details...</p>
        ) : mappingRecord ? (
          <DetailGrid
            items={[
              { label: "Mapping ID", value: mappingRecord.projectGroupByProjectId },
              { label: "Project Group", value: mappingRecord.projectGroupName },
              {
                label: "Project",
                value: formatOptionalText(
                  projectMap.get(mappingRecord.projectId),
                  `Project #${mappingRecord.projectId}`,
                ),
              },
              {
                label: "Record Status",
                value: <StatusBadge active={mappingRecord.isActive} />,
              },
              {
                label: "Created By",
                value: formatOptionalText(mappingRecord.createdBy),
              },
              {
                label: "Modified By",
                value: formatOptionalText(mappingRecord.modifiedBy),
              },
              { label: "Created", value: formatDateTime(mappingRecord.created) },
              { label: "Modified", value: formatDateTime(mappingRecord.modified) },
            ]}
          />
        ) : (
          <p className="text-sm text-gray-500 dark:text-gray-400">No mapping details were returned.</p>
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default ProjectGroupByProjectDetailPage;
