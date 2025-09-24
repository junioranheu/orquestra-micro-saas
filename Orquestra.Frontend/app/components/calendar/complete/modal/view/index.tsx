import iClient from '@/app/api/consts/client';
import iSchedule from '@/app/api/consts/schedule';
import { iUser } from '@/app/api/consts/user';
import { iEvent } from '@/app/components/calendar/complete';
import Button from '@/app/components/input/button';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputMask from '@/app/components/input/text';
import ModalGeneric from '@/app/components/modal/generic';
import styles from '@/app/components/modal/generic/index.module.scss';
import Tags from '@/app/components/tags';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import handleGetPropName from '@/app/functions/get.propName';
import { handleInputFormStateChange, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import { handleTransformArrayToDropdownOptionsGuid } from '@/app/functions/transform.arrayToDropdownOptions';
import useWindowSize from '@/app/hooks/useWindowSize';
import { useRouter } from 'next/navigation';
import { Dispatch, Fragment, SetStateAction, useEffect, useState } from 'react';

interface iProps {
    isOpen: boolean;
    setModalIsOpen: Dispatch<SetStateAction<boolean>>;
    event: iEvent | undefined;
    companyUsers: iUser[] | undefined;
    clients: iClient[] | undefined;
    onSave?: (updated: iEvent) => Promise<void> | void;
}

export const CONSTS_PAYMENT_TYPE = [
    { value: 1, label: 'Dinheiro' },
    { value: 2, label: 'Crédito' },
    { value: 3, label: 'Débito' },
    { value: 4, label: 'Pix' },
    { value: 5, label: 'TED' },
    { value: 6, label: 'Outro' }
] as iDropdownOption[];

export const CONSTS_SCHEDULE_STATUS = [
    { value: 1, label: 'Marcado' },
    { value: 2, label: 'Remarcado' },
    { value: 3, label: 'Concluído' },
    { value: 4, label: 'Cancelado' }
] as iDropdownOption[];

export default function ModalCalendarView({ isOpen, setModalIsOpen, event, companyUsers, clients, onSave }: iProps) {

    const router = useRouter();

    const windowSize = useWindowSize();
    const [canEdit, setCanEdit] = useState<boolean>(false);
    const [clientsDropDown, setClientsDropDown] = useState<iDropdownOption[]>();
    const [companyUsersDropDown, setCompanyUsersDropDown] = useState<iDropdownOption[]>();
    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iSchedule>({
        scheduleId: SYSTEM.EMPTY_GUID,
        date: SYSTEM.EMPTY_DATE,
        durationMinutes: 0,
        paymentType: '',
        scheduleStatus: '',
        clientId: SYSTEM.EMPTY_GUID,
        companyId: SYSTEM.EMPTY_GUID,
        usersIds: [],
        isRestrictForSpecificUsers: false,
        customTitle: '',
        customUrl: '',
        observation: '',
        amountReceived: 0,
        dateEnd: SYSTEM.EMPTY_DATE,
        observations: [],
        usersOutput: []
    });

    useEffect(() => {
        if (!isOpen) {
            return;
        }

        if (!clients || !clients.length) {
            swal({
                content: `Nenhum cliente foi registrado. Por favor, cadastre pelo menos um cliente para prosseguir.`,
                confirmBtnText: 'Cadastrar cliente',
                confirmFunction: () => { router.push(ROUTES.EMPRESA_CLIENTES) },
                icon: 'warning'
            });

            setModalIsOpen(false);
            return;
        }

        if (!event || !event.schedule || !event.schedule.client) {
            console.error(event);
            swal({ content: 'Houve uma falha ao abrir esse agendamento. Verifique o inspecionador de elementos.' });
            setModalIsOpen(false);
            return;
        }

        setFormData({
            scheduleId: event.schedule.scheduleId,
            date: event.start,
            durationMinutes: event.schedule.durationMinutes,
            paymentType: event.schedule.paymentType,
            scheduleStatus: event.schedule.scheduleStatus,
            clientId: event.schedule.clientId,
            companyId: event.schedule.companyId,
            usersIds: event.schedule.usersIds,
            isRestrictForSpecificUsers: event.schedule.isRestrictForSpecificUsers,
            customTitle: event.schedule.customTitle,
            customUrl: event.schedule.customUrl,
            observation: event.schedule.observation,
            amountReceived: event.schedule.amountReceived,
            dateEnd: event.end,
            observations: event.schedule.observations,
            usersOutput: event.schedule.usersOutput
        });

        const optionsCompanyUsers = handleTransformArrayToDropdownOptionsGuid(companyUsers ?? [], 'userId', 'user.fullName');
        setCompanyUsersDropDown(optionsCompanyUsers);

        const optionsClients = handleTransformArrayToDropdownOptionsGuid(clients ?? [], 'clientId', 'fullName');
        setClientsDropDown(optionsClients);
    }, [isOpen, event, companyUsers, clients, router, setModalIsOpen]);

    const setCompanyUsersIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.usersIds)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setClientIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.clientId)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setPaymentTypeOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.paymentType)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setScheduleStatusOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.scheduleStatus)[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    async function handleSave() {
        if (!canEdit) {
            return;
        }
        setSaving(true);

        try {
            setEditing(false);
        } catch { }
        finally {
            setSaving(false);
        }
    }

    if (!isOpen || !event) {
        return;
    }

    return (
        <ModalGeneric
            isOpen={isOpen}
            setModalIsOpen={setModalIsOpen}
            showCloseButton={false}
            allowCloseOutsideClick={false}
            width={windowSize.width <= 1281 ? '85%' : '55%'}
            style={{ padding: 0, background: 'transparent' }}
        >
            <div className={styles.modalCard}>
                <header className={styles.modalHeader}>
                    <div className={styles.modalHeaderLeft}>
                        <h1 className={styles.inputTitle}>{event.schedule.customTitle ?? event.title}</h1>
                    </div>

                    <div className={styles.modalHeaderRight}>
                        <div className={styles.metaRow}>
                            <Tags
                                tags={[
                                    { label: handleFormatDate(event.start, DATE_STYLE.DETALHADO) },
                                    { label: event.schedule?.scheduleStatus },
                                    { label: event.schedule?.paymentType }
                                ]}
                            />
                        </div>
                    </div>
                </header>

                <main className={styles.modalContent}>
                    <div className={styles.grid}>
                        <div className={styles.fieldGroup}>
                            <InputMask title='Data' type='date' objectFormData={handleGetPropName(formData, x => x.date ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />
                            <Dropdown title='Cliente' options={clientsDropDown ?? []} selectedOption={clientsDropDown?.find(x => x.value === formData.clientId)} setSelectedOption={setClientIdOption} isDisabled={!editing} />
                            <InputMask title='Início' type='time' objectFormData={handleGetPropName(formData, x => x.date ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />
                            <InputMask title='Fim' type='time' objectFormData={handleGetPropName(formData, x => x.dateEnd ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />
                            <InputMask title='Duração (min)' type='number' objectFormData={handleGetPropName(formData, x => x.durationMinutes ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />
                            <InputMask title='Observação' objectFormData={handleGetPropName(formData, x => x.observation ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />
                            <InputMask title='Valor recebido' type='number' objectFormData={handleGetPropName(formData, x => x.amountReceived ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />
                        </div>

                        <div className={styles.fieldGroup}>
                            <Dropdown title='Tipo de pagamento' options={CONSTS_PAYMENT_TYPE} selectedOption={CONSTS_PAYMENT_TYPE.find(x => x.label === formData.paymentType)!} setSelectedOption={setPaymentTypeOption} isDisabled={!editing} />
                            <Dropdown title='Status' options={CONSTS_SCHEDULE_STATUS} selectedOption={CONSTS_SCHEDULE_STATUS.find(x => x.label === formData.scheduleStatus)!} setSelectedOption={setScheduleStatusOption} isDisabled={!editing} />

                            <Dropdown
                                title='Membros'
                                options={companyUsersDropDown ?? []}
                                multiple={true}
                                // @ts-ignore;
                                selectedOption={(companyUsersDropDown ?? []).filter(option => (formData.usersIds ?? []).some(userOption => userOption.value.value === option.value.value))}
                                setSelectedOption={setCompanyUsersIdOption}
                                isDisabled={!editing}
                            />

                            <InputMask title='Título' objectFormData={handleGetPropName(formData, x => x.customTitle ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />
                            <InputMask title='URL' objectFormData={handleGetPropName(formData, x => x.customUrl ?? '')} isDisabled={!editing} handleChange={(e) => handleInputFormStateChange(e, setFormData)} />

                            <label>Observações do sistema</label>
                            <textarea className={styles.textarea} rows={3} value={formData.observations} readOnly={true} />
                        </div>
                    </div>
                </main>

                <footer className={styles.modalFooter}>
                    <div className={styles.buttonsRow}>
                        <Button label='Fechar' handleFunction={() => setModalIsOpen(false)} isStyleSimple={true} />
                    </div>

                    <div className={styles.buttonsRow}>
                        {
                            !editing ? (
                                <Fragment>
                                    <Button label='Editar' handleFunction={() => setEditing(true)} />
                                </Fragment>
                            ) : (
                                <Fragment>
                                    <Button label='Cancelar edição' handleFunction={() => setEditing(false)} isStyleSimple={true} />
                                    <Button label={saving ? 'Salvando...' : 'Salvar'} handleFunction={() => handleSave()} isDisabled={saving} />
                                </Fragment>
                            )
                        }
                    </div>
                </footer>
            </div>
        </ModalGeneric>
    )
}