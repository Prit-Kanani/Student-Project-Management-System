import { getExceptionDetailLines, normalizeException } from "@/Exception/Service";

export interface ExceptionFeedback {
  message: string;
  detailLines: string[];
}

export const formatDateTime = (value: string | null) => {
  if (!value) {
    return "Not available";
  }

  const parsedDate = new Date(value);
  if (Number.isNaN(parsedDate.getTime())) {
    return value;
  }

  return parsedDate.toLocaleString();
};

export const formatOptionalText = (
  value: string | null | undefined,
  fallback = "Not available",
) => {
  if (!value) {
    return fallback;
  }

  const trimmedValue = value.trim();
  return trimmedValue.length > 0 ? trimmedValue : fallback;
};

export const getExceptionFeedback = (error: unknown): ExceptionFeedback => {
  const exception = normalizeException(error);

  return {
    message: exception.message,
    detailLines: getExceptionDetailLines(exception),
  };
};
