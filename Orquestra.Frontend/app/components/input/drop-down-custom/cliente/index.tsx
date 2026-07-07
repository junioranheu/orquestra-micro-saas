import EmpresaClientesModalView from '@/app/(routes)/(auth)/empresa/clientes/modal/view';
import { iClient } from '@/app/api/consts/client';
import Icon from '@/app/components/icon';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import toast from '@/app/functions/toast';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import { Guid } from 'guid-typescript';
import { Dispatch, Fragment, SetStateAction, useEffect, useState } from 'react';

interface iProps {
    editing: boolean;
    clientsDropDown: iDropdownOption<string | number | Guid>[] | undefined;
    setClientIdOption: Dispatch<SetStateAction<iDropdownOption<string | number | Guid>[]>>;
    clientId: Guid | null | undefined;
    isObligatory?: boolean;
    showNewClientButton?: boolean;
}

export default function DropDownCliente({ editing, clientsDropDown, setClientIdOption, clientId, isObligatory = false, showNewClientButton = false }: iProps) {

    const me = useApiGetMe({});

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);
    const [, setTrigger] = useState<Date>(new Date());
    const [userJustCreated, setUserJustCreated] = useState<iClient | undefined>(undefined);

    const [internalOptions, setInternalOptions] = useState<iDropdownOption[]>([]);

    useEffect(() => {
        setInternalOptions(clientsDropDown ?? []);
    }, [clientsDropDown]);

    useEffect(() => {
        if (!userJustCreated?.clientId) {
            return;
        }

        const newOption: iDropdownOption<Guid> = {
            value: userJustCreated.clientId,
            label: userJustCreated.fullName
        };

        setInternalOptions(prev => {
            const alreadyExists = prev.some(x => x?.value?.toString?.() === newOption.value.toString());

            if (alreadyExists) {
                return prev;
            }

            toast({
                content: `Cliente ${newOption.label} adicionado à lista. Recomendamos atualizar a página para sincronizar completamente todos os dados do novo cliente.`,
                ms: 10000
            });

            return [...prev, newOption];
        });
    }, [userJustCreated]);

    return (
        <Fragment>
            <Dropdown
                title='Cliente'
                options={internalOptions}
                selectedOption={clientsDropDown?.find(x => x.value.toString() === clientId?.toString())}
                setSelectedOption={setClientIdOption}
                isDisabled={!editing}
                isObligatory={isObligatory}
                showAction={showNewClientButton && editing}
                actionLabel='Cadastrar novo cliente'
                actionLabelIcon={<Icon icon='plus-circle' size='small' />}
                onActionClick={() => setIsModalViewOpen(true)}
            />

            <EmpresaClientesModalView
                isModalOpen={isModalViewOpen}
                setIsModalOpen={setIsModalViewOpen}
                type='create'
                client={undefined}
                companyId={me?.currentMainCompany?.companyId}
                setTrigger={setTrigger}
                setUserJustCreated={setUserJustCreated}
            />
        </Fragment>
    )
}