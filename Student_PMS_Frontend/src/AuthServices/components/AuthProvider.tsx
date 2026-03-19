import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import {
  clearAccessTokenResolver,
  registerAccessTokenResolver,
  registerApiErrorInterceptor,
} from "@/CommonLibrary/functions";
import {
  hasAnyRole,
  hasRole,
  loginWithCredentials,
  logoutFromSession,
  registerAccount,
  restoreAuthSession,
} from "@/AuthServices/services";
import { authStorage } from "@/AuthServices/services/auth-storage";
import { normalizeException } from "@/Exception/Service";
import type {
  AuthContextValue,
  AuthPersistenceMode,
  AuthSession,
  AuthState,
  LoginDTO,
  RegisterDTO,
} from "@/AuthServices/types";

const AuthContext = createContext<AuthContextValue | null>(null);

const createAuthenticatedState = (session: AuthSession): AuthState => ({
  status: "authenticated",
  session,
});

const createUnauthenticatedState = (): AuthState => ({
  status: "unauthenticated",
  session: null,
});

export const AuthProvider = ({ children }: { children: ReactNode }) => {
  const [authState, setAuthState] = useState<AuthState>({
    status: "loading",
    session: null,
  });

  useEffect(() => {
    registerAccessTokenResolver(() => authStorage.getAccessToken());

    const restoredSession = restoreAuthSession();
    setAuthState(
      restoredSession
        ? createAuthenticatedState(restoredSession)
        : createUnauthenticatedState(),
    );

    return () => {
      clearAccessTokenResolver();
    };
  }, []);

  useEffect(() => {
    const removeInterceptor = registerApiErrorInterceptor((error, context) => {
      if (context.request.authMode === "public") {
        return error;
      }

      const exception = normalizeException(error);

      if (exception.statusCode === 401) {
        logoutFromSession();
        setAuthState(createUnauthenticatedState());
      }

      return exception;
    });

    return () => {
      removeInterceptor();
    };
  }, []);

  const login = useCallback(
    async (credentials: LoginDTO, persistence: AuthPersistenceMode = "local") => {
      const nextSession = await loginWithCredentials(credentials, persistence);
      setAuthState(createAuthenticatedState(nextSession));

      return nextSession;
    },
    [],
  );

  const register = useCallback(async (payload: RegisterDTO) => {
    await registerAccount(payload);
  }, []);

  const logout = useCallback(() => {
    logoutFromSession();
    setAuthState(createUnauthenticatedState());
  }, []);

  const contextValue = useMemo<AuthContextValue>(() => {
    const session = authState.session;
    const user = session?.user ?? null;

    return {
      ...authState,
      user,
      accessToken: session?.accessToken ?? null,
      isAuthenticated: authState.status === "authenticated" && session !== null,
      login,
      register,
      logout,
      hasRole: (role) => hasRole(session, role),
      hasAnyRole: (roles) => hasAnyRole(session, roles),
    };
  }, [authState, login, logout, register]);

  return <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>;
};

export const useAuth = () => {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used within AuthProvider.");
  }

  return context;
};
