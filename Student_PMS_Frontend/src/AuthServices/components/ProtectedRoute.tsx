import { Navigate, Outlet, useLocation } from "react-router";
import AuthRoutePending from "@/AuthServices/components/AuthRoutePending";
import { useAuth } from "@/AuthServices/components/AuthProvider";

interface ProtectedRouteProps {
  allowedRoles?: string[];
}

const ProtectedRoute = ({ allowedRoles }: ProtectedRouteProps) => {
  const location = useLocation();
  const { status, isAuthenticated, hasAnyRole } = useAuth();

  if (status === "loading") {
    return <AuthRoutePending message="Checking your session..." />;
  }

  if (!isAuthenticated) {
    return (
      <Navigate
        to="/signin"
        replace
        state={{ from: `${location.pathname}${location.search}${location.hash}` }}
      />
    );
  }

  if (allowedRoles && allowedRoles.length > 0 && !hasAnyRole(allowedRoles)) {
    return <Navigate to="/error-401" replace />;
  }

  return <Outlet />;
};

export default ProtectedRoute;
