export const HTTP_CLIENT_ERROR_NAME = "HttpClientError" as const;

export interface ApiErrorPayload {
  title?: string;
  message?: string;
  detail?: string;
  status?: number;
  statusCode?: number;
  code?: string;
  sqlErrorNumber?: number;
  SqlErrorNumber?: number;
  errors?: Record<string, string[]>;
}

export interface HttpClientError {
  name: typeof HTTP_CLIENT_ERROR_NAME;
  message: string;
  statusCode?: number;
  code?: string;
  payload?: ApiErrorPayload | null;
  details?: unknown;
}
