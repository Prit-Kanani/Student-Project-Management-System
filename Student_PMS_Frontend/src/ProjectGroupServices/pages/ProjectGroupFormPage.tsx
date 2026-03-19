import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router";
import { useAuth } from "@/AuthServices/components";
import ComponentCard from "@/components/common/ComponentCard";
import {
  buildProjectGroupFormValues,
  FeedbackStack,
  PageShell,
  ProjectGroupForm,
} from "@/ProjectGroupServices/components";
import {
  createProjectGroup,
  getExceptionFeedback,
  getProjectGroupPrimaryKey,
  updateProjectGroup,
} from "@/ProjectGroupServices/services";
import type { ExceptionFeedback } from "@/ProjectGroupServices/services";
import type {
  PageNotice,
  ProjectGroupCreateInput,
  ProjectGroupFormValues,
  ProjectGroupUpdateInput,
} from "@/ProjectGroupServices/types";

interface ProjectGroupFormPageProps {
  mode: "create" | "edit";
}

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const ProjectGroupFormPage = ({ mode }: ProjectGroupFormPageProps) => {
  const navigate = useNavigate();
  const params = useParams();
  const { user } = useAuth();
  const projectGroupId = Number(params.projectGroupId ?? "0");
  const [initialValues, setInitialValues] = useState<Partial<ProjectGroupFormValues>>();
  const [createdById, setCreatedById] = useState(0);
  const [isLoading, setIsLoading] = useState(mode === "edit");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [notice, setNotice] = useState<PageNotice | null>(null);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const loadForm = useCallback(async () => {
    if (mode !== "edit") {
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const projectGroupPrimaryKey = await getProjectGroupPrimaryKey(projectGroupId);
      setInitialValues(buildProjectGroupFormValues(projectGroupPrimaryKey));
      setCreatedById(projectGroupPrimaryKey.createdById);
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [mode, projectGroupId]);

  useEffect(() => {
    void loadForm();
  }, [loadForm]);

  const handleSubmit = async (
    payload: ProjectGroupCreateInput | ProjectGroupUpdateInput,
  ) => {
    setIsSubmitting(true);
    setNotice(null);
    setError(null);

    try {
      if (mode === "edit") {
        await updateProjectGroup(payload as ProjectGroupUpdateInput);
        navigate("/project-groups", {
          replace: true,
          state: {
            notice: {
              variant: "success",
              title: "Project group updated",
              message: "The project group has been updated successfully.",
            },
          },
        });
        return;
      }

      await createProjectGroup(payload as ProjectGroupCreateInput);
      navigate("/project-groups", {
        replace: true,
        state: {
          notice: {
            variant: "success",
            title: "Project group created",
            message: "The project group has been created successfully.",
          },
        },
      });
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <PageShell
      title={mode === "edit" ? "Edit Project Group" : "Create Project Group"}
      description={
        mode === "edit"
          ? "Update project group naming, approval state, and active status."
          : "Create a new project group record and send it to the ProjectGroupService backend."
      }
      actions={<Link to="/project-groups" className={linkButtonClass}>Back to Project Groups</Link>}
    >
      <FeedbackStack notice={notice} error={error} />
      <ComponentCard
        title={mode === "edit" ? "Project Group Editor" : "New Project Group"}
        desc="The form payload maps to ProjectGroupCreateDTO and ProjectGroupUpdateDTO in the backend."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading project group data...</p>
        ) : (
          <ProjectGroupForm
            mode={mode}
            projectGroupId={projectGroupId}
            createdById={createdById}
            currentUserId={user?.userId ?? 0}
            initialValues={initialValues}
            isSubmitting={isSubmitting}
            onSubmit={handleSubmit}
          />
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default ProjectGroupFormPage;
