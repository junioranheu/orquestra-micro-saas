import { iDropdownOption } from '@/app/components/input/drop-down/drop-down';

export default function transformArrayToDropdownOptions(data: any[], valueField: string, labelField: string): iDropdownOption[] {
    return data?.map(item => ({
        value: item[valueField],
        label: item[labelField],
    }));
};