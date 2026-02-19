import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import { Guid } from 'guid-typescript';
import { Dispatch, SetStateAction } from 'react';

interface iProps {
    editing: boolean;
    clientsDropDown: iDropdownOption<string | number | Guid>[] | undefined;
    setClientIdOption: Dispatch<SetStateAction<iDropdownOption<string | number | Guid>[]>>;
    clientId: Guid | null | undefined;
    isObligatory: boolean;
}

export default function DropDownCliente({ editing, clientsDropDown, setClientIdOption, clientId, isObligatory }: iProps) {
    return (
        <Dropdown
            title='Cliente'
            options={clientsDropDown ?? []}
            selectedOption={clientsDropDown?.find(x => x.value.toString() === clientId?.toString())}
            setSelectedOption={setClientIdOption}
            isDisabled={!editing}
            isObligatory={isObligatory}
        />
    )
}