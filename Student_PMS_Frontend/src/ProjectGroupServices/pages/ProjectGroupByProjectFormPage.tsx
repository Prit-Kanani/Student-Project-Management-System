import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router";
import { useAuth } from "@/AuthServices/components";
import ComponentCard from "@/components/common/ComponentCard";
import {
  buildProjectGroupByProjectFormValues,
  FeedbackStack,
  PageShell,
  ProjectGroupByProjectForm,
} from "@/ProjectGroupServices/components";
import {
  createProjectGroupByProject,
  getExceptionFeedback,
  getProjectGroupByProjectPrimaryKey,
  getProjectGroupOptions,
  getProjectOptions,
  updateProjectGroupByProject,
} from "@/ProjectGroupServices/services";
import type { ExceptionFeedback } from "@/ProjectGroupServices/services";
import type {
  LookupOption,
  PageNotice,
  ProjectGroupByProjectCreateInput,
  ProjectGroupByProjectFormValues,
  ProjectGroupByProjectUpdateInput,
} from "@/ProjectGroupServices/types";

interface ProjectGroupByProjectFormPageProps {
  mode: "create" | "edit";
}

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const ProjectGroupByProjectFormPage = ({
  mode,
}: ProjectGroupByProjectFormPageProps) => {
  const navigate = useNavigate();
  const params = useParams();
  const { user } = useAuth();
  const projectGroupByProjectId = Number(params.projectGroupByProjectId ?? "0");
  const [initialValues, setInitialValues] = useState<
    Partial<ProjectGroupByProjectFormValues>
  >();
  const [projectGroupOptions, setProjectGroupOptions] = useState<LookupOption[]>([]);
  const [projectOptions, setProjectOptions] = useState<LookupOption[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [notice, setNotice] = useState<PageNotice | null>(null);
  const [error, setError] = useState<ExceptionFeedback | null>(null);

  const loadForm = useCallback(async () => {
    setIsLoading(true);
    setError(null);

    try {
      const [nextProjectGroupOptions, nextProjectOptions, primaryKey] = await Promise.all([
        getProjectGroupOptions(),
        getProjectOptions(),
        mode === "edit"
          ? getProjectGroupByProjectPrimaryKey(projectGroupByProjectId)
          : Promise.resolve(null),
      ]);

      setProjectGroupOptions(nextProjectGroupOptions);
      setProjectOptions(nextProjectOptions);
      setInitialValues(
        primaryKey ? buildProjectGroupByProjectFormValues(primaryKey) : undefined,
      );
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [mode, projectGroupByProjectId]);

  useEffect(() => {
    void loadForm();
  }, [loadForm]);

  const handleSubmit = async (
    payload: ProjectGroupByProjectCreateInput | ProjectGroupByProjectUpdateInput,
  ) => {
    setIsSubmitting(true);
    setNotice(null);
    setError(null);

    try {
      if (mode === "edit") {
        await updateProjectGroupByProject(payload as ProjectGroupByProjectUpdateInput);
        navigate("/project-group-projects", {
          replace: true,
          state: {
            notice: {
              variant: "success",
              title: "Mapping updated",
              message: "The project group mapping has been updated successfully.",
            },
          },
        });
        return;
      }

      await createProjectGroupByProject(payload as ProjectGroupByProjectCreateInput);
      navigate("/project-group-projects", {
        replace: true,
        state: {
          notice: {
            variant: "success",
            title: "Mapping created",
            message: "The project group mapping has been created successfully.",
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
      title={mode === "edit" ? "Edit Group Project Link" : "Create Group Project Link"}
      description={
        mode === "edit"
          ? "Update the selected project group and project mapping."
          : "Create a new mapping between a project group and a project."
      }
      actions={<Link to="/project-group-projects" className={linkButtonClass}>Back to Group Project Links</Link>}
    >
      <FeedbackStack notice={notice} error={error} />
      <ComponentCard
        title={mode === "edit" ? "Mapping Editor" : "New Group Project Link"}
        desc="The form payload maps to ProjectGroupByProjectCreateDTO and ProjectGroupByProjectUpdateDTO in the backend."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading mapping form data...</p>
        ) : (
          <ProjectGroupByProjectForm
            mode={mode}
            projectGroupByProjectId={projectGroupByProjectId}
            currentUserId={user?.userId ?? 0}
            projectGroupOptions={projectGroupOptions}
            projectOptions={projectOptions}
            initialValues={initialValues}
            isSubmitting={isSubmitting}
            onSubmit={handleSubmit}
          />
        )}
      </ComponentCard>
    </PageShell>
  );
};

export default ProjectGroupByProjectFormPage;
