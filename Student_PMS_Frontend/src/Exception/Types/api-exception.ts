export class ApiException extends Error {
  public readonly statusCode: number;
  public readonly details?: unknown;

  public constructor(message: string, statusCode: number, details?: unknown) {
    super(message);
    this.name = new.target.name;
    this.statusCode = statusCode;
    this.details = details;

    Object.setPrototypeOf(this, new.target.prototype);
  }
}

export class BadRequestException extends ApiException {
  public constructor(message: string, details?: unknown) {
    super(message, 400, details);
  }
}

export class NotFoundException extends ApiException {
  public constructor(message: string, details?: unknown) {
    super(message, 404, details);
  }
}

export class UnauthorizedException extends ApiException {
  public constructor(message: string, details?: unknown) {
    super(message, 401, details);
  }
}

export class ApiServerException extends ApiException {
  public constructor(message: string, details?: unknown) {
    super(message, 500, details);
  }
}

export abstract class DatabaseException extends ApiException {
  public readonly sqlErrorNumber?: number;

  public constructor(
    message: string,
    statusCode: number,
    sqlErrorNumber?: number,
    details?: unknown,
  ) {
    super(message, statusCode, details);
    this.sqlErrorNumber = sqlErrorNumber;
  }
}

export class DuplicateKeyException extends DatabaseException {
  public constructor(
    message: string,
    sqlErrorNumber?: number,
    details?: unknown,
  ) {
    super(message, 409, sqlErrorNumber, details);
  }
}

export class ForeignKeyViolationException extends DatabaseException {
  public constructor(
    message: string,
    sqlErrorNumber?: number,
    details?: unknown,
  ) {
    super(message, 400, sqlErrorNumber, details);
  }
}

export class DatabaseTimeoutException extends DatabaseException {
  public constructor(
    message: string,
    sqlErrorNumber?: number,
    details?: unknown,
  ) {
    super(message, 503, sqlErrorNumber, details);
  }
}

export class DatabaseServerException extends ApiException {
  public constructor(message: string, details?: unknown) {
    super(message, 500, details);
  }
}

export const isApiException = (value: unknown): value is ApiException =>
  value instanceof ApiException;
