export interface PageNotice {
  variant: "success" | "error" | "warning" | "info";
  title: string;
  message: string;
}

export interface UserListItem {
  userId: number;
  name: string;
  email: string;
  isActive: boolean;
}

export interface UserView {
  userId: number;
  name: string;
  email: string;
  roleName: string | null;
  createdBy: string | null;
  modifiedBy: string | null;
  isActive: boolean;
  created: string | null;
  modified: string | null;
}

export interface UserCreateInput {
  name: string;
  email: string;
  password: string;
  roleId: number;
  isActive: boolean;
}

export interface UserUpdateInput {
  userId: number;
  name: string;
  email: string;
  roleId: number;
  isActive: boolean;
}

export interface RoleListItem {
  roleId: number;
  roleName: string;
  isActive: boolean;
}

export interface RoleView {
  roleId: number;
  roleName: string;
  description: string | null;
  createdBy: string | null;
  modifiedBy: string | null;
  isActive: boolean;
  created: string | null;
  modified: string | null;
}

export interface RoleCreateInput {
  roleName: string;
  description: string;
  isActive: boolean;
  createdById: number;
}

export interface RoleUpdateInput {
  roleId: number;
  roleName: string;
  description: string;
  isActive: boolean;
}

export interface UserProfileListItem {
  userProfileId: number;
  userId: number;
  displayName: string;
  firstName: string | null;
  lastName: string | null;
  phoneNumber: string;
  gender: string;
}

export interface UserProfileView {
  userProfileId: number;
  userId: number;
  displayName: string;
  firstName: string | null;
  lastName: string | null;
  phoneNumber: string;
  profileImageUrl: string;
  dateOfBirth: string | null;
  gender: string;
}

export interface UserProfileCreateInput {
  userId: number;
  firstName: string;
  lastName: string;
  displayName: string;
  phoneNumber: string;
  profileImageUrl: string;
  dateOfBirth: string | null;
  gender: string;
}

export interface UserProfileUpdateInput {
  userProfileId: number;
  userId: number;
  firstName: string;
  lastName: string;
  displayName: string;
  phoneNumber: string;
  profileImageUrl: string;
  dateOfBirth: string | null;
  gender: string;
}

export interface UserFormValues {
  name: string;
  email: string;
  password: string;
  roleId: string;
  isActive: boolean;
}

export interface UserFormErrors {
  name?: string;
  email?: string;
  password?: string;
  roleId?: string;
}

export interface RoleFormValues {
  roleName: string;
  description: string;
  isActive: boolean;
}

export interface RoleFormErrors {
  roleName?: string;
  description?: string;
}

export interface UserProfileFormValues {
  userId: string;
  displayName: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  profileImageUrl: string;
  dateOfBirth: string;
  gender: string;
}

export interface UserProfileFormErrors {
  userId?: string;
  displayName?: string;
  phoneNumber?: string;
  profileImageUrl?: string;
  gender?: string;
}
