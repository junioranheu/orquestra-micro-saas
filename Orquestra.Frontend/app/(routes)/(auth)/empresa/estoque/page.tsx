'use client';
import { CONSTS_INVENTORY, iInventory, iInventoryPaginated } from '@/app/api/consts/inventory';
import { Fetch } from '@/app/api/fetch';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TableGeneric, { iTableManagingOptions } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { Dispatch, Fragment, SetStateAction, useEffect, useState } from 'react';
import EmpresaEstoqueModalFilters from './modal/filter';
import EmpresaEstoqueModalView from './modal/view';

export default function EmpresaEstoque() {

    useTitle('Estoque');
    const me = useApiGetMe({});

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [items, setItems] = useState<iInventoryPaginated>();

    const [trigger, setTrigger] = useState<Date>(new Date());
    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_INVENTORY.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iInventoryPaginated>({ apiUrlRequest: apiUrlRequest, setter: setItems, hasPaginationInput: true, index: currentPage, limit: 15, trigger: trigger });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_INVENTORY.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    const [isModalFilterOpen, setIsModalFilterOpen] = useState<boolean>(false);
    const [modalFilterFormData, setModalFilterFormData] = useState<iInventory>({ name: null });

    const columns = [
        {
            title: 'Nome',
            dataIndex: 'name',
            key: 'name',
        },
        {
            title: 'Descrição',
            dataIndex: 'description',
            key: 'description',
        },
        {
            title: 'Quantidade',
            dataIndex: 'quantity',
            key: 'quantity',
        },
        {
            title: 'Preço unitário',
            dataIndex: 'unitPrice',
            key: 'unitPrice',
            render: (value?: number) =>
                value !== undefined && value !== null
                    ? value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
                    : '-',
        },
        {
            title: 'Valor total',
            dataIndex: 'totalValue',
            key: 'totalValue',
            render: (value?: number) =>
                value !== undefined && value !== null
                    ? value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' })
                    : '-',
        }
    ];

    const managingOptions = [
        {
            label: 'Editar item',
            function: (e) => handleOpenModalView(e),
            icon: <Icon icon='edit' />
        },
        ...(me?.isUserAdmOfCurrentMainCompany ? [
            {
                label: 'Remover item',
                function: (e: iInventory) => handleDisable(e, setTrigger),
                icon: <Icon icon='x' />
            }
        ] : [])
    ] as iTableManagingOptions[];

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);
    const [itemClicked, setItemClicked] = useState<iInventory | undefined>(undefined);
    const [typeModal, setTypeModal] = useState<('edit' | 'create')>('create');

    function handleOpenModalView(item: iInventory | undefined) {
        setTypeModal(item ? 'edit' : 'create');
        setItemClicked(item);
        setIsModalViewOpen(true);
    }

    useEffect(() => {
        console.log(items);
    }, [items]);

    return (
        <Fragment>
            <TemplatePageHeader
                title='Itens cadastrados'
                actions={[
                    <Button
                        key='search'
                        label='Filtrar'
                        isStyleSimple={true}
                        handleFunction={() => setIsModalFilterOpen(true)}
                        icon_feather={<Icon icon='search' size='small' />}
                    />,
                    me?.isUserAdmOfCurrentMainCompany && (
                        <Button
                            key='add'
                            label='Cadastrar novo item'
                            handleFunction={() => handleOpenModalView(undefined)}
                            icon_feather={<Icon icon='plus-circle' size='small' />}
                        />
                    )
                ]}
            >
                <TableGeneric
                    idPropName='inventoryId'
                    columns={columns}
                    data={items?.output ?? []}
                    currentPage={currentPage}
                    setCurrentPage={setCurrentPage}
                    totalRowsCount={items?.count}
                    managingOptions={managingOptions}
                    modalFilterFormData={modalFilterFormData}
                    setModalFilterFormData={setModalFilterFormData}
                    apiUrlRequest={apiUrlRequest}
                    setApiUrlRequest={setApiUrlRequest}
                />
            </TemplatePageHeader>

            <EmpresaEstoqueModalFilters
                isModalOpen={isModalFilterOpen}
                setIsModalOpen={setIsModalFilterOpen}
                modalFilterFormData={modalFilterFormData}
                setModalFilterFormData={setModalFilterFormData}
                apiUrlRequest={apiUrlRequest}
                setApiUrlRequest={setApiUrlRequest}
                setCurrentPage={setCurrentPage}
            />

            <EmpresaEstoqueModalView
                isModalOpen={isModalViewOpen}
                setIsModalOpen={setIsModalViewOpen}
                type={typeModal}
                item={itemClicked}
                companyId={me?.currentMainCompany?.companyId}
                setTrigger={setTrigger}
            />
        </Fragment>
    )
}

export async function handleDisable(item: iInventory, setTrigger: Dispatch<SetStateAction<Date>>) {
    swal({
        content: 'Você tem certeza que deseja remover este item?',
        confirmBtnText: 'Sim, desejo remover',
        confirmFunction: async () => {
            const inventory = await Fetch.put({ url: `${CONSTS_INVENTORY.disable}?inventoryId=${item.inventoryId}` });

            if (inventory) {
                toast({ content: 'Item removido com sucesso.' });
                setTrigger(new Date());
                return;
            }

            toast({ content: 'Não foi possível remover este item. Tente novamente mais tarde.' });
        },
        cancelBtnText: 'Voltar',
        icon: 'question'
    });
}