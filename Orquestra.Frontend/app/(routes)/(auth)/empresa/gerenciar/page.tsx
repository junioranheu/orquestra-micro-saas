'use client';
import iCompanyOutput, { CONSTS_COMPANY } from '@/app/api/consts/company';
import { CONSTS_COMPANY_USER } from '@/app/api/consts/company-user';
import { Fetch } from '@/app/api/fetch';
import SvgOne from '@/app/assets/svg/one.svg';
import SvgTwo from '@/app/assets/svg/two.svg';
import SvgUserArrow from '@/app/assets/svg/user-arrow.svg';
import CardSimple from '@/app/components/card/simple';
import { ContentLoaderCardGrid } from '@/app/components/content-loader/card';
import ContentLoaderGrid from '@/app/components/content-loader/grid';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { handleGetFirstName, handleGetNameInitials } from '@/app/functions/get.formatUserName';
import handleGetRandomNumber from '@/app/functions/get.randomNumber';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiGetEnum from '@/app/hooks/api/useApiGetEnum';
import useApiGetMe from '@/app/hooks/api/useApiGetMe';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import useTitle from '@/app/hooks/useTitle';
import Tippy from '@tippyjs/react';
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

    const planTypeEnum = useApiGetEnum({ enumName: 'PlanTypeEnum' });
    const [companies, setCompanies] = useState<iCompanyOutput[]>();

    useApiRequestToSetterOnUrlChange<iCompanyOutput[]>({ apiUrlRequest: `${CONSTS_COMPANY.getAllByUserId}?userId=${me?.userId ?? Guid.EMPTY}&onlyStatusTrue=false`, setter: setCompanies });
    const [isLoading, setIsLoading] = useState<boolean>(true);

    useEffect(() => {
        const timer = setTimeout(() => {
            setIsLoading(false);
        }, handleGetRandomNumber(500, 1500));

        return () => clearTimeout(timer);
    }, []);

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

                await Fetch.put({ url: `${CONSTS_COMPANY_USER.updateCurrentMainCompanyUser}?companyId=${company.companyId}` });

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
                const schedule = await Fetch.put({ url: CONSTS_COMPANY_USER.disable, body: input });

                if (schedule) {
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

    if (!companies || isLoading) {
        return (
            <section className={styles.main}>
                <ContentLoaderCardGrid />
                <ContentLoaderGrid row={1} />
            </section>
        )
    }

    return (
        <Fragment>
            <section className={styles.main}>
                {
                    (companies && companies?.length) ? (
                        <div className={styles.flex}>
                            <CardSimple
                                img={SvgOne}
                                title={me?.currentMainCompany ? `${handleGetFirstName(me?.userName)}, <span class="mainColor">${me?.currentMainCompany?.name}</span> é sua empresa principal.` : 'Escolha uma empresa abaixo para ser sua principal.'}
                                description={`Você atualmente faz parte de <b>${companies?.length} empresa${companies?.length > 1 ? 's' : ''}</b>.${(companies?.length > 1 ? '<br/>Escolha abaixo outra empresa para torná-la sua principal.<br/>Essa escolha pode ser alterada a qualquer momento!' : '')}`}
                                style={{ minHeight: '100%' }}
                                hasCardAltStyle={true}
                            />

                            <CardSimple
                                img={SvgTwo}
                                title='Se quiser, cadastre uma nova empresa!'
                                description={`Tem ou faz parte de outra empresa?<br/>Cadastre-a agora mesmo no ${SYSTEM.NAME}.`}
                                buttonLabel='Cadastrar nova empresa'
                                buttonFunction={() => handleOpenModal(undefined)}
                                style={{ minHeight: '100%' }}
                                hasCardAltStyle={true}
                            />
                        </div>
                    ) : (
                        <CardSimple
                            img={SvgUserArrow}
                            title={`${handleGetFirstName(me?.userName)}, tudo parece tão vazio por aqui...`}
                            description='Aparentemente você não faz parte de nenhuma empresa.<br/>Por que não cadastra a sua agora mesmo?'
                            buttonLabel={`Cadastrar sua empresa no ${SYSTEM.NAME}`}
                            buttonFunction={() => handleOpenModal(undefined)}
                            hasCardAltStyle={true}
                        />
                    )
                }

                <div className={styles.grid}>
                    {
                        companies?.sort((a, b) => a.name.localeCompare(b.name)).map((company) => (
                            <article key={company.companyId.toString()} className={styles.card}>
                                <header className={styles.header}>
                                    <div className={styles.avatar}>
                                        {
                                            company.logoBase64 ? (
                                                <Image src={decodeURIComponent(company.logoBase64)} alt={company.name} priority={true} width={0} height={0} />
                                            ) : (
                                                <span>{handleGetNameInitials(company.name)}</span>
                                            )
                                        }
                                    </div>

                                    <div className={styles.info}>
                                        <h3>{company.name}</h3>
                                        <p className={styles.type}>{company.companyTypeStr}</p>
                                    </div>

                                    {
                                        me?.currentMainCompany?.companyId === company.companyId && (
                                            <Tippy content='Essa é sua empresa princial atualmente'>
                                                <span className={styles.badge}>
                                                    <Icon icon='star' size='small' />
                                                </span>
                                            </Tippy>
                                        )
                                    }
                                </header>

                                <div className={styles.content}>
                                    <Tippy content='Gerenciar plano'>
                                        <p>
                                            <Icon icon='layers' size='small' /> <Link href={ROUTES.EMPRESA_USO_E_PLANO}>Plano {planTypeEnum?.find(x => x.value.toString() === company.planType?.toString())?.label?.toLocaleLowerCase()} ({company.companySituationStr?.toLocaleLowerCase()})</Link>
                                        </p>
                                    </Tippy>

                                    {
                                        company.planStartDate && company.planEndDate && (
                                            <Tippy content='Vigência do plano atual'>
                                                <p>
                                                    <Icon icon='calendar' size='small' />{' '}
                                                    {new Date(company.planStartDate).toLocaleDateString()} →{' '}
                                                    {new Date(company.planEndDate).toLocaleDateString()}
                                                </p>
                                            </Tippy>
                                        )
                                    }

                                    {
                                        company.status ? (
                                            <p>
                                                <Icon icon='check-circle' size='small' /> Empresa validada
                                            </p>
                                        ) : (
                                            <Tippy content={company?.isAdm
                                                ? 'Verifique o e-mail enviado para concluir a validação da empresa.'
                                                : 'Pendente de validação. Peça ao administrador para que verifique o e-mail enviado para concluir a validação da empresa.'}>
                                                <p>
                                                    <Icon icon='x-circle' size='small' /> <b>Empresa pendente de validação</b>
                                                </p>
                                            </Tippy>
                                        )
                                    }

                                    <p>
                                        <Icon icon='mail' size='small' /> {company.email.toLocaleLowerCase()}
                                    </p>

                                    <Tippy content='Gerenciar clientes'>
                                        <p>
                                            <Icon icon='users' size='small' /> <Link href={ROUTES.EMPRESA_CLIENTES}>{company.amountOfClients} cliente{company.amountOfClients === 1 ? '' : 's'}</Link>
                                        </p>
                                    </Tippy>

                                    {
                                        me?.currentMainCompany?.companyId == company.companyId && (
                                            <p>
                                                <Icon icon='star' size='small' /> Empresa principal
                                            </p>
                                        )
                                    }

                                    {
                                        company?.isAdm && (
                                            <p>
                                                <Icon icon='shield' size='small' /> Você é um dos administradores
                                            </p>
                                        )
                                    }

                                    {
                                        company?.isOwner && (
                                            <p>
                                                <Icon icon='award' size='small' /> Você é o dono
                                            </p>
                                        )
                                    }
                                </div>

                                <div className={styles.actions}>
                                    {
                                        !company.isOwner && (
                                            <Button label='Desvincular da empresa' handleFunction={() => handleLeave(company)} isStyleSimple={true} />
                                        )
                                    }

                                    {
                                        (company?.isAdm && me?.currentMainCompany?.companyId === company.companyId) && (
                                            <Button label='Editar' handleFunction={() => handleOpenModal(company)} isStyleSimple={true} />
                                        )
                                    }

                                    {
                                        me?.currentMainCompany?.companyId !== company.companyId && (
                                            <Button label='Tornar empresa principal' handleFunction={() => handleSetCurrentMainCompany(company)} isStyleSimple={true} />
                                        )
                                    }
                                </div>
                            </article>
                        ))
                    }
                </div>
            </section>

            <ModalEmpresaGerenciarView
                isModalOpen={isModalOpen}
                setIsModalOpen={setIsModalOpen}
                type={typeModal}
                company={companyClicked}
            />
        </Fragment>
    )
}