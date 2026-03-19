import { apiEndpoints } from "@/CommonLibrary/functions";
import type { ListResult, OperationResultDTO } from "@/CommonLibrary/types";
import type {
  ProjectGroupByProjectCreateInput,
  ProjectGroupByProjectListItem,
  ProjectGroupByProjectUpdateInput,
  ProjectGroupByProjectView,
} from "@/ProjectGroupServices/types";
import {
  mapListResult,
  mapOperationResult,
  readBoolean,
  readNullableString,
  readNumber,
  readString,
} from "@/ProjectGroupServices/services/service-utils";
import { projectGroupServicesRequest } from "@/ProjectGroupServices/services/request";

const mapProjectGroupByProjectListItem = (
  payload: unknown,
): ProjectGroupByProjectListItem => ({
  projectGroupByProjectId: readNumber(
    payload,
    "projectGroupByProjectId",
    "ProjectGroupByProjectID",
    "ProjectGroupByProjectId",
  ),
  projectGroupName: readString(payload, "projectGroupName", "ProjectGroupName"),
  projectId: readNumber(payload, "projectId", "ProjectID", "ProjectId"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
});

const mapProjectGroupByProjectView = (
  payload: unknown,
  projectGroupByProjectId = 0,
): ProjectGroupByProjectView => ({
  projectGroupByProjectId,
  projectGroupName: readString(payload, "projectGroupName", "ProjectGroupName"),
  projectId: readNumber(payload, "projectId", "ProjectID", "ProjectId"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
  created: readNullableString(payload, "created", "Created"),
  modified: readNullableString(payload, "modified", "Modified"),
  createdById: readNumber(payload, "createdById", "CreatedByID", "CreatedById"),
  modifiedById: (() => {
    const value = readNumber(payload, "modifiedById", "ModifiedByID", "ModifiedById");
    return value > 0 ? value : null;
  })(),
  createdBy: readNullableString(payload, "createdBy", "CreatedBy"),
  modifiedBy: readNullableString(payload, "modifiedBy", "ModifiedBy"),
});

const mapProjectGroupByProjectPrimaryKey = (
  payload: unknown,
  fallbackProjectGroupByProjectId = 0,
): ProjectGroupByProjectUpdateInput => ({
  projectGroupByProjectId:
    readNumber(
      payload,
      "projectGroupByProjectId",
      "ProjectGroupByProjectID",
      "ProjectGroupByProjectId",
    ) || fallbackProjectGroupByProjectId,
  isActive: readBoolean(payload, "isActive", "IsActive"),
  projectGroupId: readNumber(payload, "projectGroupId", "ProjectGroupID", "ProjectGroupId"),
  projectId: readNumber(payload, "projectId", "ProjectID", "ProjectId"),
  modifiedById: (() => {
    const value = readNumber(payload, "modifiedById", "ModifiedByID", "ModifiedById");
    return value > 0 ? value : null;
  })(),
});

export const getProjectGroupByProjectsPage = async (): Promise<
  ListResult<ProjectGroupByProjectListItem>
> => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.projectGroupByProject.page,
  });

  return mapListResult(response, mapProjectGroupByProjectListItem);
};

export const getProjectGroupByProjectView = async (
  projectGroupByProjectId: number,
) => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.projectGroupByProject.view(
      projectGroupByProjectId,
    ),
  });

  return mapProjectGroupByProjectView(response, projectGroupByProjectId);
};

export const getProjectGroupByProjectPrimaryKey = async (
  projectGroupByProjectId: number,
) => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.projectGroupByProject.primaryKey(
      projectGroupByProjectId,
    ),
  });

  return mapProjectGroupByProjectPrimaryKey(response, projectGroupByProjectId);
};

export const createProjectGroupByProject = async (
  payload: ProjectGroupByProjectCreateInput,
): Promise<OperationResultDTO> => {
  const response = await projectGroupServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.projectGroupService.projectGroupByProject.create,
    method: "POST",
    body: {
      projectGroupId: payload.projectGroupId,
      projectId: payload.projectId,
      isActive: payload.isActive,
      createdById: payload.createdById,
    },
  });

  return mapOperationResult(response);
};

export const updateProjectGroupByProject = async (
  payload: ProjectGroupByProjectUpdateInput,
): Promise<OperationResultDTO> => {
  const response = await projectGroupServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.projectGroupService.projectGroupByProject.update,
    method: "POST",
    body: {
      projectGroupByProjectId: payload.projectGroupByProjectId,
      isActive: payload.isActive,
      projectGroupId: payload.projectGroupId,
      projectId: payload.projectId,
      modifiedById: payload.modifiedById,
    },
  });

  return mapOperationResult(response);
};

export const deactivateProjectGroupByProject = async (
  projectGroupByProjectId: number,
): Promise<OperationResultDTO> => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.projectGroupByProject.deactivate(
      projectGroupByProjectId,
    ),
    method: "DELETE",
  });

  return mapOperationResult(response);
};
