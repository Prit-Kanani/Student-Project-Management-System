import { apiRequest } from "@/CommonLibrary/functions";
import type { ApiRequestOptions } from "@/CommonLibrary/types";

export const projectGroupServicesRequest = <TResponse, TBody = unknown>(
  options: ApiRequestOptions<TBody>,
) =>
  apiRequest<TResponse, TBody>({
    ...options,
    skipGlobalExceptionHandler: true,
  });
