'use client';
import { iMe } from '@/app/api/consts/auth';
import { CONSTS_COMPANY } from '@/app/api/consts/company';
import { CONSTS_COMPANY_INVOICE, iCompanyInvoice } from '@/app/api/consts/company-invoice';
import { CONSTS_UTILITY, iPlanType, iPlanTypeOutput } from '@/app/api/consts/utility';
import { Fetch } from '@/app/api/fetch';
import CardCreamWithChildren from '@/app/components/card/cream-with-children';
import CardSimpleWithChildren from '@/app/components/card/simple-with-children';
import { handleLaunchConfetti } from '@/app/functions/effect.confetti';
import swal from '@/app/functions/swal';
import toast from '@/app/functions/toast';
import useApiRequestToSetterOnUrlChange from '@/app/hooks/api/useApiRequestToSetterOnUrlChange';
import Tippy from '@tippyjs/react';
import { Guid } from 'guid-typescript';
import Link from 'next/link';
import { useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function EmpresaUsoEPlanoTabPlanos({ me }: iProps) {

    const [plans, setPlans] = useState<iPlanTypeOutput | undefined>();
    useApiRequestToSetterOnUrlChange<iPlanTypeOutput>({ apiUrlRequest: CONSTS_UTILITY.getPlanType, setter: setPlans });

    return (
        <section className={styles.main}>
            <CardSimpleWithChildren style={{ backgroundColor: 'var(--cream)' }}>
                <Plans me={me} plans={plans} />
            </CardSimpleWithChildren>
        </section>
    )
}

function Plans({ me, plans }: { me: iMe | undefined, plans: iPlanTypeOutput | undefined }) {

    const messageInvoice = 'Ao adquirir um novo plano, uma nova fatura será gerada automaticamente e o plano atual será substituído.';

    async function handleChooseNewPlan(plan: iPlanType) {
        swal({
            content: `Você tem certeza de que deseja prosseguir com a aquisição do plano <b>${plan.planTypeDescription.toLocaleLowerCase()}</b>?
                <br/><br/>${messageInvoice}`,
            icon: 'success',
            mustConfirm: true,
            checkboxLabel: 'Li e estou de acordo',
            confirmBtnText: 'Confirmar',
            confirmFunction: async () => {
                const formDataInput = new FormData();
                formDataInput.append('CompanyId', me?.currentMainCompany?.companyId?.toString() ?? Guid.EMPTY);
                formDataInput.append('PlanType', plan.planType.toString());

                const output = await Fetch.put({
                    url: CONSTS_COMPANY.updatePlanType,
                    body: formDataInput,
                    isFormData: true
                }) as iCompanyInvoice;

                if (output) {
                    handleLaunchConfetti(5000);

                    swal({
                        content: `Plano adquirido com sucesso!<br/>O novo plano da sua empresa é o <b>${plan.planTypeDescription.toLowerCase()}</b>.`,
                        confirmFunction: () => {
                            handlePay(output);
                        },
                        icon: 'success',
                        confirmBtnText: 'Ok'
                    });

                    return;
                }
            },
            cancelBtnText: 'Voltar'
        });
    }

    async function handlePay(e: iCompanyInvoice) {
        const pendingPayment = 1;

        if (e.companyInvoiceSituation.toString() !== pendingPayment.toString()) {
            swal({
                content: 'Apenas faturas <b>pendentes</b> podem ser pagas.',
                icon: 'warning'
            });

            return;
        }

        const output = await Fetch.put({ url: `${CONSTS_COMPANY_INVOICE.pay}/${e.companyInvoiceId}` });

        if (output) {
            swal({
                content: 'Fatura foi paga com sucesso.',
                confirmFunction: () => window.location.reload(),
                icon: 'success'
            });

            return;
        }

        toast({
            content: 'Não foi possível pagar esta fatura. Tente novamente mais tarde.'
        });
    }

    function handleGetPlanStatus(plan: iPlanType) {
        const isCurrentPlan = plan.planType.toString() === me?.currentMainCompany?.planType?.toString();
        const isFree = plan.planTypeName === 'Free';
        const isPaymentPendent = me?.currentMainCompany?.companySituation?.toString() === '1';

        return { isCurrentPlan, isFree, isPaymentPendent };
    }

    function handleGetPlanButtonText(plan: iPlanType, status: ReturnType<typeof handleGetPlanStatus>) {
        if (status.isCurrentPlan && status.isPaymentPendent) {
            return 'Pagamento pendente';
        }

        return `Plano ${plan.planTypeDescription.toLocaleLowerCase()}`;
    }

    return (
        <section className={styles.pricingSection}>
            <CardCreamWithChildren title='Planos que crescem com você' hasBorder={false} subtitle={
                <div className={styles.headerWrapper}>
                    <Tippy content={`Lembre-se que, ${messageInvoice.toLowerCase()}`} placement='right'>
                        <p className={styles.subtitle}>
                            Sem contratos. Cancele quando quiser.
                            <span className={styles.infoIcon}>*</span>
                        </p>
                    </Tippy>
                </div>
            }>
                <div className={styles.header}>
                    <div className={styles.headerContent}>
                        <div className={styles.headerItem}>
                            <span className={styles.headerIcon}>🔒</span>
                            <div>
                                <h5>Pagamento seguro</h5>
                                <p>Seus dados protegidos com criptografia.</p>
                            </div>
                        </div>

                        <div className={styles.headerItem}>
                            <span className={styles.headerIcon}>💳</span>
                            <div>
                                <h5>Sem surpresas</h5>
                                <p>Cancele a qualquer momento, sem multas.</p>
                            </div>
                        </div>

                        <div className={styles.headerItem}>
                            <span className={styles.headerIcon}>🚀</span>
                            <div>
                                <h5>Ativação imediata</h5>
                                <p>Seu plano ativo em instantes.</p>
                            </div>
                        </div>
                    </div>
                </div>

                <div className={styles.grid}>
                    {
                        plans?.plans?.map((plan, index) => {
                            const status = handleGetPlanStatus(plan);
                            const isDisabled = status.isFree || (status.isCurrentPlan && !status.isPaymentPendent);
                            const buttonText = handleGetPlanButtonText(plan, status);

                            return (
                                <div
                                    key={index}
                                    className={`${styles.cardWrapper} ${status.isFree ? styles.notAllowed : ''} ${status.isCurrentPlan ? styles.current : ''}`}
                                >
                                    {
                                        status.isCurrentPlan && (
                                            <div className={styles.badge}>
                                                <span className={styles.badgeIcon}>✓</span>
                                                Plano atual
                                            </div>
                                        )
                                    }

                                    <div className={`${styles.card} ${styles.cardNormal}`}>
                                        <div className={styles.cardHeader}>
                                            <h3 className={styles.planHeading}>{plan.planTypeDescription}</h3>
                                            <p className={styles.planSub}>{plan.description}</p>
                                        </div>

                                        <div className={styles.priceSection}>
                                            <div className={styles.priceWrap}>
                                                <span className={styles.currency}>R$</span>
                                                <span className={styles.price}>{plan.price}</span>
                                                {!status.isFree && <span className={styles.pricePerMonth}>/mês</span>}
                                            </div>

                                            {
                                                !status.isFree && (
                                                    <p className={styles.billingInfo}>Cobrado mensalmente</p>
                                                )
                                            }
                                        </div>

                                        <div className={styles.divider}></div>

                                        <div className={styles.perksSection}>
                                            <h4 className={styles.perksTitle}>O que está incluído:</h4>
                                            <ul className={styles.perksList}>
                                                {
                                                    plan.perks.map((perk, perkIndex) => (
                                                        <li key={perkIndex} className={styles.perkItem}>
                                                            <span className={styles.perkIcon}>
                                                                <svg width='16' height='16' viewBox='0 0 16 16' fill='none'>
                                                                    <path d='M13.3333 4L6 11.3333L2.66666 8' stroke='currentColor' strokeWidth='2' strokeLinecap='round' strokeLinejoin='round' />
                                                                </svg>
                                                            </span>

                                                            <span className={styles.perkText}>{perk}</span>
                                                        </li>
                                                    ))
                                                }
                                            </ul>
                                        </div>

                                        <div className={styles.cardFooter}>
                                            <Link
                                                href='#'
                                                className={`${styles.ctaButton} ${isDisabled ? styles.disabled : ''} ${status.isCurrentPlan ? styles.current : ''} ${status.isPaymentPendent ? styles.paymentPending : ''} `}
                                                onClick={isDisabled ? undefined : () => handleChooseNewPlan(plan)}
                                            >
                                                {
                                                    status.isPaymentPendent && status.isCurrentPlan && (
                                                        <span className={styles.warningIcon}>⚠</span>
                                                    )
                                                }

                                                {buttonText}
                                            </Link>
                                        </div>
                                    </div>
                                </div>
                            )
                        })
                    }
                </div>
            </CardCreamWithChildren>
        </section>
    )
}