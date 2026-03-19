import { getExceptionDetailLines, normalizeException } from "@/Exception/Service";
import type {
  SignInFormErrors,
  SignInFormValues,
  SignUpFormErrors,
  SignUpFormValues,
} from "@/AuthServices/types";

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

const normalizeFieldKey = (fieldName: string) =>
  fieldName.toLowerCase().replace(/[^a-z0-9]/g, "");

const resolveValidationSource = (details: unknown) => {
  if (!details || typeof details !== "object") {
    return undefined;
  }

  if ("payload" in details && details.payload && typeof details.payload === "object") {
    return details.payload as Record<string, unknown>;
  }

  return details as Record<string, unknown>;
};

const getFieldErrorsFromException = (error: unknown) => {
  const exception = normalizeException(error);
  const validationSource = resolveValidationSource(exception.details);
  const validationErrors = validationSource?.errors;

  if (!validationErrors || typeof validationErrors !== "object") {
    return {};
  }

  return Object.entries(validationErrors).reduce<Record<string, string>>(
    (accumulator, [key, value]) => {
      if (!Array.isArray(value)) {
        return accumulator;
      }

      const firstMessage = value.find(
        (message): message is string => typeof message === "string" && message.length > 0,
      );

      if (firstMessage) {
        accumulator[normalizeFieldKey(key)] = firstMessage;
      }

      return accumulator;
    },
    {},
  );
};

export const validateSignInForm = (
  values: SignInFormValues,
): SignInFormErrors => {
  const errors: SignInFormErrors = {};

  if (!values.email.trim()) {
    errors.email = "Email is required.";
  } else if (!emailPattern.test(values.email.trim())) {
    errors.email = "Enter a valid email address.";
  }

  if (!values.password) {
    errors.password = "Password is required.";
  }

  return errors;
};

export const validateSignUpForm = (
  values: SignUpFormValues,
): SignUpFormErrors => {
  const errors: SignUpFormErrors = {};

  if (!values.name.trim()) {
    errors.name = "Name is required.";
  } else if (values.name.trim().length < 3) {
    errors.name = "Name must be at least 3 characters long.";
  } else if (values.name.trim().length > 50) {
    errors.name = "Name must not exceed 50 characters.";
  }

  if (!values.email.trim()) {
    errors.email = "Email is required.";
  } else if (!emailPattern.test(values.email.trim())) {
    errors.email = "Enter a valid email address.";
  }

  if (!values.password) {
    errors.password = "Password is required.";
  } else if (values.password.length < 8) {
    errors.password = "Password must be at least 8 characters long.";
  } else if (!/[A-Z]/.test(values.password)) {
    errors.password = "Password must contain at least one uppercase letter.";
  } else if (!/[a-z]/.test(values.password)) {
    errors.password = "Password must contain at least one lowercase letter.";
  } else if (!/[0-9]/.test(values.password)) {
    errors.password = "Password must contain at least one digit.";
  } else if (!/[^a-zA-Z0-9]/.test(values.password)) {
    errors.password = "Password must contain at least one special character.";
  }

  if (!values.confirmPassword) {
    errors.confirmPassword = "Confirm your password.";
  } else if (values.password !== values.confirmPassword) {
    errors.confirmPassword = "Passwords do not match.";
  }

  if (!values.roleId) {
    errors.roleId = "Role is required.";
  }

  if (!values.acceptTerms) {
    errors.acceptTerms = "Accept the terms before registering.";
  }

  return errors;
};

export const getAuthFormFeedback = (error: unknown) => {
  const exception = normalizeException(error);
  const fieldErrors = getFieldErrorsFromException(error);

  return {
    message: exception.message,
    detailLines: getExceptionDetailLines(exception),
    fieldErrors,
  };
};
