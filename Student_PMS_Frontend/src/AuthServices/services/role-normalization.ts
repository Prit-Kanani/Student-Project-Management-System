const ROLE_ALIAS_MAP: Record<string, string> = {
  globleadmin: "Admin",
  globaladmin: "Admin",
};

export const normalizeRoleName = (role: string) => {
  const normalizedValue = role.trim();

  if (!normalizedValue) {
    return "";
  }

  return ROLE_ALIAS_MAP[normalizedValue.toLowerCase()] ?? normalizedValue;
};

export const normalizeRoleNames = (roles: readonly string[]) =>
  Array.from(
    new Set(
      roles
        .map((role) => normalizeRoleName(role))
        .filter((role): role is string => role.length > 0),
    ),
  );
