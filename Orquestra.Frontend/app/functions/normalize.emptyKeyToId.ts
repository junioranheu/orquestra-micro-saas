import { Dispatch, SetStateAction } from 'react';

/**
 * Normaliza a primeira key vazia ("" ou só espaços) do objeto,
 * removendo-a e criando uma nova propriedade com o nome informado,
 * usando o GUID encontrado em value.value.
 *
 * Atualiza o estado do formulário e também retorna
 * o objeto normalizado para uso imediato.
 *
 * @param formData Objeto atual do formulário
 * @param setFormData setState do formulário
 * @param targetProp Nome da nova propriedade (ex: "clientId")
 * @returns Formulário normalizado
 */
export default function handleNormalizeEmptyKeyToId<T extends Record<string, any>>(formData: T, setFormData: Dispatch<SetStateAction<T>>, targetProp: string): T {
    const result: Record<string, any> = { ...formData };

    const entry = Object.entries(result).find(([key]) => !key || key.trim() === '');

    if (!entry) {
        return formData;
    }

    const [emptyKey, emptyValue] = entry;

    const guid = emptyValue?.value?.value ?? null;

    delete result[emptyKey];
    result[targetProp] = guid;

    setFormData(result as T);

    return result as T;
}