import { appEnvironment } from "@/CommonLibrary/functions/environment";
import { buildQueryString } from "@/CommonLibrary/functions/query-string";
import {
  HTTP_CLIENT_ERROR_NAME,
  type ApiErrorPayload,
  type ApiErrorInterceptor,
  type ApiRequestOptions,
  type HttpClientError,
} from "@/CommonLibrary/types";

type AccessTokenResolver = () => string | null | undefined;

let accessTokenResolver: AccessTokenResolver | null = null;
const apiErrorInterceptors = new Set<ApiErrorInterceptor>();
const MAX_RATE_LIMIT_RETRIES = 1;
const DEFAULT_RATE_LIMIT_RETRY_MS = 1100;

const createHttpClientError = (
  message: string,
  statusCode?: number,
  payload?: ApiErrorPayload | null,
  details?: unknown,
): HttpClientError => ({
  name: HTTP_CLIENT_ERROR_NAME,
  message,
  statusCode,
  payload,
  details,
});

const createTimeoutSignal = (signal?: AbortSignal) => {
  if (signal) {
    return {
      signal,
      cleanup: () => undefined,
    };
  }

  const controller = new AbortController();
  const timeoutId = setTimeout(() => controller.abort(), appEnvironment.requestTimeoutMs);

  return {
    signal: controller.signal,
    cleanup: () => clearTimeout(timeoutId),
  };
};

const buildHeaders = (options: ApiRequestOptions<unknown>) => {
  const headers = new Headers(options.headers);
  const hasJsonBody = options.body !== undefined && !(options.body instanceof FormData);

  if (hasJsonBody && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }

  if (!headers.has("Accept")) {
    headers.set("Accept", "application/json");
  }

  if (options.authMode !== "public") {
    const accessToken = accessTokenResolver?.();

    if (accessToken) {
      headers.set("Authorization", `Bearer ${accessToken}`);
    }
  }

  return headers;
};

const parseJsonResponse = async <TResponse>(response: Response) => {
  const contentType = response.headers.get("content-type")?.toLowerCase() ?? "";

  if (!contentType.includes("application/json")) {
    return null;
  }

  return (await response.json()) as TResponse;
};

const wait = (durationMs: number) =>
  new Promise((resolve) => setTimeout(resolve, durationMs));

const getRateLimitRetryDelay = (response: Response) => {
  const retryAfterHeader = response.headers.get("retry-after");

  if (!retryAfterHeader) {
    return DEFAULT_RATE_LIMIT_RETRY_MS;
  }

  const retryAfterSeconds = Number(retryAfterHeader);

  if (Number.isFinite(retryAfterSeconds) && retryAfterSeconds > 0) {
    return retryAfterSeconds * 1000;
  }

  return DEFAULT_RATE_LIMIT_RETRY_MS;
};

const isHttpClientError = (error: unknown): error is HttpClientError =>
  typeof error === "object" &&
  error !== null &&
  "name" in error &&
  error.name === HTTP_CLIENT_ERROR_NAME;

const runErrorInterceptors = async (
  error: unknown,
  request: ApiRequestOptions<unknown>,
) => {
  let nextError = error;

  for (const interceptor of apiErrorInterceptors) {
    nextError = await interceptor(nextError, { request });
  }

  return nextError;
};

export const registerAccessTokenResolver = (resolver: AccessTokenResolver) => {
  accessTokenResolver = resolver;
};

export const clearAccessTokenResolver = () => {
  accessTokenResolver = null;
};

export const registerApiErrorInterceptor = (interceptor: ApiErrorInterceptor) => {
  apiErrorInterceptors.add(interceptor);

  return () => {
    apiErrorInterceptors.delete(interceptor);
  };
};

export const apiRequest = async <TResponse, TBody = unknown>(
  options: ApiRequestOptions<TBody>,
  attempt = 0,
): Promise<TResponse> => {
  const { signal, cleanup } = createTimeoutSignal(options.signal);

  try {
    const response = await fetch(
      `${appEnvironment.apiBaseUrl}${options.path}${buildQueryString(options.query)}`,
      {
        method: options.method ?? "GET",
        headers: buildHeaders(options as ApiRequestOptions<unknown>),
        body:
          options.body === undefined
            ? undefined
            : options.body instanceof FormData
              ? options.body
              : JSON.stringify(options.body),
        signal,
      },
    );

    if (response.status === 429 && attempt < MAX_RATE_LIMIT_RETRIES) {
      await wait(getRateLimitRetryDelay(response));
      return apiRequest(options, attempt + 1);
    }

    const payload = await parseJsonResponse<TResponse | ApiErrorPayload>(response);

    if (!response.ok) {
      const errorPayload = payload as ApiErrorPayload | null;

      throw createHttpClientError(
        errorPayload?.message ??
          errorPayload?.detail ??
          errorPayload?.title ??
          "The request failed.",
        response.status,
        errorPayload,
        errorPayload,
      );
    }

    if (response.status === 204) {
      return undefined as TResponse;
    }

    return (payload as TResponse | null) ?? (undefined as TResponse);
  } catch (error) {
    let nextError = error;

    if (error instanceof DOMException && error.name === "AbortError") {
      nextError = createHttpClientError(
        "The server did not respond in time.",
        503,
      );
    }

    if (isHttpClientError(nextError)) {
      throw await runErrorInterceptors(
        nextError,
        options as ApiRequestOptions<unknown>,
      );
    }

    throw await runErrorInterceptors(
      createHttpClientError(
        error instanceof Error
          ? error.message
          : "An unexpected network error occurred.",
        500,
        null,
        error,
      ),
      options as ApiRequestOptions<unknown>,
    );
  } finally {
    cleanup();
  }
};
