import type { ApiErrorPayload, HttpClientError } from "@/CommonLibrary/types";
import {
  ApiException,
  ApiServerException,
  BadRequestException,
  DatabaseServerException,
  DatabaseTimeoutException,
  DuplicateKeyException,
  ForeignKeyViolationException,
  NotFoundException,
  UnauthorizedException,
  isApiException,
} from "@/Exception/Types";

const SUPPORTED_EXCEPTION_STATUS_CODES = [400, 401, 404, 409, 500, 503] as const;

type ExceptionTone = "error" | "warning" | "info";

export type SupportedExceptionStatusCode =
  (typeof SUPPORTED_EXCEPTION_STATUS_CODES)[number];

interface ExceptionPageConfig {
  title: string;
  description: string;
  primaryActionLabel: string;
  primaryActionPath: string;
  secondaryActionLabel: string;
  secondaryActionPath: string;
  tone: ExceptionTone;
}

const exceptionPageConfig: Record<SupportedExceptionStatusCode, ExceptionPageConfig> = {
  400: {
    title: "Bad request",
    description:
      "The server rejected this request. Check the submitted values and try again.",
    primaryActionLabel: "Return to dashboard",
    primaryActionPath: "/",
    secondaryActionLabel: "Open sign in",
    secondaryActionPath: "/signin",
    tone: "warning",
  },
  401: {
    title: "Unauthorized",
    description:
      "Authentication is required for this action. Sign in again before continuing.",
    primaryActionLabel: "Open sign in",
    primaryActionPath: "/signin",
    secondaryActionLabel: "Return to dashboard",
    secondaryActionPath: "/",
    tone: "warning",
  },
  404: {
    title: "Not found",
    description:
      "The requested resource or page could not be found in the current application state.",
    primaryActionLabel: "Return to dashboard",
    primaryActionPath: "/",
    secondaryActionLabel: "Open sign in",
    secondaryActionPath: "/signin",
    tone: "info",
  },
  409: {
    title: "Conflict detected",
    description:
      "The request conflicts with existing data. Review duplicates or dependency rules and try again.",
    primaryActionLabel: "Return to dashboard",
    primaryActionPath: "/",
    secondaryActionLabel: "Open sign in",
    secondaryActionPath: "/signin",
    tone: "warning",
  },
  500: {
    title: "Server error",
    description:
      "The application hit an internal error. The request did not complete successfully.",
    primaryActionLabel: "Return to dashboard",
    primaryActionPath: "/",
    secondaryActionLabel: "Open sign in",
    secondaryActionPath: "/signin",
    tone: "error",
  },
  503: {
    title: "Service unavailable",
    description:
      "The service is temporarily unavailable or timed out. Retry after the backend is reachable.",
    primaryActionLabel: "Return to dashboard",
    primaryActionPath: "/",
    secondaryActionLabel: "Open sign in",
    secondaryActionPath: "/signin",
    tone: "error",
  },
};

const isRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null;

const readString = (value: unknown) =>
  typeof value === "string" && value.trim().length > 0 ? value.trim() : undefined;

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

  return undefined;
};

const getPayloadDetails = (value: unknown) => {
  if (!isRecord(value)) {
    return undefined;
  }

  if ("payload" in value && isRecord(value.payload)) {
    return value.payload;
  }

  if ("details" in value && isRecord(value.details)) {
    return value.details;
  }

  return value;
};

const getStatusCodeFromValue = (value: unknown): number | undefined => {
  if (!isRecord(value)) {
    return undefined;
  }

  const directStatusCode =
    readNumber(value.statusCode) ??
    readNumber(value.status) ??
    readNumber(value.StatusCode) ??
    readNumber(value.Status);

  if (directStatusCode !== undefined) {
    return directStatusCode;
  }

  const nestedDetails = getPayloadDetails(value);

  if (!nestedDetails || nestedDetails === value) {
    return undefined;
  }

  return getStatusCodeFromValue(nestedDetails);
};

const getSqlErrorNumber = (value: unknown): number | undefined => {
  if (!isRecord(value)) {
    return undefined;
  }

  const sqlErrorNumber =
    readNumber(value.sqlErrorNumber) ?? readNumber(value.SqlErrorNumber);

  if (sqlErrorNumber !== undefined) {
    return sqlErrorNumber;
  }

  const nestedDetails = getPayloadDetails(value);

  if (!nestedDetails || nestedDetails === value) {
    return undefined;
  }

  return getSqlErrorNumber(nestedDetails);
};

const getMessageFromValue = (value: unknown): string | undefined => {
  if (!isRecord(value)) {
    return undefined;
  }

  const directMessage =
    readString(value.message) ??
    readString(value.detail) ??
    readString(value.title) ??
    readString(value.Message) ??
    readString(value.Detail) ??
    readString(value.Title);

  if (directMessage) {
    return directMessage;
  }

  const nestedDetails = getPayloadDetails(value);

  if (!nestedDetails || nestedDetails === value) {
    return undefined;
  }

  return getMessageFromValue(nestedDetails);
};

const getDefaultMessage = (statusCode?: number) => {
  if (statusCode !== undefined && isSupportedExceptionStatusCode(statusCode)) {
    return exceptionPageConfig[statusCode].description;
  }

  return exceptionPageConfig[500].description;
};

export const isSupportedExceptionStatusCode = (
  value: number,
): value is SupportedExceptionStatusCode =>
  SUPPORTED_EXCEPTION_STATUS_CODES.includes(value as SupportedExceptionStatusCode);

export const getExceptionRoute = (statusCode?: number) =>
  `/error-${
    statusCode !== undefined && isSupportedExceptionStatusCode(statusCode)
      ? statusCode
      : 500
  }`;

export const getExceptionPageConfig = (statusCode?: number) =>
  exceptionPageConfig[
    statusCode !== undefined && isSupportedExceptionStatusCode(statusCode)
      ? statusCode
      : 500
  ];

export const getExceptionTone = (statusCode?: number) =>
  getExceptionPageConfig(statusCode).tone;

export const createApiException = (
  message: string,
  statusCode?: number,
  details?: unknown,
) => {
  const sqlErrorNumber = getSqlErrorNumber(details);
  const normalizedMessage = message.trim() || getDefaultMessage(statusCode);

  switch (statusCode) {
    case 400:
      return sqlErrorNumber !== undefined
        ? new ForeignKeyViolationException(
            normalizedMessage,
            sqlErrorNumber,
            details,
          )
        : new BadRequestException(normalizedMessage, details);
    case 401:
      return new UnauthorizedException(normalizedMessage, details);
    case 404:
      return new NotFoundException(normalizedMessage, details);
    case 409:
      return new DuplicateKeyException(normalizedMessage, sqlErrorNumber, details);
    case 503:
      return new DatabaseTimeoutException(
        normalizedMessage,
        sqlErrorNumber,
        details,
      );
    case 500:
      return sqlErrorNumber !== undefined
        ? new DatabaseServerException(normalizedMessage, details)
        : new ApiServerException(normalizedMessage, details);
    default:
      return new ApiException(normalizedMessage, statusCode ?? 500, details);
  }
};

export const normalizeException = (error: unknown): ApiException => {
  if (isApiException(error)) {
    return error;
  }

  if (error instanceof Error) {
    const statusCode = getStatusCodeFromValue(error);
    const details =
      isRecord(error) && "details" in error ? error.details : undefined;

    return createApiException(
      error.message || getDefaultMessage(statusCode),
      statusCode,
      details,
    );
  }

  if (isRecord(error)) {
    const statusCode = getStatusCodeFromValue(error);
    const details =
      ("details" in error && error.details) ||
      ("payload" in error && error.payload) ||
      error;

    return createApiException(
      getMessageFromValue(error) ?? getDefaultMessage(statusCode),
      statusCode,
      details,
    );
  }

  return new ApiServerException(getDefaultMessage(500), error);
};

export const getExceptionDetailLines = (exception: ApiException) => {
  const detailSource = exception.details;

  if (!isRecord(detailSource)) {
    return [];
  }

  const payload = getPayloadDetails(detailSource) as
    | ApiErrorPayload
    | HttpClientError
    | undefined;

  if (!payload || !isRecord(payload) || !("errors" in payload) || !payload.errors) {
    return [];
  }

  return Object.values(payload.errors)
    .flatMap((messages) => messages)
    .filter((message): message is string => typeof message === "string")
    .slice(0, 4);
};
