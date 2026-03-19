import type { AppEnvironment } from "@/CommonLibrary/types";

const DEFAULT_API_BASE_URL = "";
const DEFAULT_APP_NAME = "Student PMS Frontend";
const DEFAULT_REQUEST_TIMEOUT_MS = 15000;

const normalizeBaseUrl = (value: string) => value.replace(/\/+$/, "");

const parseTimeout = (value: string | undefined, fallbackValue: number) => {
  const parsedValue = Number(value);

  if (!Number.isFinite(parsedValue) || parsedValue <= 0) {
    return fallbackValue;
  }

  return parsedValue;
};

export const appEnvironment: Readonly<AppEnvironment> = Object.freeze({
  apiBaseUrl: normalizeBaseUrl(
    import.meta.env.VITE_API_BASE_URL ?? DEFAULT_API_BASE_URL,
  ),
  appName: import.meta.env.VITE_APP_NAME?.trim() || DEFAULT_APP_NAME,
  requestTimeoutMs: parseTimeout(
    import.meta.env.VITE_REQUEST_TIMEOUT_MS,
    DEFAULT_REQUEST_TIMEOUT_MS,
  ),
});
