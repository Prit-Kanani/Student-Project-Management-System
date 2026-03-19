import { useParams } from "react-router";
import { useGlobalException } from "@/Exception/Components";
import ExceptionPageShell from "@/Exception/Components/ExceptionPageShell";
import {
  getExceptionDetailLines,
  getExceptionPageConfig,
  isSupportedExceptionStatusCode,
} from "@/Exception/Service";

interface ExceptionStatusPageProps {
  statusCode?: number;
}

const ExceptionStatusPage = ({ statusCode }: ExceptionStatusPageProps) => {
  const { statusCode: statusCodeParam } = useParams();
  const { activeException } = useGlobalException();

  const resolvedStatusCode = statusCode ?? Number(statusCodeParam);
  const effectiveStatusCode = isSupportedExceptionStatusCode(resolvedStatusCode)
    ? resolvedStatusCode
    : 500;
  const pageConfig = getExceptionPageConfig(effectiveStatusCode);
  const detailLines =
    activeException?.statusCode === effectiveStatusCode
      ? getExceptionDetailLines(activeException)
      : [];

  return (
    <ExceptionPageShell
      statusCode={effectiveStatusCode}
      title={pageConfig.title}
      description={activeException?.message || pageConfig.description}
      primaryAction={{
        label: pageConfig.primaryActionLabel,
        to: pageConfig.primaryActionPath,
      }}
      secondaryAction={{
        label: pageConfig.secondaryActionLabel,
        to: pageConfig.secondaryActionPath,
      }}
      detailLines={detailLines}
    />
  );
};

export default ExceptionStatusPage;
