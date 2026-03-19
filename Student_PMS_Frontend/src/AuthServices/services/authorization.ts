import type { AuthSession, AuthUser } from "@/AuthServices/types";
import { normalizeRoleName, normalizeRoleNames } from "@/AuthServices/services/role-normalization";

const getRoles = (subject: AuthSession | AuthUser | null | undefined) => {
  if (!subject) {
    return [];
  }

  return normalizeRoleNames("user" in subject ? subject.user.roles : subject.roles);
};

export const hasRole = (
  subject: AuthSession | AuthUser | null | undefined,
  role: string,
) =>
  getRoles(subject)
    .map((value) => value.toLowerCase())
    .includes(normalizeRoleName(role).toLowerCase());

export const hasAnyRole = (
  subject: AuthSession | AuthUser | null | undefined,
  roles: readonly string[],
) => roles.some((role) => hasRole(subject, role));
