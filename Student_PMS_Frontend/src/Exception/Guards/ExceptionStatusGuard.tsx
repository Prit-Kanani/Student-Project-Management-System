import { Navigate, Outlet, useParams } from "react-router";
import { isSupportedExceptionStatusCode } from "@/Exception/Service";

const ExceptionStatusGuard = () => {
  const { statusCode } = useParams();
  const parsedStatusCode = Number(statusCode);

  if (!isSupportedExceptionStatusCode(parsedStatusCode)) {
    return <Navigate to="/error-404" replace />;
  }

  return <Outlet />;
};

export default ExceptionStatusGuard;
