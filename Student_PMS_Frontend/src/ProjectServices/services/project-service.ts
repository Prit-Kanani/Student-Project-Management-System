import { apiEndpoints } from "@/CommonLibrary/functions";
import type {
  EntityExistsDTO,
  ListResult,
  OperationResultDTO,
} from "@/CommonLibrary/types";
import type {
  ProjectCreateInput,
  ProjectListItem,
  ProjectUpdateInput,
  ProjectView,
} from "@/ProjectServices/types";
import {
  mapEntityExists,
  mapListResult,
  mapOperationResult,
  readBoolean,
  readNullableBoolean,
  readNullableString,
  readNumber,
  readString,
} from "@/ProjectServices/services/service-utils";
import { projectServicesRequest } from "@/ProjectServices/services/request";

const mapProjectListItem = (payload: unknown): ProjectListItem => ({
  projectId: readNumber(payload, "projectId", "ProjectID", "ProjectId"),
  projectName: readString(payload, "projectName", "ProjectName"),
  isApproved: readNullableBoolean(payload, "isApproved", "IsApproved"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
  isCompleted: readBoolean(payload, "isCompleted", "IsCompleted"),
});

const mapProjectView = (payload: unknown, projectId = 0): ProjectView => ({
  projectId:
    readNumber(payload, "projectId", "ProjectID", "ProjectId") || projectId,
  projectName: readString(payload, "projectName", "ProjectName"),
  description: readNullableString(payload, "description", "Description"),
  isApproved: readNullableBoolean(payload, "isApproved", "IsApproved"),
  createdById: readNumber(payload, "createdById", "CreatedByID", "CreatedById"),
  modifiedById: (() => {
    const value = readNumber(
      payload,
      "modifiedById",
      "ModifiedByID",
      "ModifiedById",
    );

    return value > 0 ? value : null;
  })(),
  createdBy: readString(payload, "createdBy", "CreatedBy"),
  modifiedBy: readNullableString(payload, "modifiedBy", "ModifiedBy"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
  isCompleted: readBoolean(payload, "isCompleted", "IsCompleted"),
  created: readNullableString(payload, "created", "Created"),
  modified: readNullableString(payload, "modified", "Modified"),
});

const mapProjectPrimaryKey = (
  payload: unknown,
  fallbackProjectId = 0,
): ProjectUpdateInput => ({
  projectId:
    readNumber(payload, "projectId", "ProjectID", "ProjectId") ||
    fallbackProjectId,
  projectName: readString(payload, "projectName", "ProjectName"),
  description: readString(payload, "description", "Description"),
  isApproved: readNullableBoolean(payload, "isApproved", "IsApproved"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
  isCompleted: readBoolean(payload, "isCompleted", "IsCompleted"),
  modifiedById: readNumber(payload, "modifiedById", "ModifiedByID", "ModifiedById"),
});

export const getProjectsPage = async (): Promise<ListResult<ProjectListItem>> => {
  const response = await projectServicesRequest<unknown>({
    path: apiEndpoints.projectService.project.page,
  });

  return mapListResult(response, mapProjectListItem);
};

export const getProjectView = async (projectId: number) => {
  const response = await projectServicesRequest<unknown>({
    path: apiEndpoints.projectService.project.view(projectId),
  });

  return mapProjectView(response, projectId);
};

export const getProjectPrimaryKey = async (projectId: number) => {
  const response = await projectServicesRequest<unknown>({
    path: apiEndpoints.projectService.project.primaryKey(projectId),
  });

  return mapProjectPrimaryKey(response, projectId);
};

export const getProjectExists = async (
  projectId: number,
): Promise<EntityExistsDTO> => {
  const response = await projectServicesRequest<unknown>({
    path: apiEndpoints.projectService.project.exists(projectId),
  });

  return mapEntityExists(response);
};

export const createProject = async (
  payload: ProjectCreateInput,
): Promise<OperationResultDTO> => {
  const response = await projectServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.projectService.project.create,
    method: "POST",
    body: {
      projectName: payload.projectName,
      description: payload.description,
      isApproved: payload.isApproved,
      isActive: payload.isActive,
      isCompleted: payload.isCompleted,
      createdById: payload.createdById,
    },
  });

  return mapOperationResult(response);
};

export const updateProject = async (
  payload: ProjectUpdateInput,
): Promise<OperationResultDTO> => {
  const response = await projectServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.projectService.project.update,
    method: "PUT",
    body: {
      projectId: payload.projectId,
      projectName: payload.projectName,
      description: payload.description,
      isApproved: payload.isApproved,
      isActive: payload.isActive,
      isCompleted: payload.isCompleted,
      modifiedById: payload.modifiedById,
    },
  });

  return mapOperationResult(response);
};

export const deleteProject = async (
  projectId: number,
): Promise<OperationResultDTO> => {
  const response = await projectServicesRequest<unknown>({
    path: apiEndpoints.projectService.project.delete(projectId),
    method: "DELETE",
  });

  return mapOperationResult(response);
};
