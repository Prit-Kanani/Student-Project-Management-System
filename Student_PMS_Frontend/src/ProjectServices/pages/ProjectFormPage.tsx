import { useCallback, useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router";
import { useAuth } from "@/AuthServices/components";
import ComponentCard from "@/components/common/ComponentCard";
import {
  buildProjectFormValues,
  FeedbackStack,
  PageShell,
  ProjectForm,
} from "@/ProjectServices/components";
import {
  createProject,
  getExceptionFeedback,
  getProjectPrimaryKey,
  updateProject,
} from "@/ProjectServices/services";
import type { ExceptionFeedback } from "@/ProjectServices/services";
import type {
  PageNotice,
  ProjectCreateInput,
  ProjectFormValues,
  ProjectUpdateInput,
} from "@/ProjectServices/types";

interface ProjectFormPageProps {
  mode: "create" | "edit";
}

const linkButtonClass =
  "inline-flex items-center justify-center rounded-lg border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-white/5";

const ProjectFormPage = ({ mode }: ProjectFormPageProps) => {
  const navigate = useNavigate();
  const params = useParams();
  const { user } = useAuth();
  const projectId = Number(params.projectId ?? "0");
  const [initialValues, setInitialValues] = useState<Partial<ProjectFormValues>>();
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
      const projectPrimaryKey = await getProjectPrimaryKey(projectId);
      setInitialValues(buildProjectFormValues(projectPrimaryKey));
    } catch (requestError) {
      setError(getExceptionFeedback(requestError));
    } finally {
      setIsLoading(false);
    }
  }, [mode, projectId]);

  useEffect(() => {
    void loadForm();
  }, [loadForm]);

  const handleSubmit = async (
    payload: ProjectCreateInput | ProjectUpdateInput,
  ) => {
    setIsSubmitting(true);
    setNotice(null);
    setError(null);

    try {
      if (mode === "edit") {
        await updateProject(payload as ProjectUpdateInput);
        navigate("/projects", {
          replace: true,
          state: {
            notice: {
              variant: "success",
              title: "Project updated",
              message: "The project has been updated successfully.",
            },
          },
        });
        return;
      }

      await createProject(payload as ProjectCreateInput);
      navigate("/projects", {
        replace: true,
        state: {
          notice: {
            variant: "success",
            title: "Project created",
            message: "The project has been created successfully.",
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
      title={mode === "edit" ? "Edit Project" : "Create Project"}
      description={
        mode === "edit"
          ? "Update project details, approval status, completion, and active state."
          : "Create a new project record and send it to the ProjectService backend."
      }
      actions={<Link to="/projects" className={linkButtonClass}>Back to Projects</Link>}
    >
      <FeedbackStack notice={notice} error={error} />
      <ComponentCard
        title={mode === "edit" ? "Project Editor" : "New Project"}
        desc="The form payload maps to ProjectCreateDTO and ProjectUpdateDTO in the backend."
      >
        {isLoading ? (
          <p className="text-sm text-gray-500 dark:text-gray-400">Loading project data...</p>
        ) : (
          <ProjectForm
            mode={mode}
            projectId={projectId}
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

export default ProjectFormPage;
