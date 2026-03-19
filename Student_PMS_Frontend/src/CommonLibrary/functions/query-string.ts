import type { QueryParams } from "@/CommonLibrary/types";

const isDefined = <TValue>(value: TValue | null | undefined): value is TValue =>
  value !== null && value !== undefined;

export const buildQueryString = (query?: QueryParams) => {
  if (!query) {
    return "";
  }

  const searchParams = new URLSearchParams();

  Object.entries(query).forEach(([key, value]) => {
    if (Array.isArray(value)) {
      value.filter(isDefined).forEach((item) => searchParams.append(key, String(item)));
      return;
    }

    if (!isDefined(value)) {
      return;
    }

    searchParams.append(key, String(value));
  });

  const queryString = searchParams.toString();

  return queryString ? `?${queryString}` : "";
};
