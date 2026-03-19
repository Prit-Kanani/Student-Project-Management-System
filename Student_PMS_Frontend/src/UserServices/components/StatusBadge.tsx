import Badge from "@/components/ui/badge/Badge";

interface StatusBadgeProps {
  active: boolean;
  activeLabel?: string;
  inactiveLabel?: string;
}

const StatusBadge = ({
  active,
  activeLabel = "Active",
  inactiveLabel = "Inactive",
}: StatusBadgeProps) => {
  return (
    <Badge color={active ? "success" : "warning"} size="sm">
      {active ? activeLabel : inactiveLabel}
    </Badge>
  );
};

export default StatusBadge;
