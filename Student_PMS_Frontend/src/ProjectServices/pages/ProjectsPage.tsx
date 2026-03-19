import { useCallback, useEffect, useMemo, useState } from "react";
import { Link, useLocation } from "react-router";
import { useAuth } from "@/AuthServices/components";
import type { ListResult } from "@/CommonLibrary/types";
import ComponentCard from "@/components/common/ComponentCard";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "@/components/ui/table";
import {
  ApprovalStatusBadge,
  CompletionBadge,
  FeedbackStack,
  PageShell,
  StatusBadge,
} from "@/ProjectServices/components";
import {
  deleteProject,
  getExceptionFeedback,
  getProjectsPage,
} from "@/ProjectServices/services";
import type { ExceptionFeedback } from "@/ProjectServices/services";
import type { PageNotice, ProjectListItem } from "@/ProjectServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const summaryCardClass =
  "rounded-2xl border border-gray-200 bg-white p-5 shadow-theme-xs dark:border-gray-800 dark:bg-white/[0.03]";

const ProjectsPage = () => {
  const location = useLocation();
  const { hasAnyRole } = useAuth();
  const canManageProjects = hasAnyRole(["Admin", "Faculty"]);
  const routeNotice =
    ((location.state as { notice?: PageNotice } | null) ?? null)?.notice ?? null;
  const [directory, setDirectory] = useState<ListResult<ProjectListItem>>({
    totalCount: 0,
    items: [],
  });
  const [isLoading, setIsLoading] = useState(true);
  const [notice, setNotice] = useState<PageNotice | null>(routeNotice);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  useEffect(() => {
    if (routeNotice) {
      setNotice(routeNotice);
    }
  }, [routeNotice]);

  const loadProjects = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      setDirectory(await getProjectsPage());
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadProjects();
  }, [loadProjects]);

  const activeCount = useMemo(
    () => directory.items.filter((projectItem) => projectItem.isActive).length,
    [directory.items],
  );

  const completedCount = useMemo(
    () =>
      directory.items.filter((projectItem) => projectItem.isCompleted).length,
    [directory.items],
  );

  const pendingApprovalCount = useMemo(
    () =>
      directory.items.filter((projectItem) => projectItem.isApproved === null).length,
    [directory.items],
  );

  const handleDelete = async (projectItem: ProjectListItem) => {
    const confirmed = window.confirm(
      `Deactivate ${projectItem.projectName}? The backend marks the record inactive instead of removing it permanently.`,
    );

    if (!confirmed) {
      return;
    }

    try {
      setNotice(null);
      setError(null);
      await deleteProject(projectItem.projectId);
      setNotice({
        variant: "success",
        title: "Project deactivated",
        message: `${projectItem.projectName} has been marked inactive.`,
      });
      await loadProjects();
    } catch (requestError) {
      setNotice(null);
      setError(getExceptionFeedback(requestError));
    }
  };

  return (
    <PageShell
      title="Projects"
      description="Browse project records from the ProjectService backend. All signed-in users can review project details, while Admin and Faculty users can manage the catalogue."
      actions={
        <>
          <button
            type="button"
            className={linkButtonClass}
            onClick={() => {
              void loadProjects();
            }}
          >
            Refresh
          </button>
          {canManageProjects ? (
            <Link to="/projects/new" className={linkButtonClass}>
              Create Project
            </Link>
          ) : null}
        </>
      }
    >
      <FeedbackStack notice={notice} error={error} />

      <section className="grid gap-4 md:grid-cols-3">
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Total Projects</p>
          <p className="mt-2 text-3xl font-semibold text-gray-800 dark:text-white/90">
            {directory.totalCount}
          </p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Active Projects</p>
          <p className="mt-2 text-3xl font-semibold text-success-600 dark:text-success-400">
            {activeCount}
          </p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Completed</p>
          <p className="mt-2 text-3xl font-semibold text-brand-600 dark:text-brand-300">
            {completedCount}
          </p>
          <p className="mt-2 text-xs text-gray-500 dark:text-gray-400">
            Pending approval: {pendingApprovalCount}
          </p>
        </div>
      </section>

      <ComponentCard
        title="Project Directory"
        desc="This page maps directly to the Project/Page endpoint exposed through the API gateway."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading projects...</p>
        ) : directory.items.length === 0 ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">No projects are available yet.</p>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow className="border-b border-gray-200 dark:border-gray-800">
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Project</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Approval</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Progress</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Status</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Actions</TableCell>
                </TableRow>
              </TableHeader>
              <TableBody>
                {directory.items.map((projectItem) => (
                  <TableRow key={projectItem.projectId} className="border-b border-gray-100 dark:border-gray-800">
                    <TableCell className="px-4 py-4 text-sm font-medium text-gray-800 dark:text-white/90">
                      {projectItem.projectName}
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <ApprovalStatusBadge value={projectItem.isApproved} />
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <CompletionBadge completed={projectItem.isCompleted} />
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <StatusBadge active={projectItem.isActive} />
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <div className="flex flex-wrap justify-end gap-2">
                        <Link to={`/projects/${projectItem.projectId}`} className={linkButtonClass}>
                          View
                        </Link>
                        {canManageProjects ? (
                          <Link
                            to={`/projects/${projectItem.projectId}/edit`}
                            className={linkButtonClass}
                          >
                            Edit
                          </Link>
                        ) : null}
                        {canManageProjects && projectItem.isActive ? (
                          <button
                            type="button"
                            className={linkButtonClass}
                            onClick={() => {
                              void handleDelete(projectItem);
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

export default ProjectsPage;
