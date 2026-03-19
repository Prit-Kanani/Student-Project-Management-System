import { registerApiErrorInterceptor } from "@/CommonLibrary/functions";
import { normalizeException, publishException } from "@/Exception/Service";

let removeInterceptor: (() => void) | null = null;

export const installGlobalApiExceptionInterceptor = () => {
  if (removeInterceptor) {
    return removeInterceptor;
  }

  removeInterceptor = registerApiErrorInterceptor((error, context) => {
    const exception = normalizeException(error);

    if (!context.request.skipGlobalExceptionHandler) {
      publishException(exception);
    }

    return exception;
  });

  return removeInterceptor;
};
