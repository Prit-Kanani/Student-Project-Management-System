import { apiEndpoints } from "@/CommonLibrary/functions";
import type { ListResult, OperationResultDTO } from "@/CommonLibrary/types";
import type {
  ProjectGroupCreateInput,
  ProjectGroupListItem,
  ProjectGroupUpdateInput,
  ProjectGroupView,
} from "@/ProjectGroupServices/types";
import {
  mapListResult,
  mapOperationResult,
  readBoolean,
  readNullableBoolean,
  readNullableString,
  readNumber,
  readString,
} from "@/ProjectGroupServices/services/service-utils";
import { projectGroupServicesRequest } from "@/ProjectGroupServices/services/request";

const mapProjectGroupListItem = (payload: unknown): ProjectGroupListItem => ({
  projectGroupId: readNumber(payload, "projectGroupId", "ProjectGroupID", "ProjectGroupId"),
  projectGroupName: readString(payload, "projectGroupName", "ProjectGroupName"),
  isApproved: readNullableBoolean(payload, "isApproved", "IsApproved"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
});

const mapProjectGroupView = (
  payload: unknown,
  projectGroupId = 0,
): ProjectGroupView => ({
  projectGroupId,
  projectGroupName: readString(payload, "projectGroupName", "ProjectGroupName"),
  approvalStatusString: readString(
    payload,
    "approvalStatusString",
    "ApprovalStatusString",
  ),
  approvedBy: readNullableString(payload, "approvedBy", "ApprovedBy"),
  createdBy: readNullableString(payload, "createdBy", "CreatedBy"),
  modifiedBy: readNullableString(payload, "modifiedBy", "ModifiedBy"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
  created: readNullableString(payload, "created", "Created"),
  modified: readNullableString(payload, "modified", "Modified"),
});

const mapProjectGroupPrimaryKey = (
  payload: unknown,
  fallbackProjectGroupId = 0,
): ProjectGroupUpdateInput => {
  const approvedById = readNumber(payload, "approvedById", "ApprovedByID", "ApprovedById");
  const modifiedById = readNumber(payload, "modifiedById", "ModifiedByID", "ModifiedById");

  return {
    projectGroupId:
      readNumber(payload, "projectGroupId", "ProjectGroupID", "ProjectGroupId") ||
      fallbackProjectGroupId,
    projectGroupName: readString(payload, "projectGroupName", "ProjectGroupName"),
    isApproved: readNullableBoolean(payload, "isApproved", "IsApproved"),
    approvedById: approvedById > 0 ? approvedById : null,
    isActive: readBoolean(payload, "isActive", "IsActive"),
    createdById: readNumber(payload, "createdById", "CreatedByID", "CreatedById"),
    modifiedById: modifiedById > 0 ? modifiedById : null,
  };
};

export const getProjectGroupsPage = async (): Promise<ListResult<ProjectGroupListItem>> => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.projectGroup.page,
  });

  return mapListResult(response, mapProjectGroupListItem);
};

export const getProjectGroupView = async (projectGroupId: number) => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.projectGroup.view(projectGroupId),
  });

  return mapProjectGroupView(response, projectGroupId);
};

export const getProjectGroupPrimaryKey = async (projectGroupId: number) => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.projectGroup.primaryKey(projectGroupId),
  });

  return mapProjectGroupPrimaryKey(response, projectGroupId);
};

export const createProjectGroup = async (
  payload: ProjectGroupCreateInput,
): Promise<OperationResultDTO> => {
  const response = await projectGroupServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.projectGroupService.projectGroup.create,
    method: "POST",
    body: {
      projectGroupName: payload.projectGroupName,
      isApproved: payload.isApproved,
      isActive: payload.isActive,
      approvedById: payload.approvedById,
      createdById: payload.createdById,
    },
  });

  return mapOperationResult(response);
};

export const updateProjectGroup = async (
  payload: ProjectGroupUpdateInput,
): Promise<OperationResultDTO> => {
  const response = await projectGroupServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.projectGroupService.projectGroup.update,
    method: "POST",
    body: {
      projectGroupId: payload.projectGroupId,
      projectGroupName: payload.projectGroupName,
      isApproved: payload.isApproved,
      approvedById: payload.approvedById,
      isActive: payload.isActive,
      createdById: payload.createdById,
      modifiedById: payload.modifiedById,
    },
  });

  return mapOperationResult(response);
};

export const deactivateProjectGroup = async (
  projectGroupId: number,
): Promise<OperationResultDTO> => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.projectGroup.deactivate(projectGroupId),
    method: "DELETE",
  });

  return mapOperationResult(response);
};
