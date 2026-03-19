export type Primitive = string | number | boolean | null | undefined;
export type QueryParamValue = Primitive | Primitive[];
export type QueryParams = Record<string, QueryParamValue>;

export type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";
export type AuthMode = "public" | "private";

export interface ApiRequestOptions<TBody = unknown> {
  path: string;
  method?: HttpMethod;
  authMode?: AuthMode;
  query?: QueryParams;
  body?: TBody;
  headers?: HeadersInit;
  signal?: AbortSignal;
  skipGlobalExceptionHandler?: boolean;
}

export interface ApiErrorInterceptorContext {
  request: ApiRequestOptions<unknown>;
}

export type ApiErrorInterceptor = (
  error: unknown,
  context: ApiErrorInterceptorContext,
) => unknown | Promise<unknown>;
