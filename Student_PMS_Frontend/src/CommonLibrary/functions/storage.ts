const hasBrowserStorage =
  typeof window !== "undefined" && typeof window.localStorage !== "undefined";

export const storageKeys = {
  accessToken: "student-pms.access-token",
  activeRole: "student-pms.active-role",
  currentUser: "student-pms.current-user",
  studentRegistrationRoleId: "student-pms.registration.student-role-id",
} as const;

export const browserStorage = {
  get: (key: string) => {
    if (!hasBrowserStorage) {
      return null;
    }

    return window.localStorage.getItem(key);
  },
  set: (key: string, value: string) => {
    if (!hasBrowserStorage) {
      return;
    }

    window.localStorage.setItem(key, value);
  },
  remove: (key: string) => {
    if (!hasBrowserStorage) {
      return;
    }

    window.localStorage.removeItem(key);
  },
  clearNamespace: (namespace: string) => {
    if (!hasBrowserStorage) {
      return;
    }

    Object.keys(window.localStorage)
      .filter((key) => key.startsWith(namespace))
      .forEach((key) => window.localStorage.removeItem(key));
  },
};
