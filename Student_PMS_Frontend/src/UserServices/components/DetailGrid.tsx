import type { ReactNode } from "react";

interface DetailItem {
  label: string;
  value: ReactNode;
}

interface DetailGridProps {
  items: DetailItem[];
}

const DetailGrid = ({ items }: DetailGridProps) => {
  return (
    <div className="grid gap-4 md:grid-cols-2">
      {items.map((item) => (
        <div
          key={item.label}
          className="rounded-xl border border-gray-200 bg-gray-50 px-4 py-3 dark:border-gray-800 dark:bg-gray-900/50"
        >
          <p className="text-xs font-semibold uppercase tracking-[0.16em] text-gray-500 dark:text-gray-400">
            {item.label}
          </p>
          <div className="mt-2 text-sm text-gray-800 dark:text-white/90">{item.value}</div>
        </div>
      ))}
    </div>
  );
};

export default DetailGrid;
