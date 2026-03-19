import { useCallback, useEffect, useMemo, useState } from "react";
import { Link, useLocation } from "react-router";
import { useAuth } from "@/AuthServices/components";
import type { ListResult } from "@/CommonLibrary/types";
import ComponentCard from "@/components/common/ComponentCard";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "@/components/ui/table";
import {
  FeedbackStack,
  PageShell,
  ProjectGroupApprovalStatusBadge,
  StatusBadge,
} from "@/ProjectGroupServices/components";
import {
  deactivateProjectGroup,
  getExceptionFeedback,
  getProjectGroupsPage,
} from "@/ProjectGroupServices/services";
import type { ExceptionFeedback } from "@/ProjectGroupServices/services";
import type { PageNotice, ProjectGroupListItem } from "@/ProjectGroupServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const summaryCardClass =
  "rounded-2xl border border-gray-200 bg-white p-5 shadow-theme-xs dark:border-gray-800 dark:bg-white/[0.03]";

const ProjectGroupsPage = () => {
  const location = useLocation();
  const { hasAnyRole } = useAuth();
  const canManageProjectGroups = hasAnyRole(["Admin", "Faculty"]);
  const routeNotice =
    ((location.state as { notice?: PageNotice } | null) ?? null)?.notice ?? null;
  const [directory, setDirectory] = useState<ListResult<ProjectGroupListItem>>({
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

  const loadProjectGroups = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      setDirectory(await getProjectGroupsPage());
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadProjectGroups();
  }, [loadProjectGroups]);

  const activeCount = useMemo(
    () => directory.items.filter((item) => item.isActive).length,
    [directory.items],
  );

  const approvedCount = useMemo(
    () => directory.items.filter((item) => item.isApproved === true).length,
    [directory.items],
  );

  const pendingCount = useMemo(
    () => directory.items.filter((item) => item.isApproved === null).length,
    [directory.items],
  );

  const handleDeactivate = async (projectGroup: ProjectGroupListItem) => {
    const confirmed = window.confirm(
      `Deactivate ${projectGroup.projectGroupName}? The backend marks the project group inactive instead of deleting it permanently.`,
    );

    if (!confirmed) {
      return;
    }

    try {
      setNotice(null);
      setError(null);
      await deactivateProjectGroup(projectGroup.projectGroupId);
      setNotice({
        variant: "success",
        title: "Project group deactivated",
        message: `${projectGroup.projectGroupName} has been marked inactive.`,
      });
      await loadProjectGroups();
    } catch (requestError) {
      setNotice(null);
      setError(getExceptionFeedback(requestError));
    }
  };

  return (
    <PageShell
      title="Project Groups"
      description="Manage the core group catalogue exposed by the ProjectGroupService backend. All signed-in users can review group details, while Admin and Faculty users can create, update, and deactivate records."
      actions={
        <>
          <button
            type="button"
            className={linkButtonClass}
            onClick={() => {
              void loadProjectGroups();
            }}
          >
            Refresh
          </button>
          {canManageProjectGroups ? (
            <Link to="/project-groups/new" className={linkButtonClass}>
              Create Project Group
            </Link>
          ) : null}
        </>
      }
    >
      <FeedbackStack notice={notice} error={error} />

      <section className="grid gap-4 md:grid-cols-3">
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Total Groups</p>
          <p className="mt-2 text-3xl font-semibold text-gray-800 dark:text-white/90">
            {directory.totalCount}
          </p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Active Groups</p>
          <p className="mt-2 text-3xl font-semibold text-success-600 dark:text-success-400">
            {activeCount}
          </p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Approved</p>
          <p className="mt-2 text-3xl font-semibold text-brand-600 dark:text-brand-300">
            {approvedCount}
          </p>
          <p className="mt-2 text-xs text-gray-500 dark:text-gray-400">
            Pending review: {pendingCount}
          </p>
        </div>
      </section>

      <ComponentCard
        title="Project Group Directory"
        desc="This page maps directly to the ProjectGroup/Page endpoint exposed through the API gateway."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading project groups...</p>
        ) : directory.items.length === 0 ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">No project groups are available yet.</p>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow className="border-b border-gray-200 dark:border-gray-800">
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Project Group</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Approval</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Status</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Actions</TableCell>
                </TableRow>
              </TableHeader>
              <TableBody>
                {directory.items.map((item) => (
                  <TableRow key={item.projectGroupId} className="border-b border-gray-100 dark:border-gray-800">
                    <TableCell className="px-4 py-4 text-sm font-medium text-gray-800 dark:text-white/90">
                      {item.projectGroupName}
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <ProjectGroupApprovalStatusBadge value={item.isApproved} />
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <StatusBadge active={item.isActive} />
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <div className="flex flex-wrap justify-end gap-2">
                        <Link to={`/project-groups/${item.projectGroupId}`} className={linkButtonClass}>
                          View
                        </Link>
                        {canManageProjectGroups ? (
                          <Link
                            to={`/project-groups/${item.projectGroupId}/edit`}
                            className={linkButtonClass}
                          >
                            Edit
                          </Link>
                        ) : null}
                        {canManageProjectGroups && item.isActive ? (
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

export default ProjectGroupsPage;
