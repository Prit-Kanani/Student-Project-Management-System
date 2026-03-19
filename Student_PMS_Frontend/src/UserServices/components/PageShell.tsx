import type { ReactNode } from "react";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";

interface PageShellProps {
  title: string;
  description: string;
  actions?: ReactNode;
  children: ReactNode;
}

const PageShell = ({ title, description, actions, children }: PageShellProps) => {
  return (
    <div className="space-y-6">
      <PageBreadcrumb pageTitle={title} />
      <section className="rounded-2xl border border-gray-200 bg-white p-6 shadow-theme-xs dark:border-gray-800 dark:bg-white/[0.03]">
        <div className="flex flex-col gap-4 lg:flex-row lg:items-start lg:justify-between">
          <div className="max-w-3xl space-y-2">
            <h1 className="text-2xl font-semibold text-gray-800 dark:text-white/90">
              {title}
            </h1>
            <p className="text-sm text-gray-500 dark:text-gray-400">{description}</p>
          </div>
          {actions ? <div className="flex flex-wrap gap-3">{actions}</div> : null}
        </div>
      </section>
      {children}
    </div>
  );
};

export default PageShell;
