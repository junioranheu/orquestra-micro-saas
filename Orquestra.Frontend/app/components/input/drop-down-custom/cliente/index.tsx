import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, SetStateAction } from 'react';

interface iProps {
    editing: boolean;
    clientsDropDown: iDropdownOption<string | number | Guid>[] | undefined;
    setClientIdOption: Dispatch<SetStateAction<iDropdownOption<string | number | Guid>[]>>;
    clientId: Guid | null | undefined;
    isObligatory?: boolean;
    showNewClientButton?: boolean;
}

export default function DropDownCliente({ editing, clientsDropDown, setClientIdOption, clientId, isObligatory = false, showNewClientButton = false }: iProps) {
    return (
        <Fragment>
            <Dropdown
                title='Cliente'
                options={clientsDropDown ?? []}
                selectedOption={clientsDropDown?.find(x => x.value.toString() === clientId?.toString())}
                setSelectedOption={setClientIdOption}
                isDisabled={!editing}
                isObligatory={isObligatory}
                showAction={true}
                actionLabel='Cadastrar novo cliente'
                onActionClick={() => alert('xd')}
            />
        </Fragment>
    )
}