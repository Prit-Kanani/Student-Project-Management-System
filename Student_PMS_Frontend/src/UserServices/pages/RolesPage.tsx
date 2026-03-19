import { useCallback, useEffect, useMemo, useState } from "react";
import { Link, useLocation } from "react-router";
import type { ListResult } from "@/CommonLibrary/types";
import ComponentCard from "@/components/common/ComponentCard";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "@/components/ui/table";
import { FeedbackStack, PageShell, StatusBadge } from "@/UserServices/components";
import { deactivateRole, getExceptionFeedback, getRolesPage } from "@/UserServices/services";
import type { ExceptionFeedback } from "@/UserServices/services";
import type { PageNotice, RoleListItem } from "@/UserServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const summaryCardClass =
  "rounded-2xl border border-gray-200 bg-white p-5 shadow-theme-xs dark:border-gray-800 dark:bg-white/[0.03]";

const RolesPage = () => {
  const location = useLocation();
  const routeNotice = ((location.state as { notice?: PageNotice } | null) ?? null)?.notice ?? null;
  const [directory, setDirectory] = useState<ListResult<RoleListItem>>({ totalCount: 0, items: [] });
  const [isLoading, setIsLoading] = useState(true);
  const [notice, setNotice] = useState<PageNotice | null>(routeNotice);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  useEffect(() => {
    if (routeNotice) {
      setNotice(routeNotice);
    }
  }, [routeNotice]);

  const loadRoles = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      setDirectory(await getRolesPage());
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadRoles();
  }, [loadRoles]);

  const activeCount = useMemo(
    () => directory.items.filter((roleItem) => roleItem.isActive).length,
    [directory.items],
  );

  const inactiveCount = directory.totalCount - activeCount;

  const handleDeactivate = async (roleItem: RoleListItem) => {
    const confirmed = window.confirm(
      `Deactivate the ${roleItem.roleName} role? Existing records remain, but new assignments should stop using it.`,
    );

    if (!confirmed) {
      return;
    }

    try {
      setNotice(null);
      setError(null);
      await deactivateRole(roleItem.roleId);
      setNotice({
        variant: "success",
        title: "Role deactivated",
        message: `${roleItem.roleName} has been marked inactive.`,
      });
      await loadRoles();
    } catch (requestError) {
      setNotice(null);
      setError(getExceptionFeedback(requestError));
    }
  };

  return (
    <PageShell
      title="Roles"
      description="Admin-only role catalogue that drives access control across the Student PMS frontend and backend."
      actions={
        <>
          <button
            type="button"
            className={linkButtonClass}
            onClick={() => {
              void loadRoles();
            }}
          >
            Refresh
          </button>
          <Link to="/roles/new" className={linkButtonClass}>Create Role</Link>
        </>
      }
    >
      <FeedbackStack notice={notice} error={error} />

      <section className="grid gap-4 md:grid-cols-3">
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Total Roles</p>
          <p className="mt-2 text-3xl font-semibold text-gray-800 dark:text-white/90">{directory.totalCount}</p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Active Roles</p>
          <p className="mt-2 text-3xl font-semibold text-success-600 dark:text-success-400">{activeCount}</p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Inactive Roles</p>
          <p className="mt-2 text-3xl font-semibold text-warning-600 dark:text-warning-400">{inactiveCount}</p>
        </div>
      </section>

      <ComponentCard title="Role Directory" desc="Use this page to manage role definitions and availability.">
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading roles...</p>
        ) : directory.items.length === 0 ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">No roles are available yet.</p>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow className="border-b border-gray-200 dark:border-gray-800">
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Role</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Status</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Actions</TableCell>
                </TableRow>
              </TableHeader>
              <TableBody>
                {directory.items.map((roleItem) => (
                  <TableRow key={roleItem.roleId} className="border-b border-gray-100 dark:border-gray-800">
                    <TableCell className="px-4 py-4 text-sm font-medium text-gray-800 dark:text-white/90">{roleItem.roleName}</TableCell>
                    <TableCell className="px-4 py-4">
                      <StatusBadge active={roleItem.isActive} />
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <div className="flex flex-wrap justify-end gap-2">
                        <Link to={`/roles/${roleItem.roleId}`} className={linkButtonClass}>View</Link>
                        <Link to={`/roles/${roleItem.roleId}/edit`} className={linkButtonClass}>Edit</Link>
                        {roleItem.isActive ? (
                          <button
                            type="button"
                            className={linkButtonClass}
                            onClick={() => {
                              void handleDeactivate(roleItem);
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

export default RolesPage;
