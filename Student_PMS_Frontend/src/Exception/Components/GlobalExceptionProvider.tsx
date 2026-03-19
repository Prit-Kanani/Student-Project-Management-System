import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  type ReactNode,
} from "react";
import { useAutoDismissState } from "@/CommonLibrary/hooks/useAutoDismissState";
import GlobalExceptionAlert from "@/Exception/Components/GlobalExceptionAlert";
import {
  normalizeException,
  publishException,
  subscribeToExceptions,
} from "@/Exception/Service";
import type { ApiException } from "@/Exception/Types";

interface GlobalExceptionContextValue {
  activeException: ApiException | null;
  reportException: (error: unknown) => void;
  clearException: () => void;
}

const GlobalExceptionContext = createContext<GlobalExceptionContextValue | null>(
  null,
);

export const GlobalExceptionProvider = ({
  children,
}: {
  children: ReactNode;
}) => {
  const [activeException, setActiveException] =
    useAutoDismissState<ApiException>(null);

  const clearException = useCallback(() => {
    setActiveException(null);
  }, [setActiveException]);

  const reportException = useCallback((error: unknown) => {
    publishException(error);
  }, []);

  useEffect(() => {
    const unsubscribe = subscribeToExceptions((exception) => {
      setActiveException(exception);
    });

    const handleWindowError = (event: ErrorEvent) => {
      setActiveException(
        normalizeException(event.error ?? { message: event.message, statusCode: 500 }),
      );
    };

    const handleUnhandledRejection = (event: PromiseRejectionEvent) => {
      setActiveException(normalizeException(event.reason));
    };

    window.addEventListener("error", handleWindowError);
    window.addEventListener("unhandledrejection", handleUnhandledRejection);

    return () => {
      unsubscribe();
      window.removeEventListener("error", handleWindowError);
      window.removeEventListener("unhandledrejection", handleUnhandledRejection);
    };
  }, []);

  const contextValue = useMemo<GlobalExceptionContextValue>(
    () => ({
      activeException,
      reportException,
      clearException,
    }),
    [activeException, clearException, reportException],
  );

  return (
    <GlobalExceptionContext.Provider value={contextValue}>
      {children}
      {activeException && (
        <GlobalExceptionAlert
          exception={activeException}
          onDismiss={clearException}
        />
      )}
    </GlobalExceptionContext.Provider>
  );
};

export const useGlobalException = () => {
  const context = useContext(GlobalExceptionContext);

  if (!context) {
    throw new Error(
      "useGlobalException must be used within GlobalExceptionProvider.",
    );
  }

  return context;
};
