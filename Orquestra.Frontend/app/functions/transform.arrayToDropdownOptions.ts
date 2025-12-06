import { iDropdownOption } from '@/app/components/input/drop-down';
import { Guid } from 'guid-typescript';

// Versão para GUID (com possibilidade de múltiplos labelField);
export function handleTransformArrayToDropdownOptionsGuid(data: any[], valueField: string, labelField: string | string[]): iDropdownOption<Guid>[] {
    return data?.map(item => {
        const value = Guid.parse(String(handleGetNestedValue(item, valueField)));
        const label = Array.isArray(labelField) ? labelField.map(f => String(handleGetNestedValue(item, f) ?? '').trim()).filter(v => v.length > 0).join(' - ') : String(handleGetNestedValue(item, labelField));

        return { value, label };
    });
}

// Versão para Number;
export function handleTransformArrayToDropdownOptionsNumber(data: any[], valueField: string, labelField: string): iDropdownOption<number>[] {
    return data?.map(item => ({
        value: Number(handleGetNestedValue(item, valueField)),
        label: String(handleGetNestedValue(item, labelField))
    }));
}

// Versão para string;
export function handleTransformArrayToDropdownOptionsString(data: any[], valueField: string, labelField: string): iDropdownOption<string>[] {
    return data?.map(item => ({
        value: String(handleGetNestedValue(item, valueField)),
        label: String(handleGetNestedValue(item, labelField))
    }));
}

function handleGetNestedValue(obj: any, path: string) {
    return path.split('.').reduce((acc, key) => acc?.[key], obj);
}