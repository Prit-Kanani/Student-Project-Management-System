import {
  apiEndpoints,
  apiRequest,
  browserStorage,
  storageKeys,
} from "@/CommonLibrary/functions";
import type { OperationResultDTO } from "@/CommonLibrary/types";
import {
  createAuthSession,
  isSessionExpired,
} from "@/AuthServices/services/jwt";
import { authStorage } from "@/AuthServices/services/auth-storage";
import type {
  AuthPersistenceMode,
  AuthResponseDTO,
  AuthSession,
  LoginDTO,
  PublicRegistrationRole,
  RegisterDTO,
} from "@/AuthServices/types";
import { ApiServerException } from "@/Exception/Types";

const DEFAULT_PUBLIC_REGISTRATION_ROLE: PublicRegistrationRole = {
  id: 3,
  name: "Student",
  description: "All self-registered accounts are created as Student accounts.",
};

const isRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null;

const readString = (value: unknown) =>
  typeof value === "string" ? value.trim() : "";

const readNumber = (value: unknown) => {
  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "string" && value.trim().length > 0) {
    const parsedValue = Number(value);

    if (Number.isFinite(parsedValue)) {
      return parsedValue;
    }
  }

  return 0;
};

const normalizeRoleName = (value: string) => value.trim().toLowerCase();

const readCachedStudentRoleId = () => {
  const cachedRoleId = browserStorage.get(storageKeys.studentRegistrationRoleId);
  const parsedRoleId = Number(cachedRoleId);

  return Number.isFinite(parsedRoleId) && parsedRoleId > 0 ? parsedRoleId : null;
};

const createStudentRegistrationRole = (
  roleId: number,
): PublicRegistrationRole => ({
  ...DEFAULT_PUBLIC_REGISTRATION_ROLE,
  id: roleId,
});

export const resolvePublicRegistrationRole = async (): Promise<PublicRegistrationRole> => {
  const cachedStudentRoleId = readCachedStudentRoleId();

  if (cachedStudentRoleId) {
    return createStudentRegistrationRole(cachedStudentRoleId);
  }

  try {
    const response = await apiRequest<unknown[]>({
      path: apiEndpoints.userService.role.dropdown,
      skipGlobalExceptionHandler: true,
    });

    if (Array.isArray(response)) {
      const studentRole = response.find((roleOption) => {
        if (!isRecord(roleOption)) {
          return false;
        }

        return (
          normalizeRoleName(readString(roleOption.name ?? roleOption.Name)) ===
          "student"
        );
      });

      if (studentRole && isRecord(studentRole)) {
        const resolvedRoleId = readNumber(studentRole.id ?? studentRole.Id);

        if (resolvedRoleId > 0) {
          browserStorage.set(
            storageKeys.studentRegistrationRoleId,
            String(resolvedRoleId),
          );

          return createStudentRegistrationRole(resolvedRoleId);
        }
      }
    }
  } catch {
    // Anonymous signup cannot reach the protected role endpoint. Fall back to the known student role.
  }

  return DEFAULT_PUBLIC_REGISTRATION_ROLE;
};

export const restoreAuthSession = (): AuthSession | null => {
  const storedSession = authStorage.getStoredSession();

  if (!storedSession) {
    return null;
  }

  const normalizedSession = createAuthSession(
    storedSession.accessToken,
    storedSession.persistence,
    storedSession.message,
  );

  if (isSessionExpired(normalizedSession)) {
    authStorage.clearSession();
    return null;
  }

  authStorage.saveSession(normalizedSession);

  return normalizedSession;
};

export const loginWithCredentials = async (
  credentials: LoginDTO,
  persistence: AuthPersistenceMode = "local",
) => {
  const response = await apiRequest<AuthResponseDTO, LoginDTO>({
    path: apiEndpoints.auth.login,
    method: "POST",
    authMode: "public",
    body: credentials,
    skipGlobalExceptionHandler: true,
  });

  if (!response.token) {
    throw new ApiServerException(
      "Login succeeded but no authentication token was returned.",
    );
  }

  const session = createAuthSession(response.token, persistence, response.message);
  authStorage.saveSession(session);

  return session;
};

export const registerAccount = async (payload: RegisterDTO) => {
  await apiRequest<OperationResultDTO, Record<string, unknown>>({
    path: apiEndpoints.auth.register,
    method: "POST",
    authMode: "public",
    body: {
      name: payload.name,
      email: payload.email,
      password: payload.password,
      roleId: payload.roleId,
      isActive: payload.isActive,
    },
    skipGlobalExceptionHandler: true,
  });
};

export const logoutFromSession = () => {
  authStorage.clearSession();
};
