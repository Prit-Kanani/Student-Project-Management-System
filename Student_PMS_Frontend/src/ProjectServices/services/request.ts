import { apiRequest } from "@/CommonLibrary/functions";
import type { ApiRequestOptions } from "@/CommonLibrary/types";

export const projectServicesRequest = <TResponse, TBody = unknown>(
  options: ApiRequestOptions<TBody>,
) =>
  apiRequest<TResponse, TBody>({
    ...options,
    skipGlobalExceptionHandler: true,
  });
