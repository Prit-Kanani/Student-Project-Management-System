import Badge from "@/components/ui/badge/Badge";
import { getApprovalStatusLabel } from "@/ProjectServices/services";

interface ApprovalStatusBadgeProps {
  value: boolean | null;
}

const ApprovalStatusBadge = ({ value }: ApprovalStatusBadgeProps) => {
  const color = value === true ? "success" : value === false ? "error" : "warning";

  return (
    <Badge color={color} size="sm">
      {getApprovalStatusLabel(value)}
    </Badge>
  );
};

export default ApprovalStatusBadge;
