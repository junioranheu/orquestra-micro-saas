import { iDropdownOption } from '@/app/components/input/drop-down';
import { Guid } from 'guid-typescript';

// Versão para GUID;
export function handleTransformArrayToDropdownOptionsGuid(data: any[], valueField: string, labelField: string): iDropdownOption<Guid>[] {
    return data?.map(item => ({
        value: Guid.parse(String(item[valueField])), // Guid;
        label: String(item[labelField])
    }));
}

// Versão para Number;
export function handleTransformArrayToDropdownOptionsNumber(data: any[], valueField: string, labelField: string): iDropdownOption<number>[] {
    return data?.map(item => ({
        value: Number(item[valueField]), // Number;
        label: String(item[labelField])
    }));
}