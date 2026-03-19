import { useEffect } from "react";
import { useAutoDismissState } from "@/CommonLibrary/hooks/useAutoDismissState";
import Alert from "@/components/ui/alert/Alert";
import type { PageNotice } from "@/UserServices/types";
import type { ExceptionFeedback } from "@/UserServices/services";

interface FeedbackStackProps {
  notice?: PageNotice | null;
  error?: ExceptionFeedback | null;
}

const FeedbackStack = ({ notice, error }: FeedbackStackProps) => {
  const [activeNotice, setActiveNotice] = useAutoDismissState<PageNotice>(
    notice ?? null,
  );
  const [activeError, setActiveError] = useAutoDismissState<ExceptionFeedback>(
    error ?? null,
  );

  useEffect(() => {
    setActiveNotice(notice ?? null);
  }, [notice, setActiveNotice]);

  useEffect(() => {
    setActiveError(error ?? null);
  }, [error, setActiveError]);

  if (!activeNotice && !activeError) {
    return null;
  }

  const detailLines = activeError
    ? Array.from(new Set(activeError.detailLines))
    : [];

  return (
    <div className="space-y-4">
      {activeNotice ? (
        <Alert
          variant={activeNotice.variant}
          title={activeNotice.title}
          message={activeNotice.message}
        />
      ) : null}

      {activeError ? (
        <div className="space-y-3">
          <Alert
            variant="error"
            title="Request failed"
            message={activeError.message}
          />
          {detailLines.length > 0 ? (
            <div className="rounded-2xl border border-gray-200 bg-white p-4 text-sm text-gray-600 shadow-theme-xs dark:border-gray-800 dark:bg-gray-900 dark:text-gray-300">
              <ul className="space-y-2">
                {detailLines.map((line) => (
                  <li key={line}>{line}</li>
                ))}
              </ul>
            </div>
          ) : null}
        </div>
      ) : null}
    </div>
  );
};

export default FeedbackStack;
