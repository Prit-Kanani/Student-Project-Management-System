import Badge from "@/components/ui/badge/Badge";

interface CompletionBadgeProps {
  completed: boolean;
}

const CompletionBadge = ({ completed }: CompletionBadgeProps) => {
  return (
    <Badge color={completed ? "success" : "info"} size="sm">
      {completed ? "Completed" : "In Progress"}
    </Badge>
  );
};

export default CompletionBadge;
