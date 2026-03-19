import type {
  AuthPersistenceMode,
  AuthSession,
} from "@/AuthServices/types";

const AUTH_SESSION_STORAGE_KEY = "student-pms.auth-session";

const hasBrowserStorage =
  typeof window !== "undefined" &&
  typeof window.localStorage !== "undefined" &&
  typeof window.sessionStorage !== "undefined";

const getStorage = (persistence: AuthPersistenceMode) => {
  if (!hasBrowserStorage) {
    return null;
  }

  return persistence === "local" ? window.localStorage : window.sessionStorage;
};

const parseStoredSession = (value: string | null): AuthSession | null => {
  if (!value) {
    return null;
  }

  try {
    const parsedValue = JSON.parse(value) as AuthSession;

    if (
      typeof parsedValue !== "object" ||
      parsedValue === null ||
      typeof parsedValue.accessToken !== "string" ||
      typeof parsedValue.persistence !== "string"
    ) {
      return null;
    }

    return parsedValue;
  } catch {
    return null;
  }
};

const removeFromAllStorages = () => {
  if (!hasBrowserStorage) {
    return;
  }

  window.localStorage.removeItem(AUTH_SESSION_STORAGE_KEY);
  window.sessionStorage.removeItem(AUTH_SESSION_STORAGE_KEY);
};

export const authStorage = {
  saveSession: (session: AuthSession) => {
    const storage = getStorage(session.persistence);

    removeFromAllStorages();
    storage?.setItem(AUTH_SESSION_STORAGE_KEY, JSON.stringify(session));
  },
  getStoredSession: () => {
    if (!hasBrowserStorage) {
      return null;
    }

    return (
      parseStoredSession(window.localStorage.getItem(AUTH_SESSION_STORAGE_KEY)) ??
      parseStoredSession(window.sessionStorage.getItem(AUTH_SESSION_STORAGE_KEY))
    );
  },
  getAccessToken: () => authStorage.getStoredSession()?.accessToken ?? null,
  clearSession: () => {
    removeFromAllStorages();
  },
};
