'use client';
import { CONSTS_COMPANY_USER, iCompanyUser, iCompanyUserPaginated } from '@/app/api/consts/company-user';
import { iUser } from '@/app/api/consts/user';
import Icon from '@/app/components/icon';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import useApiGetCompanySituationEnum from '@/app/hooks/api/enums/useApiGetCompanySituationEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { Fragment, useEffect, useState } from 'react';
import EmpresaMembrosModalFilters, { iCompanyUserFormDataModalFilter } from './modal/filter';
import EmpresaMembrosModalView from './modal/view';
import styles from './page.module.scss';

export default function EmpresaMembros() {

    useTitle('Membros da empresa');
    const me = useApiGetMe({});

    const companyUserRoleEnum = useApiGetCompanySituationEnum({ enumName: 'CompanyUserRoleEnum' });
    const moduleEnum = useApiGetCompanySituationEnum({ enumName: 'ModuleEnum' });

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [members, setMembers] = useState<iCompanyUserPaginated>();
    const [membersNormalized, setMembersNormalized] = useState<iCompanyUser[]>([]);

    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_COMPANY_USER.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iCompanyUserPaginated>({ apiUrlRequest: apiUrlRequest, setter: setMembers, hasPaginationInput: true, index: currentPage, limit: 15 });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_COMPANY_USER.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    useEffect(() => {
        if (moduleEnum && companyUserRoleEnum && members) {
            const normalized = members?.output?.map(member => {
                const moduleLabels = (member.modules as (number | string)[] | undefined)?.map(mod => {
                    const modNumber = Number(mod);
                    return moduleEnum.find(x => x.value === modNumber)?.label ?? modNumber;
                }) ?? [];

                return {
                    ...member,
                    modules: moduleLabels.join('; '),
                    companyUserRole: companyUserRoleEnum?.find(x => x.value === member.companyUserRole)?.label ?? member.companyUserRole
                };
            }) ?? [];

            setMembersNormalized(normalized);
        }
    }, [moduleEnum, companyUserRoleEnum, members]);

    const [isModalFilterOpen, setIsModalFilterOpen] = useState<boolean>(false);

    const [modalFilterFormData, setModalFilterFormData] = useState<iCompanyUserFormDataModalFilter>({
        companyUserRole: null, modules: null, fullName: null, email: null
    });

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);
    const [memberClicked, setMemberClicked] = useState<iUser | undefined>(undefined);
    const [typeModal, setTypeModal] = useState<('edit' | 'create')>('create');

    function handleOpenModalView(member: iUser | undefined) {
        setTypeModal(member ? 'edit' : 'create');
        setMemberClicked(member);
        setIsModalViewOpen(true);
    }

    const columns = [
        {
            title: 'Nome completo',
            dataIndex: 'user.fullName',
            key: 'fullName'
        },
        {
            title: 'E-mail',
            dataIndex: 'user.email',
            key: 'email'
        },
        {
            title: 'Tipo',
            dataIndex: 'companyUserRole',
            key: 'companyUserRole'
        },
        {
            title: 'Módulos atribuídos',
            dataIndex: 'modules',
            key: 'modules'
        },
        {
            title: 'Data de registro',
            dataIndex: 'createdDate',
            key: 'createdDate',
            render: (value?: Date) => value ? new Date(value).toLocaleDateString('pt-BR') : '-'
        },
        {
            title: 'Dono?',
            dataIndex: 'isOwner',
            key: 'isOwner',
            render: (value?: boolean) => value ? 'Sim' : 'Não'
        },
    ] as iTableColumn[];

    const managingOptions = [
        {
            label: 'Editar membro',
            function: (e) => handleOpenModalView(e),
            icon: <Icon icon='edit' />
        }
    ] as iTableManagingOptions[];

    return (
        <Fragment>
            <section className={styles.main}>
                <TableGeneric
                    idPropName='companyUserId'
                    columns={columns}
                    data={membersNormalized ?? []}
                    currentPage={currentPage}
                    setCurrentPage={setCurrentPage}
                    totalRowsCount={members?.count}
                    handleTableRowClick={(e) => handleOpenModalView(e)}

                    title={`Membros cadastrados em ${me?.currentMainCompany?.name ?? ''}`}
                    managingOptions={managingOptions}
                    btn_add_label='Cadastrar novo'
                    btn_add_function={() => handleOpenModalView(undefined)}
                    btn_filter_label='Filtrar'
                    btn_filter_function={() => setIsModalFilterOpen(true)}

                    modalFilterFormData={modalFilterFormData}
                    setModalFilterFormData={setModalFilterFormData}
                    apiUrlRequest={apiUrlRequest}
                    setApiUrlRequest={setApiUrlRequest}
                />
            </section>

            <EmpresaMembrosModalFilters
                isModalOpen={isModalFilterOpen}
                setIsModalOpen={setIsModalFilterOpen}
                modalFilterFormData={modalFilterFormData}
                setModalFilterFormData={setModalFilterFormData}
                apiUrlRequest={apiUrlRequest}
                setApiUrlRequest={setApiUrlRequest}
                setCurrentPage={setCurrentPage}
            />

            <EmpresaMembrosModalView
                isModalOpen={isModalViewOpen}
                setIsModalOpen={setIsModalViewOpen}
                type={typeModal}
                member={memberClicked}
                companyId={me?.currentMainCompany?.companyId}
            />
        </Fragment>
    )
}