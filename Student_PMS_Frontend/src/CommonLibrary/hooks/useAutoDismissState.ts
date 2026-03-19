import { useEffect, useState } from "react";

export const AUTO_DISMISS_DURATION_MS = 5000;

export const useAutoDismissState = <T>(
  initialValue: T | null,
  durationMs = AUTO_DISMISS_DURATION_MS,
) => {
  const [value, setValue] = useState<T | null>(initialValue);

  useEffect(() => {
    if (value === null || durationMs <= 0) {
      return undefined;
    }

    const timeoutId = window.setTimeout(() => {
      setValue(null);
    }, durationMs);

    return () => {
      window.clearTimeout(timeoutId);
    };
  }, [durationMs, value]);

  return [value, setValue] as const;
};
