import { getProjectsPage } from "@/ProjectServices/services";
import { getUsersPage } from "@/UserServices/services";
import { getProjectGroupsPage } from "@/ProjectGroupServices/services/project-group-service";
import type { LookupOption } from "@/ProjectGroupServices/types";

const sortOptions = (options: LookupOption[]) =>
  [...options].sort((left, right) => left.label.localeCompare(right.label));

const withInactiveLabel = (label: string, isActive: boolean) =>
  isActive ? label : `${label} (Inactive)`;

export const getProjectGroupOptions = async (): Promise<LookupOption[]> => {
  const page = await getProjectGroupsPage();

  return sortOptions(
    page.items.map((item) => ({
      value: item.projectGroupId,
      label: withInactiveLabel(item.projectGroupName, item.isActive),
    })),
  );
};

export const getProjectOptions = async (): Promise<LookupOption[]> => {
  const page = await getProjectsPage();

  return sortOptions(
    page.items.map((item) => ({
      value: item.projectId,
      label: withInactiveLabel(item.projectName, item.isActive),
    })),
  );
};

export const getUserOptions = async (): Promise<LookupOption[]> => {
  const page = await getUsersPage();

  return sortOptions(
    page.items.map((item) => ({
      value: item.userId,
      label: `${item.name} (#${item.userId})${item.isActive ? "" : " (Inactive)"}`,
    })),
  );
};

export const buildLookupMap = (options: LookupOption[]) =>
  new Map(options.map((option) => [option.value, option.label]));
