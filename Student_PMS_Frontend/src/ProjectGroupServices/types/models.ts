export type ApprovalStatusValue = "pending" | "approved" | "rejected";

export interface PageNotice {
  variant: "success" | "error" | "warning" | "info";
  title: string;
  message: string;
}

export interface LookupOption {
  value: number;
  label: string;
}

export interface ProjectGroupListItem {
  projectGroupId: number;
  projectGroupName: string;
  isApproved: boolean | null;
  isActive: boolean;
}

export interface ProjectGroupView {
  projectGroupId: number;
  projectGroupName: string;
  approvalStatusString: string;
  approvedBy: string | null;
  createdBy: string | null;
  modifiedBy: string | null;
  isActive: boolean;
  created: string | null;
  modified: string | null;
}

export interface ProjectGroupCreateInput {
  projectGroupName: string;
  isApproved: boolean | null;
  isActive: boolean;
  approvedById: number | null;
  createdById: number;
}

export interface ProjectGroupUpdateInput {
  projectGroupId: number;
  projectGroupName: string;
  isApproved: boolean | null;
  approvedById: number | null;
  isActive: boolean;
  createdById: number;
  modifiedById: number | null;
}

export interface ProjectGroupFormValues {
  projectGroupName: string;
  approvalStatus: ApprovalStatusValue;
  isActive: boolean;
}

export interface ProjectGroupFormErrors {
  projectGroupName?: string;
  approvalStatus?: string;
  form?: string;
}

export interface ProjectGroupByProjectListItem {
  projectGroupByProjectId: number;
  projectGroupName: string;
  projectId: number;
  isActive: boolean;
}

export interface ProjectGroupByProjectView {
  projectGroupByProjectId: number;
  projectGroupName: string;
  projectId: number;
  isActive: boolean;
  created: string | null;
  modified: string | null;
  createdById: number;
  modifiedById: number | null;
  createdBy: string | null;
  modifiedBy: string | null;
}

export interface ProjectGroupByProjectCreateInput {
  projectGroupId: number;
  projectId: number;
  isActive: boolean;
  createdById: number;
}

export interface ProjectGroupByProjectUpdateInput {
  projectGroupByProjectId: number;
  isActive: boolean;
  projectGroupId: number;
  projectId: number;
  modifiedById: number | null;
}

export interface ProjectGroupByProjectFormValues {
  projectGroupId: string;
  projectId: string;
  isActive: boolean;
}

export interface ProjectGroupByProjectFormErrors {
  projectGroupId?: string;
  projectId?: string;
  form?: string;
}

export interface GroupWiseStudentListItem {
  studentWiseGroupId: number;
  projectGroupId: number;
  isActive: boolean;
}

export interface GroupWiseStudentView {
  studentWiseGroupId: number;
  projectGroupId: number;
  studentId: number;
  isActive: boolean;
  created: string | null;
  modified: string | null;
  createdById: number;
  modifiedById: number | null;
  createdBy: string | null;
  modifiedBy: string | null;
}

export interface GroupWiseStudentCreateInput {
  projectGroupId: number;
  studentId: number;
  isActive: boolean;
  createdById: number;
  modifiedById: number | null;
}

export interface GroupWiseStudentUpdateInput {
  studentWiseGroupId: number;
  projectGroupId: number;
  studentId: number;
  isActive: boolean;
  createdById: number;
  modifiedById: number | null;
}

export interface GroupWiseStudentFormValues {
  projectGroupId: string;
  studentId: string;
  isActive: boolean;
}

export interface GroupWiseStudentFormErrors {
  projectGroupId?: string;
  studentId?: string;
  form?: string;
}
