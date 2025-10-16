'use client';
import { CONSTS_COMPANY_USER, iCompanyUser, iCompanyUserPaginated } from '@/app/api/consts/company-user';
import { iUser } from '@/app/api/consts/user';
import { Fetch } from '@/app/api/fetch';
import Icon from '@/app/components/icon';
import TableGeneric, { iTableColumn, iTableManagingOptions } from '@/app/components/table/generic';
import ROUTES from '@/app/consts/routes';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetCompanySituationEnum from '@/app/hooks/api/enums/useApiGetCompanySituationEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import { useRouter } from 'next/navigation';
import { Fragment, useEffect, useState } from 'react';
import EmpresaMembrosModalFilters, { iCompanyUserFormDataModalFilter } from './modal/filter';
import EmpresaMembrosModalView from './modal/view';
import styles from './page.module.scss';

export default function EmpresaMembros() {

    useTitle('Membros da empresa');
    const router = useRouter();
    const me = useApiGetMe({});

    const companyUserRoleEnum = useApiGetCompanySituationEnum({ enumName: 'CompanyUserRoleEnum' });
    const moduleEnum = useApiGetCompanySituationEnum({ enumName: 'ModuleEnum' });

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

    const [isModalViewOpen, setIsModalViewOpen] = useState<boolean>(false);

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
            label: 'Remover membro',
            function: (e) => handleDisable(e),
            icon: <Icon icon='user-x' />
        }
    ] as iTableManagingOptions[];

    async function handleDisable(member: iUser) {
        const isSameUser = member.userId === me?.userId;

        swal({
            content: isSameUser ? 'Você tem certeza que deseja sair desta empresa?' : 'Você tem certeza que deseja remover este membro?',
            confirmBtnText: isSameUser ? 'Sim, desejo sair' : 'Sim, desejo remover',
            confirmFunction: async () => {
                const input = { companyId: me?.currentMainCompany?.companyId, userId: member.userId };
                const schedule = await Fetch.put({ url: CONSTS_COMPANY_USER.disable, body: input });

                if (schedule) {
                    if (!isSameUser) {
                        toast({ content: 'Membro removido da equipe com sucesso.' });
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

                toast({ content: isSameUser ? 'Não foi possível sair desta empresa. Tente novamente mais tarde.' : 'Não foi possível remover este membro da equipe. Tente novamente mais tarde.' });
            },
            cancelBtnText: 'Voltar',
            icon: 'question'
        });
    }

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

                    title={`Membros cadastrados em ${me?.currentMainCompany?.name ?? ''}`}
                    managingOptions={managingOptions}
                    btn_add_label='Convidar novo membro'
                    btn_add_function={() => setIsModalViewOpen(true)}
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
                companyId={me?.currentMainCompany?.companyId}
            />
        </Fragment>
    )
}