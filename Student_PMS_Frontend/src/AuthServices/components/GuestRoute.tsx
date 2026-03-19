import { Navigate, Outlet, useLocation } from "react-router";
import AuthRoutePending from "@/AuthServices/components/AuthRoutePending";
import { useAuth } from "@/AuthServices/components/AuthProvider";
import type { AuthRouteState } from "@/AuthServices/types";

const GuestRoute = () => {
  const location = useLocation();
  const { status, isAuthenticated } = useAuth();

  if (status === "loading") {
    return <AuthRoutePending message="Loading authentication..." />;
  }

  if (isAuthenticated) {
    const state = location.state as AuthRouteState | null;
    const redirectTo = state?.from || "/";

    return <Navigate to={redirectTo} replace />;
  }

  return <Outlet />;
};

export default GuestRoute;
