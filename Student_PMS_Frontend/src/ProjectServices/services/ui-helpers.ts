import { getExceptionDetailLines, normalizeException } from "@/Exception/Service";
import type { ProjectApprovalStatus } from "@/ProjectServices/types";

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

export const getApprovalStatusLabel = (value: boolean | null | undefined) => {
  if (value === true) {
    return "Approved";
  }

  if (value === false) {
    return "Rejected";
  }

  return "Pending";
};

export const getApprovalStatusValue = (status: ProjectApprovalStatus) => {
  if (status === "approved") {
    return true;
  }

  if (status === "rejected") {
    return false;
  }

  return null;
};

export const getApprovalStatusFromValue = (
  value: boolean | null | undefined,
): ProjectApprovalStatus => {
  if (value === true) {
    return "approved";
  }

  if (value === false) {
    return "rejected";
  }

  return "pending";
};
