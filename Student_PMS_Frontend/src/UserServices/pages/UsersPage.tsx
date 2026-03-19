import { useCallback, useEffect, useMemo, useState } from "react";
import { Link, useLocation } from "react-router";
import type { ListResult } from "@/CommonLibrary/types";
import ComponentCard from "@/components/common/ComponentCard";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "@/components/ui/table";
import { FeedbackStack, PageShell, StatusBadge } from "@/UserServices/components";
import { deactivateUser, getExceptionFeedback, getUsersPage } from "@/UserServices/services";
import type { ExceptionFeedback } from "@/UserServices/services";
import type { PageNotice, UserListItem } from "@/UserServices/types";

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const summaryCardClass =
  "rounded-2xl border border-gray-200 bg-white p-5 shadow-theme-xs dark:border-gray-800 dark:bg-white/[0.03]";

const UsersPage = () => {
  const location = useLocation();
  const routeNotice = ((location.state as { notice?: PageNotice } | null) ?? null)?.notice ?? null;
  const [directory, setDirectory] = useState<ListResult<UserListItem>>({
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

  const loadUsers = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      setDirectory(await getUsersPage());
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadUsers();
  }, [loadUsers]);

  const activeCount = useMemo(
    () => directory.items.filter((userItem) => userItem.isActive).length,
    [directory.items],
  );

  const inactiveCount = directory.totalCount - activeCount;

  const handleDeactivate = async (userItem: UserListItem) => {
    const confirmed = window.confirm(
      `Deactivate ${userItem.name}'s account? This only disables access; it does not delete the record.`,
    );

    if (!confirmed) {
      return;
    }

    try {
      setNotice(null);
      setError(null);
      await deactivateUser(userItem.userId);
      setNotice({
        variant: "success",
        title: "User deactivated",
        message: `${userItem.name} has been marked inactive.`,
      });
      await loadUsers();
    } catch (requestError) {
      setNotice(null);
      setError(getExceptionFeedback(requestError));
    }
  };

  return (
    <PageShell
      title="Users"
      description="Admin-only user directory with create, edit, view, and deactivate operations mapped directly to the backend User controller."
      actions={
        <>
          <button
            type="button"
            className={linkButtonClass}
            onClick={() => {
              void loadUsers();
            }}
          >
            Refresh
          </button>
          <Link to="/users/new" className={linkButtonClass}>
            Create User
          </Link>
        </>
      }
    >
      <FeedbackStack notice={notice} error={error} />

      <section className="grid gap-4 md:grid-cols-3">
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Total Users</p>
          <p className="mt-2 text-3xl font-semibold text-gray-800 dark:text-white/90">{directory.totalCount}</p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Active Users</p>
          <p className="mt-2 text-3xl font-semibold text-success-600 dark:text-success-400">{activeCount}</p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Inactive Users</p>
          <p className="mt-2 text-3xl font-semibold text-warning-600 dark:text-warning-400">{inactiveCount}</p>
        </div>
      </section>

      <ComponentCard
        title="Directory"
        desc="Create and manage user accounts. Faculty and students only receive read-only access through their account view."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading users...</p>
        ) : directory.items.length === 0 ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">No users are available yet.</p>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow className="border-b border-gray-200 dark:border-gray-800">
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Name</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Email</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Status</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Actions</TableCell>
                </TableRow>
              </TableHeader>
              <TableBody>
                {directory.items.map((userItem) => (
                  <TableRow key={userItem.userId} className="border-b border-gray-100 dark:border-gray-800">
                    <TableCell className="px-4 py-4 text-sm font-medium text-gray-800 dark:text-white/90">{userItem.name}</TableCell>
                    <TableCell className="px-4 py-4 text-sm text-gray-500 dark:text-gray-400">{userItem.email}</TableCell>
                    <TableCell className="px-4 py-4">
                      <StatusBadge active={userItem.isActive} />
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <div className="flex flex-wrap justify-end gap-2">
                        <Link to={`/users/${userItem.userId}`} className={linkButtonClass}>View</Link>
                        <Link to={`/users/${userItem.userId}/edit`} className={linkButtonClass}>Edit</Link>
                        {userItem.isActive ? (
                          <button
                            type="button"
                            className={linkButtonClass}
                            onClick={() => {
                              void handleDeactivate(userItem);
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

export default UsersPage;
