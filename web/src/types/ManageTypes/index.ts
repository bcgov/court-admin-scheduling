export interface assignmentTypeInfoType {
    code: string;
    concurrencyToken?: number;
    id: number;
    locationId: number;
    type: string;
    sortOrder: number;
}

export interface leaveTrainingTypeInfoType {
    code: string;
    concurrencyToken?: number;
    id: number;
    validityPeriod?: number;
    mandatory?: boolean;
    category?: string;
    advanceNotice?: number;
    type: string;
    sortOrder: number;
}

export interface defineTypeInfoType {
    id: number;
    name: string;
    description: string;
    abbreviation: string;
    displayColor: string;
    isSystem?: boolean;
    expiryDate?: string | null;
    sortOrder?: number | null;
    _rowVariant?: string;
}