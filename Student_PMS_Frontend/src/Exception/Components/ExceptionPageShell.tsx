import { Link } from "react-router";
import GridShape from "@/components/common/GridShape";
import PageMeta from "@/components/common/PageMeta";
import { getExceptionTone } from "@/Exception/Service";

interface ExceptionAction {
  label: string;
  to: string;
}

interface RetryAction {
  label: string;
  onClick: () => void;
}

interface ExceptionPageShellProps {
  statusCode: number;
  title: string;
  description: string;
  primaryAction: ExceptionAction;
  secondaryAction: ExceptionAction;
  detailLines?: string[];
  retryAction?: RetryAction;
}

const toneClasses = {
  error:
    "border-error-200 bg-error-50 text-error-700 dark:border-error-500/20 dark:bg-error-500/10 dark:text-error-300",
  warning:
    "border-warning-200 bg-warning-50 text-warning-700 dark:border-warning-500/20 dark:bg-warning-500/10 dark:text-warning-300",
  info: "border-blue-light-200 bg-blue-light-50 text-blue-light-700 dark:border-blue-light-500/20 dark:bg-blue-light-500/10 dark:text-blue-light-300",
};

const ExceptionPageShell = ({
  statusCode,
  title,
  description,
  primaryAction,
  secondaryAction,
  detailLines = [],
  retryAction,
}: ExceptionPageShellProps) => {
  const tone = getExceptionTone(statusCode);

  return (
    <>
      <PageMeta
        title={`${statusCode} ${title} | Student PMS Frontend`}
        description={description}
      />
      <div className="relative flex min-h-screen items-center justify-center overflow-hidden px-6 py-10">
        <GridShape />
        <div className="relative z-10 mx-auto w-full max-w-3xl">
          <div className="rounded-[28px] border border-gray-200 bg-white p-8 text-center shadow-theme-xl dark:border-gray-800 dark:bg-gray-900 sm:p-10">
            <div
              className={`mx-auto inline-flex rounded-full border px-4 py-2 text-xs font-semibold uppercase tracking-[0.24em] ${toneClasses[tone]}`}
            >
              HTTP {statusCode}
            </div>
            <h1 className="mt-6 text-5xl font-bold tracking-tight text-gray-900 dark:text-white sm:text-6xl">
              {statusCode}
            </h1>
            <h2 className="mt-4 text-2xl font-semibold text-gray-900 dark:text-white/90 sm:text-3xl">
              {title}
            </h2>
            <p className="mx-auto mt-4 max-w-2xl text-sm leading-7 text-gray-600 dark:text-gray-400 sm:text-base">
              {description}
            </p>

            {detailLines.length > 0 && (
              <div className="mx-auto mt-8 max-w-2xl rounded-2xl border border-gray-200 bg-gray-50 p-5 text-left dark:border-gray-800 dark:bg-gray-800/40">
                <p className="text-xs font-semibold uppercase tracking-[0.18em] text-gray-500 dark:text-gray-400">
                  Error details
                </p>
                <ul className="mt-3 space-y-2 text-sm text-gray-600 dark:text-gray-300">
                  {detailLines.map((line) => (
                    <li
                      key={line}
                      className="rounded-xl bg-white px-3 py-2 dark:bg-gray-900"
                    >
                      {line}
                    </li>
                  ))}
                </ul>
              </div>
            )}

            <div className="mt-8 flex flex-col items-center justify-center gap-3 sm:flex-row">
              <Link
                to={primaryAction.to}
                className="inline-flex min-w-[190px] items-center justify-center rounded-xl bg-brand-500 px-5 py-3 text-sm font-medium text-white transition hover:bg-brand-600"
              >
                {primaryAction.label}
              </Link>
              <Link
                to={secondaryAction.to}
                className="inline-flex min-w-[190px] items-center justify-center rounded-xl border border-gray-300 bg-white px-5 py-3 text-sm font-medium text-gray-700 transition hover:bg-gray-50 dark:border-gray-700 dark:bg-gray-800 dark:text-gray-300 dark:hover:bg-gray-700/70"
              >
                {secondaryAction.label}
              </Link>
              {retryAction && (
                <button
                  type="button"
                  onClick={retryAction.onClick}
                  className="inline-flex min-w-[190px] items-center justify-center rounded-xl border border-transparent bg-gray-100 px-5 py-3 text-sm font-medium text-gray-700 transition hover:bg-gray-200 dark:bg-gray-800 dark:text-gray-300 dark:hover:bg-gray-700"
                >
                  {retryAction.label}
                </button>
              )}
            </div>
          </div>
        </div>
      </div>
    </>
  );
};

export default ExceptionPageShell;
