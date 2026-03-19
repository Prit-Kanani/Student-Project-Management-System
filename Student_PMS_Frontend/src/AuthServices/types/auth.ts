import type { UserRole } from "@/CommonLibrary/types";

export type AuthPersistenceMode = "local" | "session";

export interface LoginDTO {
  email: string;
  password: string;
}

export interface AuthResponseDTO {
  message?: string | null;
  token?: string | null;
}

export interface RegisterDTO {
  name: string;
  email: string;
  password: string;
  roleId: number;
  isActive: boolean;
}

export interface JwtPayload {
  sub?: string;
  exp?: number;
  email?: string;
  unique_name?: string;
  name?: string;
  role?: string | string[];
  roles?: string | string[];
  [claim: string]: unknown;
}

export interface AuthUser {
  userId: number | null;
  name: string;
  email: string;
  roles: string[];
  primaryRole: string | null;
  expiresAt: number | null;
}

export interface AuthSession {
  accessToken: string;
  user: AuthUser;
  persistence: AuthPersistenceMode;
  message?: string | null;
}

export interface AuthState {
  status: "loading" | "authenticated" | "unauthenticated";
  session: AuthSession | null;
}

export interface AuthContextValue extends AuthState {
  user: AuthUser | null;
  accessToken: string | null;
  isAuthenticated: boolean;
  login: (
    credentials: LoginDTO,
    persistence?: AuthPersistenceMode,
  ) => Promise<AuthSession>;
  register: (payload: RegisterDTO) => Promise<void>;
  logout: () => void;
  hasRole: (role: UserRole | string) => boolean;
  hasAnyRole: (roles: readonly (UserRole | string)[]) => boolean;
}

export interface PublicRegistrationRole {
  id: number;
  name: UserRole;
  description: string;
}

export interface SignInFormValues {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface SignUpFormValues {
  name: string;
  email: string;
  password: string;
  confirmPassword: string;
  roleId: string;
  acceptTerms: boolean;
}

export interface SignInFormErrors {
  email?: string;
  password?: string;
  form?: string;
}

export interface SignUpFormErrors {
  name?: string;
  email?: string;
  password?: string;
  confirmPassword?: string;
  roleId?: string;
  acceptTerms?: string;
  form?: string;
}

export interface AuthPageNotice {
  variant: "success" | "error" | "warning" | "info";
  title: string;
  message: string;
}

export interface AuthRouteState {
  from?: string;
  notice?: AuthPageNotice;
}
