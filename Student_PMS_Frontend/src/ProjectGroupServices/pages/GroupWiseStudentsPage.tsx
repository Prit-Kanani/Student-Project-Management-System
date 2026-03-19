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
  deactivateGroupWiseStudent,
  formatOptionalText,
  getExceptionFeedback,
  getGroupWiseStudentPrimaryKey,
  getGroupWiseStudentsPage,
  getProjectGroupOptions,
  getUserOptions,
} from "@/ProjectGroupServices/services";
import type { ExceptionFeedback } from "@/ProjectGroupServices/services";
import type {
  GroupWiseStudentListItem,
  LookupOption,
  PageNotice,
} from "@/ProjectGroupServices/types";

type GroupWiseStudentDirectoryItem = GroupWiseStudentListItem & {
  studentId: number | null;
};

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const summaryCardClass =
  "rounded-2xl border border-gray-200 bg-white p-5 shadow-theme-xs dark:border-gray-800 dark:bg-white/[0.03]";

const GroupWiseStudentsPage = () => {
  const location = useLocation();
  const { hasAnyRole } = useAuth();
  const canManageAssignments = hasAnyRole(["Admin", "Faculty"]);
  const routeNotice =
    ((location.state as { notice?: PageNotice } | null) ?? null)?.notice ?? null;
  const [directory, setDirectory] = useState<ListResult<GroupWiseStudentDirectoryItem>>({
    totalCount: 0,
    items: [],
  });
  const [projectGroupOptions, setProjectGroupOptions] = useState<LookupOption[]>([]);
  const [userOptions, setUserOptions] = useState<LookupOption[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [notice, setNotice] = useState<PageNotice | null>(routeNotice);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const projectGroupMap = useMemo(
    () => buildLookupMap(projectGroupOptions),
    [projectGroupOptions],
  );
  const userMap = useMemo(() => buildLookupMap(userOptions), [userOptions]);

  useEffect(() => {
    if (routeNotice) {
      setNotice(routeNotice);
    }
  }, [routeNotice]);

  const loadAssignments = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [assignmentPage, nextProjectGroupOptions, nextUserOptions] = await Promise.all([
        getGroupWiseStudentsPage(0, 100),
        getProjectGroupOptions(),
        getUserOptions(),
      ]);

      const enrichedItems = await Promise.all(
        assignmentPage.items.map(async (item) => {
          try {
            const primaryKey = await getGroupWiseStudentPrimaryKey(item.studentWiseGroupId);

            return {
              ...item,
              studentId: primaryKey.studentId,
            };
          } catch {
            return {
              ...item,
              studentId: null,
            };
          }
        }),
      );

      setDirectory({
        totalCount: assignmentPage.totalCount,
        items: enrichedItems,
      });
      setProjectGroupOptions(nextProjectGroupOptions);
      setUserOptions(nextUserOptions);
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, []);

  useEffect(() => {
    void loadAssignments();
  }, [loadAssignments]);

  const activeCount = useMemo(
    () => directory.items.filter((item) => item.isActive).length,
    [directory.items],
  );

  const uniqueGroupCount = useMemo(
    () => new Set(directory.items.map((item) => item.projectGroupId)).size,
    [directory.items],
  );

  const linkedStudentCount = useMemo(
    () =>
      new Set(
        directory.items
          .map((item) => item.studentId)
          .filter((studentId): studentId is number => typeof studentId === "number" && studentId > 0),
      ).size,
    [directory.items],
  );

  const handleDeactivate = async (assignment: GroupWiseStudentDirectoryItem) => {
    const confirmed = window.confirm(
      `Deactivate student assignment #${assignment.studentWiseGroupId}?`,
    );

    if (!confirmed) {
      return;
    }

    try {
      setNotice(null);
      setError(null);
      await deactivateGroupWiseStudent(assignment.studentWiseGroupId);
      setNotice({
        variant: "success",
        title: "Assignment deactivated",
        message: "The group-wise student assignment has been marked inactive.",
      });
      await loadAssignments();
    } catch (requestError) {
      setNotice(null);
      setError(getExceptionFeedback(requestError));
    }
  };

  return (
    <PageShell
      title="Group Wise Students"
      description="Manage which student record is attached to which project group. This model maps directly to the GroupWiseStudent endpoints in ProjectGroupService."
      actions={
        <>
          <button
            type="button"
            className={linkButtonClass}
            onClick={() => {
              void loadAssignments();
            }}
          >
            Refresh
          </button>
          {canManageAssignments ? (
            <Link to="/group-wise-students/new" className={linkButtonClass}>
              Create Assignment
            </Link>
          ) : null}
        </>
      }
    >
      <FeedbackStack notice={notice} error={error} />

      <section className="grid gap-4 md:grid-cols-3">
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Loaded Assignments</p>
          <p className="mt-2 text-3xl font-semibold text-gray-800 dark:text-white/90">
            {directory.totalCount}
          </p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Active Assignments</p>
          <p className="mt-2 text-3xl font-semibold text-success-600 dark:text-success-400">
            {activeCount}
          </p>
        </div>
        <div className={summaryCardClass}>
          <p className="text-sm text-gray-500 dark:text-gray-400">Coverage</p>
          <p className="mt-2 text-3xl font-semibold text-brand-600 dark:text-brand-300">
            {linkedStudentCount}
          </p>
          <p className="mt-2 text-xs text-gray-500 dark:text-gray-400">
            Groups represented: {uniqueGroupCount}
          </p>
        </div>
      </section>

      <ComponentCard
        title="Group Wise Student Directory"
        desc="The backend page endpoint supports skip and take. This screen loads the first 100 records for the assignment directory."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading student assignments...</p>
        ) : directory.items.length === 0 ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">No student assignments are available yet.</p>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow className="border-b border-gray-200 dark:border-gray-800">
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Project Group</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Student</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Status</TableCell>
                  <TableCell isHeader className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-[0.16em] text-gray-500">Actions</TableCell>
                </TableRow>
              </TableHeader>
              <TableBody>
                {directory.items.map((item) => (
                  <TableRow key={item.studentWiseGroupId} className="border-b border-gray-100 dark:border-gray-800">
                    <TableCell className="px-4 py-4 text-sm font-medium text-gray-800 dark:text-white/90">
                      {formatOptionalText(
                        projectGroupMap.get(item.projectGroupId),
                        `Project Group #${item.projectGroupId}`,
                      )}
                    </TableCell>
                    <TableCell className="px-4 py-4 text-sm text-gray-600 dark:text-gray-300">
                      {item.studentId
                        ? formatOptionalText(
                            userMap.get(item.studentId),
                            `Student #${item.studentId}`,
                          )
                        : "Not resolved"}
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <StatusBadge active={item.isActive} />
                    </TableCell>
                    <TableCell className="px-4 py-4">
                      <div className="flex flex-wrap justify-end gap-2">
                        <Link
                          to={`/group-wise-students/${item.studentWiseGroupId}`}
                          className={linkButtonClass}
                        >
                          View
                        </Link>
                        {canManageAssignments ? (
                          <Link
                            to={`/group-wise-students/${item.studentWiseGroupId}/edit`}
                            className={linkButtonClass}
                          >
                            Edit
                          </Link>
                        ) : null}
                        {canManageAssignments && item.isActive ? (
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

export default GroupWiseStudentsPage;
