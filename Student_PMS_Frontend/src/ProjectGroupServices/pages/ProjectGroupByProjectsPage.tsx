import { useCallback, useEffect, useMemo, useState } from "react";
import { Link, useLocation } from "react-router";
import { useAuth } from "@/AuthServices/components";
import type { ListResult } from "@/CommonLibrary/types";
import ComponentCard from "@/components/common/ComponentCard";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "@/components/ui/table";
import {
  FeedbackStack,
  PageShell,
  StatusBadge,
} from "@/ProjectGroupServices/components";
import {
  buildLookupMap,
  deactivateProjectGroupByProject,
  formatOptionalText,
  getExceptionFeedback,
  getProjectGroupByProjectsPage,
  getProjectOptions,
} from "@/ProjectGroupServices/services";
import type { ExceptionFeedback } from "@/ProjectGroupServices/services";
import type {
  LookupOption,
  PageNotice,
  ProjectGroupByProjectListItem,
} from "@/ProjectGroupServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const summaryCardClass =
  "rounded-2xl border border-gray-200 bg-white p-5 shadow-theme-xs dark:border-gray-800 dark:bg-white/[0.03]";

const ProjectGroupByProjectsPage = () => {
  const location = useLocation();
  const { hasAnyRole } = useAuth();
  const canManageMappings = hasAnyRole(["Admin", "Faculty"]);
  const routeNotice =
    ((location.state as { notice?: PageNotice } | null) ?? null)?.notice ?? null;
  const [directory, setDirectory] = useState<ListResult<ProjectGroupByProjectListItem>>({
    totalCount: 0,
    items: [],
  });
  const [projectOptions, setProjectOptions] = useState<LookupOption[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [notice, setNotice] = useState<PageNotice | null>(routeNotice);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const projectMap = useMemo(() => buildLookupMap(projectOptions), [projectOptions]);

  useEffect(() => {
    if (routeNotice) {
      setNotice(routeNotice);
    }
  }, [routeNotice]);

  const loadMappings = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [mappingPage, nextProjectOptions] = await Promise.all([
        getProjectGroupByProjectsPage(),
        getProjectOptions(),
      ]);

      setDirectory(mappingPage);
      setProjectOptions(nextProjectOptions);
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadMappings();
  }, [loadMappings]);

  const activeCount = useMemo(
    () => directory.items.filter((item) => item.isActive).length,
    [directory.items],
  );

  const uniqueProjectCount = useMemo(
    () => new Set(directory.items.map((item) => item.projectId)).size,
    [directory.items],
  );

  const uniqueProjectGroupCount = useMemo(
    () => new Set(directory.items.map((item) => item.projectGroupName)).size,
    [directory.items],
  );

  const handleDeactivate = async (mapping: ProjectGroupByProjectListItem) => {
    const confirmed = window.confirm(
      `Deactivate the mapping between ${mapping.projectGroupName} and project #${mapping.projectId}?`,
    );

    if (!confirmed) {
      return;
    }

    try {
      setNotice(null);
      setError(null);
      await deactivateProjectGroupByProject(mapping.projectGroupByProjectId);
      setNotice({
        variant: "success",
        title: "Mapping deactivated",
        message: "The group-to-project mapping has been marked inactive.",
      });
      await loadMappings();
    } catch (requestError) {
      setNotice(null);
      setError(getExceptionFeedback(requestError));
    }
  };

  return (
    <PageShell
      title="Group Project Links"
      description="Manage which project group is attached to which project. This model maps directly to the ProjectGroupByProject endpoints in ProjectGroupService."
      actions={
        <>
          <button
            type="button"
            className={linkButtonClass}
            onClick={() => {
              void loadMappings();
            }}
          >
            Refresh
          </button>
          {canManageMappings ? (
            <Link to="/project-group-projects/new" className={linkButtonClass}>
              Create Mapping
            </Link>
          ) : null}
        </>
      }
    >
      <FeedbackStack notice={notice} error={error} />

      <section className="grid gap-4 md:grid-cols-3">
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Total Links</p>
          <p className="mt-2 text-3xl font-semibold text-gray-800 dark:text-white/90">
            {directory.totalCount}
          </p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Active Links</p>
          <p className="mt-2 text-3xl font-semibold text-success-600 dark:text-success-400">
            {activeCount}
          </p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Coverage</p>
          <p className="mt-2 text-3xl font-semibold text-brand-600 dark:text-brand-300">
            {uniqueProjectCount}
          </p>
          <p className="mt-2 text-xs text-gray-500 dark:text-gray-400">
            Groups represented: {uniqueProjectGroupCount}
          </p>
        </div>
      </section>

      <ComponentCard
        title="Project Group By Project Directory"
        desc="This page maps directly to the ProjectGroupByProject/Page endpoint."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading project links...</p>
        ) : directory.items.length === 0 ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">No project links are available yet.</p>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow className="border-b border-gray-200 dark:border-gray-800">
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Project Group</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Project</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Status</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Actions</TableCell>
                </TableRow>
              </TableHeader>
              <TableBody>
                {directory.items.map((item) => (
                  <TableRow key={item.projectGroupByProjectId} className="border-b border-gray-100 dark:border-gray-800">
                    <TableCell className="px-4 py-4 text-sm font-medium text-gray-800 dark:text-white/90">
                      {item.projectGroupName}
                    </TableCell>
                    <TableCell className="px-4 py-4 text-sm text-gray-600 dark:text-gray-300">
                      {formatOptionalText(projectMap.get(item.projectId), `Project #${item.projectId}`)}
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <StatusBadge active={item.isActive} />
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <div className="flex flex-wrap justify-end gap-2">
                        <Link
                          to={`/project-group-projects/${item.projectGroupByProjectId}`}
                          className={linkButtonClass}
                        >
                          View
                        </Link>
                        {canManageMappings ? (
                          <Link
                            to={`/project-group-projects/${item.projectGroupByProjectId}/edit`}
                            className={linkButtonClass}
                          >
                            Edit
                          </Link>
                        ) : null}
                        {canManageMappings && item.isActive ? (
                          <button
                            type="button"
                            className={linkButtonClass}
                            onClick={() => {
                              void handleDeactivate(item);
                            }}
                          >
                            Deactivate
                          </button>
                        ) : null}
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default ProjectGroupByProjectsPage;
