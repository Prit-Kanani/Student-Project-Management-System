export type ProjectApprovalStatus = "pending" | "approved" | "rejected";

export interface PageNotice {
  variant: "success" | "error" | "warning" | "info";
  title: string;
  message: string;
}

export interface ProjectListItem {
  projectId: number;
  projectName: string;
  isApproved: boolean | null;
  isActive: boolean;
  isCompleted: boolean;
}

export interface ProjectView {
  projectId: number;
  projectName: string;
  description: string | null;
  isApproved: boolean | null;
  createdById: number;
  modifiedById: number | null;
  createdBy: string;
  modifiedBy: string | null;
  isActive: boolean;
  isCompleted: boolean;
  created: string | null;
  modified: string | null;
}

export interface ProjectCreateInput {
  projectName: string;
  description: string;
  isApproved: boolean | null;
  isActive: boolean;
  isCompleted: boolean;
  createdById: number;
}

export interface ProjectUpdateInput {
  projectId: number;
  projectName: string;
  description: string;
  isApproved: boolean | null;
  isActive: boolean;
  isCompleted: boolean;
  modifiedById: number;
}

export interface ProjectFormValues {
  projectName: string;
  description: string;
  approvalStatus: ProjectApprovalStatus;
  isActive: boolean;
  isCompleted: boolean;
}

export interface ProjectFormErrors {
  projectName?: string;
  description?: string;
  approvalStatus?: string;
  form?: string;
}
