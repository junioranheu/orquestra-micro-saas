'use client';
import { CONSTS_COMPANY, iCompanyOutput } from '@/app/api/consts/company';
import { CONSTS_COMPANY_USER } from '@/app/api/consts/company-user';
import { Fetch } from '@/app/api/fetch';
import SvgEmpty from '@/app/assets/svg/empty.svg';
import CardSimple from '@/app/components/card/simple';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import TemplatePageHeader from '@/app/components/template/template-page-header';
import Tippy from '@/app/components/tool-tip';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName, handleGetNameInitials } from '@/app/functions/get.formatUserName';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import { useFakeLoading } from '@/app/hooks/useFakeLoader';
import useTitle from '@/app/hooks/useTitle';
import { Guid } from 'guid-typescript';
import Image from 'next/image';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { Fragment, useEffect, useState } from 'react';
import ModalEmpresaGerenciarView from './modal/view';
import styles from './page.module.scss';

export default function EmpresaGerenciar() {

    useTitle('Gerenciar empresas');

    const router = useRouter();
    const me = useApiGetMe({});
    const isLoading = useFakeLoading();

    const planTypeEnum = useApiGetEnum({ enumName: 'PlanTypeEnum' });
    const [companies, setCompanies] = useState<iCompanyOutput[]>();

    useApiRequestToSetterOnUrlChange<iCompanyOutput[]>({
        apiUrlRequest: `${CONSTS_COMPANY.getAllByUserId}?userId=${me?.userId ?? Guid.EMPTY}&onlyStatusTrue=false`,
        setter: setCompanies
    });

    useEffect(() => {
        if (me && !me?.currentMainCompany) {
            handleOpenModal(undefined);
        }
    }, [me]);

    function handleSetCurrentMainCompany(company: iCompanyOutput) {
        swal({
            content: `Você deseja selecionar <b>${company.name}</b> como sua empresa principal?`,
            icon: 'question',
            cancelBtnText: 'Cancelar',
            confirmBtnText: 'Sim, continuar',
            confirmFunction: async () => {
                if (company.companyId === me?.currentMainCompany?.companyId) {
                    swal({ content: 'Essa já é sua empresa principal.', icon: 'warning' });
                    return;
                }

                await Fetch.put({
                    url: `${CONSTS_COMPANY_USER.updateCurrentMainCompanyUser}?companyId=${company.companyId}`
                });

                swal({
                    content: `A empresa <b>${company.name}</b> agora é a sua principal!`,
                    confirmFunction: () => window.location.reload(),
                    icon: 'success'
                });
            }
        });
    }

    const [companyClicked, setCompanyClicked] = useState<iCompanyOutput | undefined>(undefined);
    const [isModalOpen, setIsModalOpen] = useState<boolean>(false);
    const [typeModal, setTypeModal] = useState<('edit' | 'create')>('create');

    function handleOpenModal(company: iCompanyOutput | undefined) {
        setTypeModal(company ? 'edit' : 'create');
        setCompanyClicked(company);
        setIsModalOpen(true);
    }

    async function handleLeave(company: iCompanyOutput) {
        swal({
            content: 'Você tem certeza que deseja sair desta empresa?',
            confirmBtnText: 'Sim, desejo sair',
            confirmFunction: async () => {
                const input = { companyId: company.companyId, userId: me?.userId };
                const member = await Fetch.put({ url: CONSTS_COMPANY_USER.disable, body: input });

                if (member) {
                    router.push(ROUTES.DASHBOARD);

                    swal({
                        content: 'Você saiu desta empresa.',
                        confirmFunction: () => window.location.reload(),
                        icon: 'success'
                    });

                    return;
                }

                toast({ content: 'Não foi possível sair desta empresa. Tente novamente mais tarde.' });
            },
            cancelBtnText: 'Voltar',
            icon: 'question'
        });
    }

    async function handleResendVerifyEmail(company: iCompanyOutput) {
        const output = await Fetch.post({
            url: `${CONSTS_COMPANY.resendVerifyEmailCompany}/${company.companyId}`
        });

        if (output) {
            swal({
                content: 'Um novo e-mail de verificação foi enviado com sucesso.',
                icon: 'success'
            });

            return;
        }
    }

    if (isLoading) {
        return (
            <TemplatePageHeader title='Minhas empresas' isLoading={isLoading}>
            </TemplatePageHeader>
        )
    }

    return (
        <Fragment>
            <TemplatePageHeader
                title='Minhas empresas'
                isLoading={isLoading}
                actions={[
                    <Button
                        key='add'
                        label='Adicionar nova empresa'
                        handleFunction={() => handleOpenModal(undefined)}
                        icon_feather={<Icon icon='plus' size='small' />}
                    />
                ]}
            >
                <Fragment>
                    {
                        companies?.length === 0 && (
                            <CardSimple
                                img={SvgEmpty}
                                title={`${handleGetFirstName(me?.userName)}, tudo parece tão vazio por aqui...`}
                                description='Aparentemente você não faz parte de nenhuma empresa.<br/>Por que não cadastra a sua agora mesmo?'
                                buttonLabel={`Cadastrar sua empresa no ${SYSTEM.NAME}`}
                                buttonFunction={() => handleOpenModal(undefined)}
                                hasCardAltStyle={true}
                            />
                        )
                    }

                    <div className={styles.companiesGrid}>
                        {
                            companies?.sort((a, b) => a.name.localeCompare(b.name)).map((company) => {
                                const isCurrentMain = me?.currentMainCompany?.companyId === company.companyId;
                                const isAdmin = company?.isAdm;
                                const isOwner = company?.isOwner;
                                const isValidated = company.status;

                                return (
                                    <article key={company.companyId.toString()} className={`${styles.companyCard} ${isCurrentMain ? styles.mainCompany : ''}`}>
                                        <div className={styles.cardTop}>
                                            <div className={styles.companyHeader}>
                                                <div className={styles.companyAvatar}>
                                                    {
                                                        company.logoBase64 ? (
                                                            <Image
                                                                src={decodeURIComponent(company.logoBase64)}
                                                                alt={company.name}
                                                                priority={true}
                                                                width={56}
                                                                height={56}
                                                            />
                                                        ) : (
                                                            <span className={styles.avatarInitials}>
                                                                {handleGetNameInitials(company.name)}
                                                            </span>
                                                        )
                                                    }
                                                </div>

                                                <div className={styles.companyTitleArea}>
                                                    <h3 className={styles.companyTitle}>{company.name}</h3>
                                                    <p className={styles.companySubtitle}>{company.companyTypeStr}</p>
                                                </div>
                                            </div>

                                            <div className={styles.badgeContainer}>
                                                {
                                                    isCurrentMain && (
                                                        <Tippy content='Sua empresa principal.'>
                                                            <span className={`${styles.statusBadge} ${styles.badgeMain}`}>
                                                                <Icon icon='star' size='small' />
                                                                <span>Empresa principal</span>
                                                            </span>
                                                        </Tippy>
                                                    )
                                                }

                                                {
                                                    isAdmin && (
                                                        <Tippy content='Você é administrador desta empresa.'>
                                                            <span className={`${styles.statusBadge} ${styles.badgeAdmin}`}>
                                                                <Icon icon='shield' size='small' />
                                                                <span>Admin</span>
                                                            </span>
                                                        </Tippy>
                                                    )
                                                }

                                                {
                                                    isOwner && (
                                                        <Tippy content='Você é o proprietário.'>
                                                            <span className={`${styles.statusBadge} ${styles.badgeOwner}`}>
                                                                <Icon icon='award' size='small' />
                                                                <span>Dono</span>
                                                            </span>
                                                        </Tippy>
                                                    )
                                                }
                                            </div>
                                        </div>

                                        <div className={styles.cardContent}>
                                            <div className={styles.infoGrid}>
                                                <div className={styles.infoItem}>
                                                    <span className={styles.infoLabel}>Plano:</span>
                                                    {
                                                        isCurrentMain ? (
                                                            <Link href={ROUTES.EMPRESA_USO_E_PLANO} className={styles.infoValueLink}>
                                                                {planTypeEnum?.find(x => x.value.toString() === company.planType?.toString())?.label}
                                                            </Link>
                                                        ) : (
                                                            <span className={styles.infoValue}>
                                                                {planTypeEnum?.find(x => x.value.toString() === company.planType?.toString())?.label}
                                                            </span>
                                                        )
                                                    }
                                                </div>

                                                <div className={styles.infoItem}>
                                                    <span className={styles.infoLabel}>E-mail:</span>

                                                    <Tippy content={company.email.toLowerCase()} placement='bottom'>
                                                        <span className={`${styles.infoValue} ${styles.email}`} style={{ cursor: 'help' }}>
                                                            {company.email.toLowerCase()}
                                                        </span>
                                                    </Tippy>
                                                </div>

                                                {
                                                    company.planStartDate && (
                                                        <div className={styles.infoItem}>
                                                            <span className={styles.infoLabel}>Início:</span>
                                                            <span className={styles.infoValue}>
                                                                {new Date(company.planStartDate).toLocaleDateString()}
                                                            </span>
                                                        </div>
                                                    )
                                                }

                                                {
                                                    company.planEndDate && (
                                                        <div className={styles.infoItem}>
                                                            <span className={styles.infoLabel}>Fim:</span>
                                                            <span className={styles.infoValue}>
                                                                {new Date(company.planEndDate).toLocaleDateString()}
                                                            </span>
                                                        </div>
                                                    )
                                                }

                                                <div className={styles.infoItem}>
                                                    <span className={styles.infoLabel}>Colaboradores:</span>

                                                    {
                                                        isCurrentMain ? (
                                                            <Link href={ROUTES.EMPRESA_COLABORADORES} className={styles.infoValueLink}>
                                                                {company?.companyUsers?.length}
                                                            </Link>
                                                        ) : (
                                                            <span className={styles.infoValue}>{company?.companyUsers?.length}</span>
                                                        )
                                                    }
                                                </div>

                                                <div className={styles.infoItem}>
                                                    <span className={styles.infoLabel}>Clientes:</span>

                                                    {
                                                        isCurrentMain ? (
                                                            <Link href={ROUTES.EMPRESA_CLIENTES} className={styles.infoValueLink}>
                                                                {company.amountOfClients}
                                                            </Link>
                                                        ) : (
                                                            <span className={styles.infoValue}>{company.amountOfClients}</span>
                                                        )
                                                    }
                                                </div>
                                            </div>

                                            {
                                                !isValidated && (
                                                    <div className={styles.warningBox}>
                                                        <Icon icon='alert-circle' size='small' />

                                                        <div className={styles.warningContent}>
                                                            <strong>Empresa pendente de validação</strong>

                                                            {
                                                                isAdmin && (
                                                                    <button
                                                                        className={styles.warningLink}
                                                                        onClick={() => handleResendVerifyEmail(company)}
                                                                    >
                                                                        Reenviar e-mail de verificação
                                                                    </button>
                                                                )
                                                            }
                                                        </div>
                                                    </div>
                                                )
                                            }
                                        </div>

                                        <div className={styles.cardActions}>
                                            <Button
                                                label='Ver detalhes'
                                                handleFunction={() => handleOpenModal(company)}
                                                styleType='transparent'
                                                icon_feather={<Icon icon='search' size='small' />}
                                            />

                                            {
                                                !isCurrentMain && (
                                                    <Button
                                                        label='Tornar principal'
                                                        handleFunction={() => handleSetCurrentMainCompany(company)}
                                                        styleType='transparent'
                                                        icon_feather={<Icon icon='star' size='small' />}
                                                    />
                                                )
                                            }

                                            {
                                                !isOwner && (
                                                    <Button
                                                        label='Sair'
                                                        handleFunction={() => handleLeave(company)}
                                                        styleType='red'
                                                        icon_feather={<Icon icon='log-out' size='small' />}
                                                    />
                                                )
                                            }
                                        </div>
                                    </article>
                                )
                            })
                        }
                    </div>
                </Fragment>
            </TemplatePageHeader>

            <ModalEmpresaGerenciarView
                isModalOpen={isModalOpen}
                setIsModalOpen={setIsModalOpen}
                type={typeModal}
                company={companyClicked}
            />
        </Fragment>
    )
}