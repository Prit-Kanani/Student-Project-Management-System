import { apiEndpoints } from "@/CommonLibrary/functions";
import type { ListResult, OperationResultDTO } from "@/CommonLibrary/types";
import type {
  GroupWiseStudentCreateInput,
  GroupWiseStudentListItem,
  GroupWiseStudentUpdateInput,
  GroupWiseStudentView,
} from "@/ProjectGroupServices/types";
import {
  mapListResult,
  mapOperationResult,
  readBoolean,
  readNullableString,
  readNumber,
} from "@/ProjectGroupServices/services/service-utils";
import { projectGroupServicesRequest } from "@/ProjectGroupServices/services/request";

const mapGroupWiseStudentListItem = (payload: unknown): GroupWiseStudentListItem => ({
  studentWiseGroupId: readNumber(
    payload,
    "studentWiseGroupId",
    "StudentWiseGroupID",
    "StudentWiseGroupId",
  ),
  projectGroupId: readNumber(payload, "projectGroupId", "ProjectGroupID", "ProjectGroupId"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
});

const mapGroupWiseStudentView = (
  payload: unknown,
  studentWiseGroupId = 0,
): GroupWiseStudentView => ({
  studentWiseGroupId:
    readNumber(
      payload,
      "studentWiseGroupId",
      "StudentWiseGroupID",
      "StudentWiseGroupId",
    ) || studentWiseGroupId,
  projectGroupId: readNumber(payload, "projectGroupId", "ProjectGroupID", "ProjectGroupId"),
  studentId: readNumber(payload, "studentId", "StudentID", "StudentId"),
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

const mapGroupWiseStudentPrimaryKey = (
  payload: unknown,
  fallbackStudentWiseGroupId = 0,
): GroupWiseStudentUpdateInput => ({
  studentWiseGroupId:
    readNumber(
      payload,
      "studentWiseGroupId",
      "StudentWiseGroupID",
      "StudentWiseGroupId",
    ) || fallbackStudentWiseGroupId,
  projectGroupId: readNumber(payload, "projectGroupId", "ProjectGroupID", "ProjectGroupId"),
  studentId: readNumber(payload, "studentId", "StudentID", "StudentId"),
  isActive: readBoolean(payload, "isActive", "IsActive"),
  createdById: readNumber(payload, "createdById", "CreatedByID", "CreatedById"),
  modifiedById: (() => {
    const value = readNumber(payload, "modifiedById", "ModifiedByID", "ModifiedById");
    return value > 0 ? value : null;
  })(),
});

export const getGroupWiseStudentsPage = async (
  skip = 0,
  take = 10,
): Promise<ListResult<GroupWiseStudentListItem>> => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.groupWiseStudent.page,
    query: { skip, take },
  });

  return mapListResult(response, mapGroupWiseStudentListItem);
};

export const getGroupWiseStudentView = async (studentWiseGroupId: number) => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.groupWiseStudent.view(studentWiseGroupId),
  });

  return mapGroupWiseStudentView(response, studentWiseGroupId);
};

export const getGroupWiseStudentPrimaryKey = async (
  studentWiseGroupId: number,
) => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.groupWiseStudent.primaryKey(
      studentWiseGroupId,
    ),
  });

  return mapGroupWiseStudentPrimaryKey(response, studentWiseGroupId);
};

export const createGroupWiseStudent = async (
  payload: GroupWiseStudentCreateInput,
): Promise<OperationResultDTO> => {
  const response = await projectGroupServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.projectGroupService.groupWiseStudent.create,
    method: "POST",
    body: {
      projectGroupId: payload.projectGroupId,
      studentId: payload.studentId,
      isActive: payload.isActive,
      createdById: payload.createdById,
      modifiedById: payload.modifiedById,
    },
  });

  return mapOperationResult(response);
};

export const updateGroupWiseStudent = async (
  payload: GroupWiseStudentUpdateInput,
): Promise<OperationResultDTO> => {
  const response = await projectGroupServicesRequest<unknown, Record<string, unknown>>({
    path: apiEndpoints.projectGroupService.groupWiseStudent.update,
    method: "POST",
    body: {
      studentWiseGroupId: payload.studentWiseGroupId,
      projectGroupId: payload.projectGroupId,
      studentId: payload.studentId,
      isActive: payload.isActive,
      createdById: payload.createdById,
      modifiedById: payload.modifiedById,
    },
  });

  return mapOperationResult(response);
};

export const deactivateGroupWiseStudent = async (
  studentWiseGroupId: number,
): Promise<OperationResultDTO> => {
  const response = await projectGroupServicesRequest<unknown>({
    path: apiEndpoints.projectGroupService.groupWiseStudent.deactivate(
      studentWiseGroupId,
    ),
    method: "DELETE",
  });

  return mapOperationResult(response);
};
