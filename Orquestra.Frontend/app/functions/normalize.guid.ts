import { Guid } from 'guid-typescript';

/**
 * Normaliza um array extraindo `.value` quando presente
 * e retornando um array de Guid.
 *
 * @param field Array contendo objetos ou strings GUID
 * @returns Array de Guid válido
 *
 * @example
 * const usersIds = [
 *   { value: "0198c7f3-821a-7866-962e-4983c388a016" },
 *   "3fa85f64-5717-4562-b3fc-2c963f66afa6",
 *   null
 * ];
 * handleNormalizeGuidArrayField(usersIds);
 * // Retorna: [Guid, Guid]
 */
export function handleNormalizeGuidArrayField(field: Guid[]): Guid[] {
    // @ts-ignore;
    const normalized = field?.map(u => u.value);

    if (!normalized || !normalized?.length || normalized.every(x => x === null) || Array.isArray(normalized) && normalized.length === 1 && (normalized[0] === null || normalized[0] === undefined)) {
        return field;
    }

    return normalized;
}

export function handleNormalizeGuidField(field: Guid): Guid {
    // @ts-ignore;
    const normalized = field?.value;

    if (!normalized || !normalized?.length) {
        return field;
    }

    return normalized;
}