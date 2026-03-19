import { Link } from "react-router";
import { AlertHexaIcon, CloseIcon, ErrorHexaIcon, InfoIcon } from "@/icons";
import {
  getExceptionDetailLines,
  getExceptionPageConfig,
  getExceptionRoute,
  getExceptionTone,
} from "@/Exception/Service";
import type { ApiException } from "@/Exception/Types";

interface GlobalExceptionAlertProps {
  exception: ApiException;
  onDismiss: () => void;
}

const toneStyles = {
  error: {
    container:
      "border-error-200 bg-white dark:border-error-500/20 dark:bg-gray-900",
    badge:
      "bg-error-50 text-error-700 dark:bg-error-500/10 dark:text-error-300",
    icon: "bg-error-50 text-error-600 dark:bg-error-500/10 dark:text-error-300",
  },
  warning: {
    container:
      "border-warning-200 bg-white dark:border-warning-500/20 dark:bg-gray-900",
    badge:
      "bg-warning-50 text-warning-700 dark:bg-warning-500/10 dark:text-warning-300",
    icon: "bg-warning-50 text-warning-600 dark:bg-warning-500/10 dark:text-warning-300",
  },
  info: {
    container:
      "border-blue-light-200 bg-white dark:border-blue-light-500/20 dark:bg-gray-900",
    badge:
      "bg-blue-light-50 text-blue-light-700 dark:bg-blue-light-500/10 dark:text-blue-light-300",
    icon: "bg-blue-light-50 text-blue-light-600 dark:bg-blue-light-500/10 dark:text-blue-light-300",
  },
};

const toneIcons = {
  error: <ErrorHexaIcon className="size-5" />,
  warning: <AlertHexaIcon className="size-5" />,
  info: <InfoIcon className="size-5" />,
};

const GlobalExceptionAlert = ({
  exception,
  onDismiss,
}: GlobalExceptionAlertProps) => {
  const tone = getExceptionTone(exception.statusCode);
  const pageConfig = getExceptionPageConfig(exception.statusCode);
  const detailLines = getExceptionDetailLines(exception);

  return (
    <div className="fixed right-4 top-20 z-[9999] w-[calc(100%-2rem)] max-w-md lg:right-6 lg:top-24">
      <div
        className={`rounded-3xl border p-5 shadow-theme-lg ${toneStyles[tone].container}`}
      >
        <div className="flex items-start gap-4">
          <div className={`rounded-2xl p-3 ${toneStyles[tone].icon}`}>
            {toneIcons[tone]}
          </div>
          <div className="min-w-0 flex-1">
            <div className="flex items-start justify-between gap-3">
              <div>
                <span
                  className={`inline-flex rounded-full px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.22em] ${toneStyles[tone].badge}`}
                >
                  HTTP {exception.statusCode}
                </span>
                <h3 className="mt-3 text-base font-semibold text-gray-900 dark:text-white">
                  {pageConfig.title}
                </h3>
              </div>
              <button
                type="button"
                onClick={onDismiss}
                className="rounded-full p-2 text-gray-400 transition hover:bg-gray-100 hover:text-gray-700 dark:hover:bg-gray-800 dark:hover:text-gray-200"
                aria-label="Dismiss error message"
              >
                <CloseIcon className="size-4" />
              </button>
            </div>

            <p className="mt-2 text-sm leading-6 text-gray-600 dark:text-gray-400">
              {exception.message || pageConfig.description}
            </p>

            {detailLines.length > 0 && (
              <ul className="mt-4 space-y-2 text-xs text-gray-500 dark:text-gray-400">
                {detailLines.map((line) => (
                  <li
                    key={line}
                    className="rounded-xl border border-gray-200 bg-gray-50 px-3 py-2 dark:border-gray-800 dark:bg-gray-800/60"
                  >
                    {line}
                  </li>
                ))}
              </ul>
            )}

            <div className="mt-4 flex items-center gap-3">
              <Link
                to={getExceptionRoute(exception.statusCode)}
                className="inline-flex items-center rounded-xl bg-brand-500 px-4 py-2 text-sm font-medium text-white transition hover:bg-brand-600"
              >
                Open error page
              </Link>
              <button
                type="button"
                onClick={onDismiss}
                className="inline-flex items-center rounded-xl border border-gray-300 px-4 py-2 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:text-gray-300 dark:hover:bg-gray-800"
              >
                Dismiss
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default GlobalExceptionAlert;
