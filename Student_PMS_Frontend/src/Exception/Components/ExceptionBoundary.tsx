import { Component, type ErrorInfo, type ReactNode } from "react";
import ExceptionPageShell from "@/Exception/Components/ExceptionPageShell";
import {
  getExceptionDetailLines,
  getExceptionPageConfig,
  normalizeException,
  publishException,
} from "@/Exception/Service";
import type { ApiException } from "@/Exception/Types";

interface ExceptionBoundaryProps {
  children: ReactNode;
}

interface ExceptionBoundaryState {
  exception: ApiException | null;
}

class ExceptionBoundary extends Component<
  ExceptionBoundaryProps,
  ExceptionBoundaryState
> {
  public state: ExceptionBoundaryState = {
    exception: null,
  };

  public static getDerivedStateFromError(error: Error): ExceptionBoundaryState {
    return {
      exception: normalizeException({
        message: error.message,
        statusCode: 500,
      }),
    };
  }

  public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
    const exception = normalizeException({
      message: error.message || "A rendering error interrupted the application.",
      statusCode: 500,
      details: {
        componentStack: errorInfo.componentStack,
      },
    });

    publishException(exception);
    this.setState({ exception });
  }

  private readonly handleRetry = () => {
    this.setState({ exception: null });
  };

  public render() {
    if (this.state.exception) {
      const pageConfig = getExceptionPageConfig(this.state.exception.statusCode);

      return (
        <ExceptionPageShell
          statusCode={this.state.exception.statusCode}
          title={pageConfig.title}
          description={this.state.exception.message || pageConfig.description}
          primaryAction={{
            label: pageConfig.primaryActionLabel,
            to: pageConfig.primaryActionPath,
          }}
          secondaryAction={{
            label: pageConfig.secondaryActionLabel,
            to: pageConfig.secondaryActionPath,
          }}
          detailLines={getExceptionDetailLines(this.state.exception)}
          retryAction={{
            label: "Try again",
            onClick: this.handleRetry,
          }}
        />
      );
    }

    return this.props.children;
  }
}

export default ExceptionBoundary;
