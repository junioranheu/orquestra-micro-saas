import { Guid } from 'guid-typescript';

// Extrai o valor "limpo" de um item GUID, mesmo quando ele vem dentro de um objeto com .value ou .id;
function getGuidValue(value: unknown): unknown {
    if (value === null || value === undefined) {
        return null;
    }

    if (typeof value === 'object') {
        if (Array.isArray(value)) {
            // Se vier um array, normaliza cada item recursivamente;
            return value.map(item => getGuidValue(item));
        }

        const record = value as Record<string, unknown>;
        // Se o objeto tiver .value, usa esse valor como fonte do GUID;
        if (record.value !== undefined && record.value !== null) {
            return getGuidValue(record.value);
        }

        // Se não tiver .value, tenta usar .id como fallback;
        if (record.id !== undefined && record.id !== null) {
            return getGuidValue(record.id);
        }

        // Se não for nenhum desses casos, devolve o próprio objeto;
        return value;
    }

    // Para strings, numbers e outros valores simples, retorna como está;
    return value;
}

// Normaliza um array de GUIDs, aceitando tanto arrays quanto valores únicos;
export function handleNormalizeGuidArrayField(field: unknown): Guid[] {
    if (field === null || field === undefined) {
        return [];
    }

    // Se vier um único valor, transforma em array com um elemento;
    if (!Array.isArray(field)) {
        const normalized = getGuidValue(field);

        return normalized === null || normalized === undefined ? [] : [normalized as Guid];
    }

    // Se vier um array, normaliza cada item e remove valores vazios;
    const normalized = field
        .map(item => getGuidValue(item))
        .filter(item => item !== null && item !== undefined);

    if (!normalized.length) {
        return [];
    }

    return normalized as Guid[];
}

// Normaliza um único campo GUID, retornando o valor simples quando possível;
export function handleNormalizeGuidField(field: unknown): Guid {
    if (field === null || field === undefined) {
        return field as unknown as Guid;
    }

    const normalized = getGuidValue(field);

    return (normalized === null || normalized === undefined ? field : normalized) as unknown as Guid;
}