import { normalizeException } from "@/Exception/Service/exception-service";
import type { ApiException } from "@/Exception/Types";

type ExceptionListener = (exception: ApiException) => void;

const exceptionListeners = new Set<ExceptionListener>();

export const publishException = (error: unknown) => {
  const exception = normalizeException(error);

  exceptionListeners.forEach((listener) => listener(exception));

  return exception;
};

export const subscribeToExceptions = (listener: ExceptionListener) => {
  exceptionListeners.add(listener);

  return () => {
    exceptionListeners.delete(listener);
  };
};
