import type {
  EntityExistsDTO,
  ListResult,
  OperationResultDTO,
} from "@/CommonLibrary/types";

const isRecord = (value: unknown): value is Record<string, unknown> =>
  typeof value === "object" && value !== null;

const readValue = (source: unknown, keys: string[]) => {
  if (!isRecord(source)) {
    return undefined;
  }

  const sourceKeys = Object.keys(source);

  for (const key of keys) {
    if (key in source) {
      return source[key];
    }

    const lowerCaseMatch = sourceKeys.find(
      (sourceKey) => sourceKey.toLowerCase() === key.toLowerCase(),
    );

    if (lowerCaseMatch) {
      return source[lowerCaseMatch];
    }
  }

  return undefined;
};

export const readString = (source: unknown, ...keys: string[]) => {
  const value = readValue(source, keys);

  return typeof value === "string" ? value.trim() : "";
};

export const readNullableString = (source: unknown, ...keys: string[]) => {
  const value = readValue(source, keys);

  if (typeof value !== "string") {
    return null;
  }

  const trimmedValue = value.trim();
  return trimmedValue.length > 0 ? trimmedValue : null;
};

export const readNumber = (source: unknown, ...keys: string[]) => {
  const value = readValue(source, keys);

  if (typeof value === "number" && Number.isFinite(value)) {
    return value;
  }

  if (typeof value === "string" && value.trim().length > 0) {
    const parsedValue = Number(value);

    if (Number.isFinite(parsedValue)) {
      return parsedValue;
    }
  }

  return 0;
};

export const readBoolean = (source: unknown, ...keys: string[]) => {
  const value = readValue(source, keys);

  if (typeof value === "boolean") {
    return value;
  }

  if (typeof value === "string") {
    return value.toLowerCase() === "true";
  }

  return Boolean(value);
};

export const readNullableBoolean = (source: unknown, ...keys: string[]) => {
  const value = readValue(source, keys);

  if (typeof value === "boolean") {
    return value;
  }

  if (typeof value === "string") {
    const normalizedValue = value.trim().toLowerCase();

    if (normalizedValue === "true") {
      return true;
    }

    if (normalizedValue === "false") {
      return false;
    }
  }

  return null;
};

export const mapListResult = <TItem>(
  payload: unknown,
  mapper: (item: unknown) => TItem,
): ListResult<TItem> => {
  const itemsSource = readValue(payload, ["items", "Items"]);
  const items = Array.isArray(itemsSource) ? itemsSource.map(mapper) : [];

  return {
    totalCount: readNumber(payload, "totalCount", "TotalCount"),
    items,
  };
};

export const mapOperationResult = (payload: unknown): OperationResultDTO => ({
  id: readNumber(payload, "id", "Id"),
  rowsAffected: readNumber(payload, "rowsAffected", "RowsAffected"),
});

export const mapEntityExists = (payload: unknown): EntityExistsDTO => ({
  id: readNumber(payload, "id", "Id"),
  exists: readBoolean(payload, "exists", "Exists"),
});
