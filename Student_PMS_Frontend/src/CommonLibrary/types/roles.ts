export const USER_ROLES = ["Admin", "Faculty", "Student"] as const;

export type UserRole = (typeof USER_ROLES)[number];
