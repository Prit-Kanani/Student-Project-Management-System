import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router";
import { useAutoDismissState } from "@/CommonLibrary/hooks/useAutoDismissState";
import Alert from "@/components/ui/alert/Alert";
import Button from "@/components/ui/button/Button";
import Checkbox from "@/components/form/input/Checkbox";
import Input from "@/components/form/input/InputField";
import Label from "@/components/form/Label";
import { ChevronLeftIcon, EyeCloseIcon, EyeIcon } from "@/icons";
import { useAuth } from "@/AuthServices/components/AuthProvider";
import {
  getAuthFormFeedback,
  resolvePublicRegistrationRole,
  validateSignUpForm,
} from "@/AuthServices/services";
import type {
  PublicRegistrationRole,
  SignUpFormErrors,
  SignUpFormValues,
} from "@/AuthServices/types";

interface InlineFeedback {
  variant: "success" | "error" | "warning" | "info";
  title: string;
  message: string;
  detailLines?: string[];
}

const SignUpForm = () => {
  const navigate = useNavigate();
  const { register } = useAuth();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isLoadingRole, setIsLoadingRole] = useState(true);
  const [values, setValues] = useState<SignUpFormValues>({
    name: "",
    email: "",
    password: "",
    confirmPassword: "",
    roleId: "",
    acceptTerms: false,
  });
  const [errors, setErrors] = useState<SignUpFormErrors>({});
  const [feedback, setFeedback] = useAutoDismissState<InlineFeedback>(null);
  const [registrationRole, setRegistrationRole] =
    useState<PublicRegistrationRole | null>(null);

  useEffect(() => {
    let isMounted = true;

    const loadRegistrationRole = async () => {
      setIsLoadingRole(true);

      try {
        const resolvedRole = await resolvePublicRegistrationRole();

        if (!isMounted) {
          return;
        }

        setRegistrationRole(resolvedRole);
        setValues((currentValue) => ({
          ...currentValue,
          roleId: String(resolvedRole.id),
        }));
        setErrors((currentErrors) => ({
          ...currentErrors,
          roleId: undefined,
          form: undefined,
        }));
      } finally {
        if (isMounted) {
          setIsLoadingRole(false);
        }
      }
    };

    void loadRegistrationRole();

    return () => {
      isMounted = false;
    };
  }, []);

  const updateField = <TField extends keyof SignUpFormValues>(
    field: TField,
    value: SignUpFormValues[TField],
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

    const validationErrors = validateSignUpForm(values);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      setFeedback(null);
      return;
    }

    setIsSubmitting(true);
    setFeedback(null);

    try {
      await register({
        name: values.name.trim(),
        email: values.email.trim(),
        password: values.password,
        roleId: Number(values.roleId),
        isActive: true,
      });

      navigate("/signin", {
        replace: true,
        state: {
          notice: {
            variant: "success",
            title: "Registration complete",
            message:
              "Your account has been created successfully. Sign in to continue.",
          },
        },
      });
    } catch (error) {
      const authFeedback = getAuthFormFeedback(error);

      setErrors({
        name: authFeedback.fieldErrors.name,
        email: authFeedback.fieldErrors.email,
        password: authFeedback.fieldErrors.password,
        roleId: authFeedback.fieldErrors.roleid,
        form: authFeedback.message,
      });
      setFeedback({
        variant: "error",
        title: "Registration failed",
        message: authFeedback.message,
        detailLines: authFeedback.detailLines,
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className="flex w-full flex-1 flex-col overflow-y-auto no-scrollbar lg:w-1/2">
      <div className="mx-auto mb-5 w-full max-w-md sm:pt-10">
        <Link
          to="/signin"
          className="inline-flex items-center text-sm text-gray-500 transition-colors hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300"
        >
          <ChevronLeftIcon className="size-5" />
          Back to sign in
        </Link>
      </div>

      <div className="mx-auto flex w-full max-w-md flex-1 flex-col justify-center">
        <div className="mb-8">
          <h1 className="mb-2 text-title-sm font-semibold text-gray-800 dark:text-white/90 sm:text-title-md">
            Create your Student PMS account
          </h1>
          <p className="text-sm text-gray-500 dark:text-gray-400">
            Register with your institutional details. Public sign-up creates a
            Student account automatically.
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

        <form className="space-y-5" onSubmit={handleSubmit}>
          <div>
            <Label htmlFor="signup-name">
              Full Name <span className="text-error-500">*</span>
            </Label>
            <Input
              id="signup-name"
              name="name"
              type="text"
              placeholder="Enter your full name"
              value={values.name}
              onChange={(event) => updateField("name", event.target.value)}
              error={Boolean(errors.name)}
              hint={errors.name}
            />
          </div>

          <div>
            <Label htmlFor="signup-email">
              Email <span className="text-error-500">*</span>
            </Label>
            <Input
              id="signup-email"
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
            <Label htmlFor="signup-role">
              Account Role <span className="text-error-500">*</span>
            </Label>
            <div
              id="signup-role"
              className={`rounded-2xl border px-4 py-3 shadow-theme-xs dark:bg-gray-900 ${
                errors.roleId
                  ? "border-error-500"
                  : "border-gray-300 bg-gray-50 dark:border-gray-700 dark:bg-gray-900/50"
              }`}
            >
              <div className="flex items-center justify-between gap-4">
                <div>
                  <p className="text-sm font-medium text-gray-800 dark:text-white/90">
                    {registrationRole?.name ?? "Student"}
                  </p>
                  <p className="mt-1 text-xs text-gray-500 dark:text-gray-400">
                    {isLoadingRole
                      ? "Resolving the Student role from the backend..."
                      : registrationRole
                        ? `Role ID ${registrationRole.id} will be submitted with your registration.`
                        : "The Student role will be applied automatically."}
                  </p>
                </div>
                <span className="rounded-full bg-brand-50 px-3 py-1 text-xs font-semibold uppercase tracking-[0.16em] text-brand-600 dark:bg-brand-500/10 dark:text-brand-300">
                  Locked
                </span>
              </div>
            </div>
            <p
              className={`mt-1.5 text-xs ${
                errors.roleId ? "text-error-500" : "text-gray-500"
              }`}
            >
              {errors.roleId ||
                registrationRole?.description ||
                "Self-registration is restricted to Student accounts."}
            </p>
          </div>

          <div>
            <Label htmlFor="signup-password">
              Password <span className="text-error-500">*</span>
            </Label>
            <div className="relative">
              <Input
                id="signup-password"
                name="password"
                type={showPassword ? "text" : "password"}
                placeholder="Create a strong password"
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

          <div>
            <Label htmlFor="signup-confirm-password">
              Confirm Password <span className="text-error-500">*</span>
            </Label>
            <div className="relative">
              <Input
                id="signup-confirm-password"
                name="confirmPassword"
                type={showConfirmPassword ? "text" : "password"}
                placeholder="Re-enter your password"
                value={values.confirmPassword}
                onChange={(event) =>
                  updateField("confirmPassword", event.target.value)
                }
                error={Boolean(errors.confirmPassword)}
                hint={errors.confirmPassword}
              />
              <button
                type="button"
                onClick={() =>
                  setShowConfirmPassword((currentValue) => !currentValue)
                }
                className="absolute right-4 top-1/2 z-30 -translate-y-1/2"
                aria-label={
                  showConfirmPassword ? "Hide password" : "Show password"
                }
              >
                {showConfirmPassword ? (
                  <EyeIcon className="size-5 fill-gray-500 dark:fill-gray-400" />
                ) : (
                  <EyeCloseIcon className="size-5 fill-gray-500 dark:fill-gray-400" />
                )}
              </button>
            </div>
          </div>

          <div className="space-y-2">
            <div className="flex items-center gap-3">
              <Checkbox
                className="h-5 w-5"
                checked={values.acceptTerms}
                onChange={(checked) => updateField("acceptTerms", checked)}
              />
              <p className="text-sm font-normal text-gray-500 dark:text-gray-400">
                I confirm the provided account details are correct and I accept
                the project terms.
              </p>
            </div>
            {errors.acceptTerms && (
              <p className="text-xs text-error-500">{errors.acceptTerms}</p>
            )}
          </div>

          <Button
            className="w-full"
            size="sm"
            disabled={isSubmitting || isLoadingRole}
          >
            {isLoadingRole
              ? "Resolving student role..."
              : isSubmitting
                ? "Creating account..."
                : "Create account"}
          </Button>
        </form>

        <div className="mt-6">
          <p className="text-center text-sm font-normal text-gray-700 dark:text-gray-400 sm:text-start">
            Already have an account?{" "}
            <Link
              to="/signin"
              className="text-brand-500 hover:text-brand-600 dark:text-brand-400"
            >
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default SignUpForm;
