import { useEffect, useState } from "react";
import { Link, useLocation, useNavigate } from "react-router";
import { useAutoDismissState } from "@/CommonLibrary/hooks/useAutoDismissState";
import Alert from "@/components/ui/alert/Alert";
import Label from "@/components/form/Label";
import Input from "@/components/form/input/InputField";
import Checkbox from "@/components/form/input/Checkbox";
import Button from "@/components/ui/button/Button";
import { ChevronLeftIcon, EyeCloseIcon, EyeIcon } from "@/icons";
import { useAuth } from "@/AuthServices/components/AuthProvider";
import {
  getAuthFormFeedback,
  validateSignInForm,
} from "@/AuthServices/services";
import type {
  AuthPageNotice,
  AuthRouteState,
  SignInFormErrors,
  SignInFormValues,
} from "@/AuthServices/types";

interface InlineFeedback {
  variant: "success" | "error" | "warning" | "info";
  title: string;
  message: string;
  detailLines?: string[];
}

const SignInForm = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const { login } = useAuth();
  const [showPassword, setShowPassword] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [values, setValues] = useState<SignInFormValues>({
    email: "",
    password: "",
    rememberMe: true,
  });
  const [errors, setErrors] = useState<SignInFormErrors>({});
  const [feedback, setFeedback] = useAutoDismissState<InlineFeedback>(null);

  const routeState = (location.state as AuthRouteState | null) ?? null;
  const notice = routeState?.notice as AuthPageNotice | undefined;
  const redirectTo = routeState?.from || "/";

  useEffect(() => {
    if (notice) {
      setFeedback({
        variant: notice.variant,
        title: notice.title,
        message: notice.message,
      });
    }
  }, [notice]);

  const updateField = <TField extends keyof SignInFormValues>(
    field: TField,
    value: SignInFormValues[TField],
  ) => {
    setValues((currentValue) => ({
      ...currentValue,
      [field]: value,
    }));

    setErrors((currentErrors) => ({
      ...currentErrors,
      [field]: undefined,
      form: undefined,
    }));
  };

  const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const validationErrors = validateSignInForm(values);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      setFeedback(null);
      return;
    }

    setIsSubmitting(true);
    setFeedback(null);

    try {
      await login(
        {
          email: values.email.trim(),
          password: values.password,
        },
        values.rememberMe ? "local" : "session",
      );

      navigate(redirectTo, { replace: true });
    } catch (error) {
      const authFeedback = getAuthFormFeedback(error);

      setErrors({
        email: authFeedback.fieldErrors.email,
        password: authFeedback.fieldErrors.password,
        form: authFeedback.message,
      });
      setFeedback({
        variant: "error",
        title: "Sign in failed",
        message: authFeedback.message,
        detailLines: authFeedback.detailLines,
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex flex-1 flex-col">
      <div className="mx-auto w-full max-w-md pt-10">
        <Link
          to="/"
          className="inline-flex items-center text-sm text-gray-500 transition-colors hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300"
        >
          <ChevronLeftIcon className="size-5" />
          Back to dashboard
        </Link>
      </div>
      <div className="mx-auto flex w-full max-w-md flex-1 flex-col justify-center">
        <div className="mb-8">
          <h1 className="mb-2 text-title-sm font-semibold text-gray-800 dark:text-white/90 sm:text-title-md">
            Sign in to Student PMS
          </h1>
          <p className="text-sm text-gray-500 dark:text-gray-400">
            Use your registered email and password to access the project
            management workspace.
          </p>
        </div>

        {feedback && (
          <div className="mb-6 space-y-3">
            <Alert
              variant={feedback.variant}
              title={feedback.title}
              message={feedback.message}
            />
            {feedback.detailLines && feedback.detailLines.length > 0 && (
              <div className="rounded-2xl border border-gray-200 bg-white p-4 text-sm text-gray-600 shadow-theme-xs dark:border-gray-800 dark:bg-gray-900 dark:text-gray-300">
                <ul className="space-y-2">
                  {feedback.detailLines.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                </ul>
              </div>
            )}
          </div>
        )}

        <form className="space-y-6" onSubmit={handleSubmit}>
          <div>
            <Label htmlFor="signin-email">
              Email <span className="text-error-500">*</span>
            </Label>
            <Input
              id="signin-email"
              name="email"
              type="email"
              placeholder="name@college.edu"
              value={values.email}
              onChange={(event) => updateField("email", event.target.value)}
              error={Boolean(errors.email)}
              hint={errors.email}
            />
          </div>

          <div>
            <Label htmlFor="signin-password">
              Password <span className="text-error-500">*</span>
            </Label>
            <div className="relative">
              <Input
                id="signin-password"
                name="password"
                type={showPassword ? "text" : "password"}
                placeholder="Enter your password"
                value={values.password}
                onChange={(event) => updateField("password", event.target.value)}
                error={Boolean(errors.password)}
                hint={errors.password}
              />
              <button
                type="button"
                onClick={() => setShowPassword((currentValue) => !currentValue)}
                className="absolute right-4 top-1/2 z-30 -translate-y-1/2"
                aria-label={showPassword ? "Hide password" : "Show password"}
              >
                {showPassword ? (
                  <EyeIcon className="size-5 fill-gray-500 dark:fill-gray-400" />
                ) : (
                  <EyeCloseIcon className="size-5 fill-gray-500 dark:fill-gray-400" />
                )}
              </button>
            </div>
          </div>

          <div className="flex items-center justify-between gap-4">
            <div className="flex items-center gap-3">
              <Checkbox
                checked={values.rememberMe}
                onChange={(checked) => updateField("rememberMe", checked)}
              />
              <span className="text-theme-sm text-gray-700 dark:text-gray-400">
                Keep me logged in
              </span>
            </div>
            <span className="text-sm text-gray-400">
              Password reset is not wired yet.
            </span>
          </div>

          <Button className="w-full" size="sm" disabled={isSubmitting}>
            {isSubmitting ? "Signing in..." : "Sign in"}
          </Button>
        </form>

        <div className="mt-6">
          <p className="text-center text-sm font-normal text-gray-700 dark:text-gray-400 sm:text-start">
            Don&apos;t have an account?{" "}
            <Link
              to="/signup"
              className="text-brand-500 hover:text-brand-600 dark:text-brand-400"
            >
              Create one
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default SignInForm;
