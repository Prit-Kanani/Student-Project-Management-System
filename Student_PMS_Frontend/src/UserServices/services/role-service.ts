import { apiEndpoints } from "@/CommonLibrary/functions";
import type { ListResult, OperationResultDTO, OptionDTO } from "@/CommonLibrary/types";
import type {
  RoleCreateInput,
  RoleListItem,
  RoleUpdateInput,
  RoleView,
} from "@/UserServices/types";
import {
  mapListResult,
  mapOperationResult,
  mapOption,
  readBoolean,
  readNullableString,
  readNumber,
  readString,
} from "@/UserServices/services/service-utils";
import { userServicesRequest } from "@/UserServices/services/request";

const mapRoleListItem = (payload: unknown): RoleListItem => ({
  roleId: readNumber(payload, "roleId", "RoleID", "RoleId"),
  roleName: readString(payload, "roleName", "RoleName"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
});

const mapRoleView = (payload: unknown, roleId = 0): RoleView => ({
  roleId: readNumber(payload, "roleId", "RoleID", "RoleId") || roleId,
  roleName: readString(payload, "roleName", "RoleName"),
  description: readNullableString(payload, "description", "Description"),
  createdBy: readNullableString(payload, "createdBy", "CreatedBy"),
  modifiedBy: readNullableString(payload, "modifiedBy", "ModifiedBy"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
  created: readNullableString(payload, "created", "Created"),
  modified: readNullableString(payload, "modified", "Modified"),
});

const mapRolePrimaryKey = (payload: unknown, fallbackRoleId = 0): RoleUpdateInput => ({
  roleId:
    readNumber(payload, "roleId", "RoleID", "RoleId") || fallbackRoleId,
  roleName: readString(payload, "roleName", "RoleName"),
  description: readString(payload, "description", "Description"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
});

export const getRolesPage = async (): Promise<ListResult<RoleListItem>> => {
  const response = await userServicesRequest<unknown>({
    path: apiEndpoints.userService.role.page,
  });

  return mapListResult(response, mapRoleListItem);
};

export const getRoleView = async (roleId: number) => {
  const response = await userServicesRequest<unknown>({
    path: apiEndpoints.userService.role.view(roleId),
  });

  return mapRoleView(response, roleId);
};

export const getRolePrimaryKey = async (roleId: number) => {
  const response = await userServicesRequest<unknown>({
    path: apiEndpoints.userService.role.primaryKey(roleId),
  });

  return mapRolePrimaryKey(response, roleId);
};

export const getRoleOptions = async (): Promise<OptionDTO[]> => {
  const response = await userServicesRequest<unknown[]>({
    path: apiEndpoints.userService.role.dropdown,
  });

  return Array.isArray(response) ? response.map(mapOption) : [];
};

export const createRole = async (
  payload: RoleCreateInput,
): Promise<OperationResultDTO> => {
  const response = await userServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.userService.role.insert,
    method: "POST",
    body: {
      roleName: payload.roleName,
      description: payload.description,
      isActive: payload.isActive,
      createdById: payload.createdById,
    },
  });

  return mapOperationResult(response);
};

export const updateRole = async (
  payload: RoleUpdateInput,
): Promise<OperationResultDTO> => {
  const response = await userServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.userService.role.update,
    method: "PUT",
    body: {
      roleId: payload.roleId,
      roleName: payload.roleName,
      description: payload.description,
      isActive: payload.isActive,
    },
  });

  return mapOperationResult(response);
};

export const deactivateRole = async (
  roleId: number,
): Promise<OperationResultDTO> => {
  const response = await userServicesRequest<unknown>({
    path: apiEndpoints.userService.role.deactivate(roleId),
    method: "DELETE",
  });

  return mapOperationResult(response);
};
