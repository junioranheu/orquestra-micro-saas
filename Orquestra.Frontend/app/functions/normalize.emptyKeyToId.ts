import { Dispatch, SetStateAction } from 'react';

/**
 * Normaliza a primeira key vazia ("" ou só espaços) do objeto,
 * removendo-a e criando/atualizando uma propriedade com o nome informado,
 * usando o GUID encontrado em value.value.
 *
 * - Se existir key vazia → move o valor para targetProp
 * - Se NÃO existir key vazia mas targetProp existir → atualiza targetProp
 * - Atualiza o estado do formulário e retorna o objeto final
 *
 * @param formData Objeto atual do formulário
 * @param setFormData setState do formulário
 * @param targetProp Nome da propriedade (ex: "clientId")
 * @returns Formulário normalizado
 */
export default function handleNormalizeEmptyKeyToId<T extends Record<string, any>>(formData: T, setFormData: Dispatch<SetStateAction<T>>, targetProp: string): T {
    const result: Record<string, any> = { ...formData };

    const emptyEntry = Object.entries(result).find(([key]) => !key || key.trim() === '');

    let guid: string | null = null;

    if (emptyEntry) {
        const [emptyKey, emptyValue] = emptyEntry;
        guid = emptyValue?.value?.value ?? null;
        delete result[emptyKey];
    } else if (result[targetProp]) {
        // Mantém compatível com o formato do select;
        guid = result[targetProp]?.value?.value ?? result[targetProp];
    }

    if (guid !== null) {
        result[targetProp] = guid;
    }

    setFormData(result as T);

    return result as T;
}