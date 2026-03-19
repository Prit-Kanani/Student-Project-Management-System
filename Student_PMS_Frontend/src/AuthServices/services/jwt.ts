import type {
  AuthPersistenceMode,
  AuthSession,
  AuthUser,
  JwtPayload,
} from "@/AuthServices/types";
import { normalizeRoleName } from "@/AuthServices/services/role-normalization";

const NAME_IDENTIFIER_CLAIM =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier";
const NAME_CLAIM =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";
const ROLE_CLAIM =
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
const EMAIL_CLAIM =
  "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress";

const decodeBase64Url = (value: string) => {
  const normalizedValue = value.replace(/-/g, "+").replace(/_/g, "/");
  const paddedValue = normalizedValue.padEnd(
    normalizedValue.length + ((4 - (normalizedValue.length % 4)) % 4),
    "=",
  );

  return atob(paddedValue);
};

const parseNumber = (value: unknown) => {
  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "string" && value.trim().length > 0) {
    const parsedValue = Number(value);

    if (Number.isFinite(parsedValue)) {
      return parsedValue;
    }
  }

  return undefined;
};

const parseString = (value: unknown) =>
  typeof value === "string" && value.trim().length > 0 ? value.trim() : undefined;

const parseRoleValues = (value: unknown) => {
  if (Array.isArray(value)) {
    return value
      .filter((item): item is string => typeof item === "string")
      .map((item) => normalizeRoleName(item))
      .filter(Boolean);
  }

  const singleRole = parseString(value);

  return singleRole ? [normalizeRoleName(singleRole)] : [];
};

export const decodeJwtPayload = (token: string): JwtPayload => {
  const tokenParts = token.split(".");

  if (tokenParts.length < 2) {
    throw new Error("The authentication token format is invalid.");
  }

  return JSON.parse(decodeBase64Url(tokenParts[1])) as JwtPayload;
};

export const getAuthUserFromToken = (token: string): AuthUser => {
  const payload = decodeJwtPayload(token);
  const expiresAt = parseNumber(payload.exp);
  const roles = Array.from(
    new Set([
      ...parseRoleValues(payload.role),
      ...parseRoleValues(payload.roles),
      ...parseRoleValues(payload[ROLE_CLAIM]),
    ]),
  );

  return {
    userId:
      parseNumber(payload.sub) ?? parseNumber(payload[NAME_IDENTIFIER_CLAIM]) ?? null,
    name:
      parseString(payload[NAME_CLAIM]) ??
      parseString(payload.unique_name) ??
      parseString(payload.name) ??
      "Student PMS User",
    email: parseString(payload.email) ?? parseString(payload[EMAIL_CLAIM]) ?? "",
    roles,
    primaryRole: roles[0] ?? null,
    expiresAt: expiresAt ? expiresAt * 1000 : null,
  };
};

export const isSessionExpired = (session: AuthSession) =>
  session.user.expiresAt !== null && session.user.expiresAt <= Date.now();

export const createAuthSession = (
  accessToken: string,
  persistence: AuthPersistenceMode,
  message?: string | null,
): AuthSession => ({
  accessToken,
  persistence,
  message,
  user: getAuthUserFromToken(accessToken),
});
