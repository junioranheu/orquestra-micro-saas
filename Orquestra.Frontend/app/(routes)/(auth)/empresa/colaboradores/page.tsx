'use client';
import { CONSTS_COMPANY_USER, iCompanyUser, iCompanyUserPaginated } from '@/app/api/consts/company-user';
import { iUser } from '@/app/api/consts/user';
import { Fetch } from '@/app/api/fetch';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import ROUTES from '@/app/consts/routes';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { Fragment, useEffect, useState } from 'react';
import EmpresaMembrosModalEdit from './modal/edit';
import EmpresaMembrosModalFilters, { iCompanyUserFormDataModalFilter } from './modal/filter';
import EmpresaMembrosModalInvite from './modal/invite';

export default function EmpresaMembros() {

    useTitle('Colaboradores');
    const router = useRouter();
    const me = useApiGetMe({});

    const companyUserRoleEnum = useApiGetEnum({ enumName: 'CompanyUserRoleEnum' });
    const moduleEnum = useApiGetEnum({ enumName: 'ModuleEnum' });

    const [currentPage, setCurrentPage] = useState<number>(1);
    const [members, setMembers] = useState<iCompanyUserPaginated>();
    const [membersNormalized, setMembersNormalized] = useState<iCompanyUser[]>([]);

    const [trigger, setTrigger] = useState<Date>(new Date());
    const [apiUrlRequest, setApiUrlRequest] = useState<string>(CONSTS_COMPANY_USER.getAllByCompanyId);
    useApiRequestToSetterOnUrlChange<iCompanyUserPaginated>({ apiUrlRequest: apiUrlRequest, setter: setMembers, hasPaginationInput: true, index: currentPage, limit: 15, trigger: trigger });

    useEffect(() => {
        if (me && me?.currentMainCompany?.companyId) {
            setApiUrlRequest(`${CONSTS_COMPANY_USER.getAllByCompanyId}?companyId=${me?.currentMainCompany?.companyId}`);
        }
    }, [me]);

    useEffect(() => {
        if (moduleEnum && companyUserRoleEnum) {
            const normalized = members?.output?.map(member => {
                const moduleLabels = (member.userModules as (number | string)[] | undefined)?.map(mod => {
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
            key: 'modules',
            render: (modules: string | undefined, record) => {
                if (record.companyUserRole === 'Administrador') {
                    return 'Todos';
                }

                return modules;
            }
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
            label: 'Editar colaborador',
            function: (e) => handleOpenModalEdit(e),
            icon: <Icon icon='edit' />
        },
        ...(me?.isUserAdmOfCurrentMainCompany ? [
            {
                label: 'Remover colaborador',
                function: (e: iUser) => handleDisable(e),
                icon: <Icon icon='user-x' />
            }
        ] : [])
    ] as iTableManagingOptions[];

    const [isModalInviteOpen, setIsModalInviteOpen] = useState<boolean>(false);
    const [isModalEditOpen, setIsModalEditOpen] = useState<boolean>(false);
    const [userClicked, setUserClicked] = useState<iCompanyUser | undefined>(undefined);

    function handleOpenModalEdit(user: iCompanyUser | undefined) {
        if (!me?.isUserAdmOfCurrentMainCompany) {
            toast({ content: 'Apenas os administradores da empresa podem realizar modificações nos colaboradores.' });
            return;
        }

        setUserClicked(user);
        setIsModalEditOpen(true);
    }

    async function handleDisable(member: iUser) {
        const isSameUser = member.userId === me?.userId;

        swal({
            content: isSameUser ? 'Você tem certeza que deseja sair desta empresa?' : 'Você tem certeza que deseja remover este colaborador?',
            confirmBtnText: isSameUser ? 'Sim, desejo sair' : 'Sim, desejo remover',
            confirmFunction: async () => {
                const input = { companyId: me?.currentMainCompany?.companyId, userId: member.userId };
                const output = await Fetch.put({ url: CONSTS_COMPANY_USER.disable, body: input });

                if (output) {
                    if (!isSameUser) {
                        toast({ content: 'Colaborador removido da equipe com sucesso.' });
                    }

                    if (isSameUser) {
                        router.push(ROUTES.DASHBOARD);

                        swal({
                            content: 'Você saiu desta empresa.',
                            confirmFunction: () => window.location.reload(),
                            icon: 'success'
                        });

                        return;
                    }

                    setTrigger(new Date());
                    return;
                }

                toast({ content: isSameUser ? 'Não foi possível sair desta empresa. Tente novamente mais tarde.' : 'Não foi possível remover este colaborador da equipe. Tente novamente mais tarde.' });
            },
            cancelBtnText: 'Voltar',
            icon: 'question'
        });
    }

    return (
        <Fragment>
            <TemplatePageHeader
                title='Colaboradores cadastrados'
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
                            label='Convidar novo colaborador'
                            handleFunction={() => setIsModalInviteOpen(true)}
                            icon_feather={<Icon icon='plus-circle' size='small' />}
                        />
                    )
                ]}
            >
                <TableGeneric
                    idPropName='companyUserId'
                    columns={columns}
                    data={membersNormalized ?? []}
                    currentPage={currentPage}
                    setCurrentPage={setCurrentPage}
                    totalRowsCount={members?.count}
                    managingOptions={managingOptions}
                    modalFilterFormData={modalFilterFormData}
                    setModalFilterFormData={setModalFilterFormData}
                    apiUrlRequest={apiUrlRequest}
                    setApiUrlRequest={setApiUrlRequest}
                />
            </TemplatePageHeader>

            <EmpresaMembrosModalFilters
                isModalOpen={isModalFilterOpen}
                setIsModalOpen={setIsModalFilterOpen}
                modalFilterFormData={modalFilterFormData}
                setModalFilterFormData={setModalFilterFormData}
                apiUrlRequest={apiUrlRequest}
                setApiUrlRequest={setApiUrlRequest}
                setCurrentPage={setCurrentPage}
                companyUserRoleEnum={companyUserRoleEnum}
                moduleEnum={moduleEnum}
            />

            <EmpresaMembrosModalEdit
                isModalOpen={isModalEditOpen}
                setIsModalOpen={setIsModalEditOpen}
                user={userClicked}
                setTrigger={setTrigger}
                companyUserRoleEnum={companyUserRoleEnum}
                moduleEnum={moduleEnum}
            />

            <EmpresaMembrosModalInvite
                isModalOpen={isModalInviteOpen}
                setIsModalOpen={setIsModalInviteOpen}
                companyId={me?.currentMainCompany?.companyId}
            />
        </Fragment>
    )
}