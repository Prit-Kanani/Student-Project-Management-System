// Shared DTO contracts normalized for the frontend layer.
export interface OperationResultDTO {
  id: number;
  rowsAffected: number;
}

export interface ListResult<TItem> {
  totalCount: number;
  items: TItem[];
}

export interface OptionDTO {
  id: number;
  name: string;
}

export interface JwtSettings {
  secretKey: string;
  issuer: string;
  audience: string;
  expiryMinutes: number;
}

export interface CreatedAndModifiedDTO {
  createdBy: string;
  modifiedBy: string;
}

export interface AuditUsersDTO {
  createdBy: string;
  modifiedBy: string;
  approvedBy: string;
}

export interface EntityExistsDTO {
  id: number;
  exists: boolean;
}
