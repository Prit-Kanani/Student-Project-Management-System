import Badge from "@/components/ui/badge/Badge";

interface ProjectGroupApprovalStatusBadgeProps {
  value: boolean | null | string;
}

const normalizeApprovalLabel = (value: boolean | null | string) => {
  if (typeof value === "boolean") {
    return value ? "Approved" : "Rejected";
  }

  if (typeof value === "string" && value.trim().length > 0) {
    return value.trim();
  }

  return "Pending";
};

const getBadgeColor = (value: boolean | null | string) => {
  if (typeof value === "boolean") {
    return value ? "success" : "error";
  }

  if (typeof value !== "string") {
    return "warning";
  }

  const normalizedValue = value.trim().toLowerCase();

  if (normalizedValue.includes("approved")) {
    return "success";
  }

  if (normalizedValue.includes("reject")) {
    return "error";
  }

  return "warning";
};

const ProjectGroupApprovalStatusBadge = ({
  value,
}: ProjectGroupApprovalStatusBadgeProps) => (
  <Badge color={getBadgeColor(value)} size="sm">
    {normalizeApprovalLabel(value)}
  </Badge>
);

export default ProjectGroupApprovalStatusBadge;
