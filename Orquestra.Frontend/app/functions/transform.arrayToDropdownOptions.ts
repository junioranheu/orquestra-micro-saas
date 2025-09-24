import { iDropdownOption } from '@/app/components/input/drop-down';
import { Guid } from 'guid-typescript';

// Versão para GUID;
export function handleTransformArrayToDropdownOptionsGuid(data: any[], valueField: string, labelField: string): iDropdownOption<Guid>[] {
    return data?.map(item => ({
        value: Guid.parse(String(handleGetNestedValue(item, valueField))),
        label: String(handleGetNestedValue(item, labelField))
    }));
}

// Versão para Number;
export function handleTransformArrayToDropdownOptionsNumber(data: any[], valueField: string, labelField: string): iDropdownOption<number>[] {
    return data?.map(item => ({
        value: Number(handleGetNestedValue(item, valueField)),
        label: String(handleGetNestedValue(item, labelField))
    }));
}

function handleGetNestedValue(obj: any, path: string) {
    return path.split('.').reduce((acc, key) => acc?.[key], obj);
}