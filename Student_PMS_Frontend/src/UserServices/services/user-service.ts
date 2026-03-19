import { apiEndpoints } from "@/CommonLibrary/functions";
import type { ListResult, OperationResultDTO } from "@/CommonLibrary/types";
import type {
  UserCreateInput,
  UserListItem,
  UserUpdateInput,
  UserView,
} from "@/UserServices/types";
import {
  mapListResult,
  mapOperationResult,
  readBoolean,
  readNullableString,
  readNumber,
  readString,
} from "@/UserServices/services/service-utils";
import { userServicesRequest } from "@/UserServices/services/request";

const mapUserListItem = (payload: unknown): UserListItem => ({
  userId: readNumber(payload, "userId", "UserID", "UserId"),
  name: readString(payload, "name", "Name"),
  email: readString(payload, "email", "Email"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
});

const mapUserView = (payload: unknown, userId = 0): UserView => ({
  userId: readNumber(payload, "userId", "UserID", "UserId") || userId,
  name: readString(payload, "name", "Name"),
  email: readString(payload, "email", "Email"),
  roleName: readNullableString(payload, "roleName", "RoleName"),
  createdBy: readNullableString(payload, "createdBy", "CreatedBy"),
  modifiedBy: readNullableString(payload, "modifiedBy", "ModifiedBy"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
  created: readNullableString(payload, "created", "Created"),
  modified: readNullableString(payload, "modified", "Modified"),
});

const mapUserPrimaryKey = (payload: unknown, fallbackUserId = 0): UserUpdateInput => ({
  userId:
    readNumber(payload, "userId", "UserID", "UserId") || fallbackUserId,
  name: readString(payload, "name", "Name"),
  email: readString(payload, "email", "Email"),
  roleId: readNumber(payload, "roleId", "RoleID", "RoleId"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
});

export const getUsersPage = async (): Promise<ListResult<UserListItem>> => {
  const response = await userServicesRequest<unknown>({
    path: apiEndpoints.userService.user.page,
  });

  return mapListResult(response, mapUserListItem);
};

export const getUserView = async (userId: number) => {
  const response = await userServicesRequest<unknown>({
    path: apiEndpoints.userService.user.view(userId),
  });

  return mapUserView(response, userId);
};

export const getUserPrimaryKey = async (userId: number) => {
  const response = await userServicesRequest<unknown>({
    path: apiEndpoints.userService.user.primaryKey(userId),
  });

  return mapUserPrimaryKey(response, userId);
};

export const createUser = async (
  payload: UserCreateInput,
): Promise<OperationResultDTO> => {
  const response = await userServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.userService.user.insert,
    method: "POST",
    body: {
      name: payload.name,
      email: payload.email,
      password: payload.password,
      roleId: payload.roleId,
      isActive: payload.isActive,
    },
  });

  return mapOperationResult(response);
};

export const updateUser = async (
  payload: UserUpdateInput,
): Promise<OperationResultDTO> => {
  const response = await userServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.userService.user.update,
    method: "PUT",
    body: {
      userId: payload.userId,
      name: payload.name,
      email: payload.email,
      roleId: payload.roleId,
      isActive: payload.isActive,
    },
  });

  return mapOperationResult(response);
};

export const deactivateUser = async (
  userId: number,
): Promise<OperationResultDTO> => {
  const response = await userServicesRequest<unknown>({
    path: apiEndpoints.userService.user.deactivate(userId),
    method: "DELETE",
  });

  return mapOperationResult(response);
};
