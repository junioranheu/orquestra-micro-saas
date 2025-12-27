'use client';
import { CONSTS_CLIENT_FOLLOW_UP, iClientFollowUp } from '@/app/api/consts/client-follow-up';
import { iSchedule } from '@/app/api/consts/schedule';
import { Fetch } from '@/app/api/fetch';
import Dropdown, { iDropdownOption } from '@/app/components/input/drop-down';
import InputImage from '@/app/components/input/image';
import ModalGeneric from '@/app/components/modal/generic';
import ModalGenericFooter from '@/app/components/modal/generic/footer/footer';
import styles from '@/app/components/modal/generic/index.module.scss';
import TagList from '@/app/components/tags/tag-list';
import SYSTEM from '@/app/consts/system';
import { DATE_STYLE, handleFormatDate } from '@/app/functions/format.date';
import handleGetPropName from '@/app/functions/get.propName';
import { handleClearFormData, handleLoopFormData, handleSetDropdownOption } from '@/app/functions/set.formState';
import swal from '@/app/functions/swal';
import swalLoading from '@/app/functions/swal.loading';
import { handleTransformArrayToDropdownOptionsGuid } from '@/app/functions/transform.arrayToDropdownOptions';
import { handleConvertBase64ListToFiles } from '@/app/functions/transform.base64';
import { Guid } from 'guid-typescript';
import { Dispatch, SetStateAction, useCallback, useEffect, useState } from 'react';

interface iProps {
    isModalOpen: boolean;
    setIsModalOpen: Dispatch<SetStateAction<boolean>>;
    type: 'edit' | 'create';
    clientId: Guid | undefined;
    followUpClicked: iClientFollowUp | undefined;
    clientFollowUpStatusEnum: iDropdownOption<string | number | Guid>[] | undefined;
    setTrigger: Dispatch<SetStateAction<Date>>;
    schedules: iSchedule[];
}

export default function EmpresaClientesModalFollowUp({ isModalOpen, setIsModalOpen, type, clientId, followUpClicked, clientFollowUpStatusEnum, setTrigger, schedules }: iProps) {

    const [editing, setEditing] = useState<boolean>(false);
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iClientFollowUp>({
        clientId: SYSTEM.EMPTY_GUID,
        scheduleId: null,
        observation: '',
        clientFollowUpStatus: '',
        imagesFormFile: [],
        imagesBase64: []
    });

    const [schedulesOptions, setSchedulesOptions] = useState<iDropdownOption<Guid>[]>([]);

    useEffect(() => {
        if (schedules) {
            const schedulesWithFormattedDate = schedules.map(schedule => ({
                ...schedule,
                dateStart: handleFormatDate(schedule.dateStart, DATE_STYLE.DETALHADO)
            }));

            const schedulesOptions = handleTransformArrayToDropdownOptionsGuid(schedulesWithFormattedDate ?? [], 'scheduleId', ['dateStart', 'customTitle']);
            setSchedulesOptions(schedulesOptions ?? []);
        }
    }, [schedules]);

    const setClientFollowUpStatusOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.clientFollowUpStatus ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;
    const setScheduleIdOption = handleSetDropdownOption(formData, setFormData, handleGetPropName(formData, x => x.scheduleId ?? '')[1]) as Dispatch<SetStateAction<iDropdownOption[]>>;

    const handleClose = useCallback(() => {
        setSaving(false);
        setEditing(false);
        setIsModalOpen(false);
        handleClearFormData(setFormData);
    }, [setIsModalOpen]);

    useEffect(() => {
        handleClearFormData(setFormData);
        setSaving(false);
        setEditing(true);

        if (!isModalOpen || !clientId || !clientFollowUpStatusEnum || !clientFollowUpStatusEnum?.length) {
            return;
        }

        if (type === 'edit') {
            if (!followUpClicked) {
                return;
            }
        }

        const clientFollowUpStatusInProgress = clientFollowUpStatusEnum?.find(x => x.value.toString() === '1');

        setFormData({
            clientFollowUpId: followUpClicked ? followUpClicked.clientFollowUpId : SYSTEM.EMPTY_GUID,
            clientId: clientId,
            scheduleId: followUpClicked ? followUpClicked.scheduleId : null,
            observation: followUpClicked ? followUpClicked.observation : '',
            clientFollowUpStatus: followUpClicked ? followUpClicked.clientFollowUpStatus : clientFollowUpStatusInProgress?.value?.toString(),
            imagesBase64: followUpClicked?.imagesBase64?.length ? followUpClicked?.imagesBase64 : [],
            imagesFormFile: followUpClicked?.imagesBase64?.length ?
                handleConvertBase64ListToFiles(
                    followUpClicked.imagesBase64.map(img => ({
                        base64: img,
                        filename: 'img',
                    }))
                ) : [],
        });
    }, [isModalOpen, type, clientId, followUpClicked, clientFollowUpStatusEnum, setIsModalOpen, handleClose]);

    async function handleSave() {
        if (!formData.clientId || !formData.clientFollowUpStatus) {
            swal({ content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS, icon: 'warning' });
            return;
        }

        if (!formData.observation && !formData.imagesFormFile) {
            swal({ content: 'Adicione pelo menos a observação ou um anexo.', icon: 'warning' });
            return;
        }

        setEditing(false);
        setSaving(true);

        let hasImages = false;
        const data = handleLoopFormData(formData);
        const input = data.json as iClientFollowUp;
        const formDataInput = new FormData();

        formDataInput.append('ClientFollowUpId', input.clientFollowUpId ? input.clientFollowUpId.toString() : SYSTEM.EMPTY_GUID.toString());
        formDataInput.append('ClientId', clientId!.toString());
        formDataInput.append('ScheduleId', input.scheduleId ? input.scheduleId.toString() : SYSTEM.EMPTY_GUID.toString());
        formDataInput.append('Observation', input.observation ?? '');
        formDataInput.append('ClientFollowUpStatus', input.clientFollowUpStatus ?? '');

        if (input.imagesFormFile && (Array.isArray(input.imagesFormFile) && input.imagesFormFile.every(f => f instanceof File))) {
            for (const file of input.imagesFormFile) {
                hasImages = true;
                formDataInput.append('ImagesFormFile', file, file.name);
            }
        }

        if (type === 'create') {
            const output = await Fetch.post({ url: CONSTS_CLIENT_FOLLOW_UP.post, body: formDataInput, isFormData: true });

            if (output) {
                swal({
                    content: 'Acompanhamento registrado com sucesso.',
                    confirmFunction: () => {
                        setTrigger(new Date());
                        swalLoading({ handleFunction: () => handleClose(), timeoutMs: (hasImages ? 3500 : 1000) });
                    },
                    icon: 'success'
                });
                return;
            }

            setEditing(true);
            setSaving(false);
            return;
        }

        const output = await Fetch.put({ url: CONSTS_CLIENT_FOLLOW_UP.put, body: formDataInput, isFormData: true });

        if (output) {
            swal({
                content: 'Acompanhamento atualizado com sucesso.',
                confirmFunction: () => {
                    setTrigger(new Date());
                    swalLoading({ handleFunction: () => handleClose(), timeoutMs: (hasImages ? 3500 : 1000) });
                },
                icon: 'success'
            });

            return;
        }

        setEditing(true);
        setSaving(false);
        return;
    }

    if (!isModalOpen) {
        return;
    }

    return (
        <ModalGeneric
            isOpen={isModalOpen}
            setIsModalOpen={setIsModalOpen}
            onRequestClose={handleClose}
            showCloseButton={false}
            allowCloseOutsideClick={false}
            style={{ padding: 0, width: '50rem', maxWidth: '90%' }}
        >
            <div className={styles.modalCard}>
                <header className={styles.modalHeader}>
                    <div className={styles.modalHeaderLeft}>
                        <h1 className={styles.inputTitle}>
                            {type === 'create' ? 'Cadastrar novo acompanhamento' : 'Editar acompanhamento'}
                        </h1>
                    </div>

                    <div className={styles.modalHeaderRight}>
                        <div className={styles.metaRow}>
                            <TagList
                                tags={[
                                    { label: '✖', color: 'transparent', handleFunction: () => handleClose(), title: 'Fechar' }
                                ]}
                            />
                        </div>
                    </div>
                </header>

                <main className={styles.modalContent}>
                    <div className='modal-layout-flex'>
                        <div className={styles.div}>
                            <label>Observações do acompanhamento</label>
                            <textarea value={formData.observation ?? ''} className={styles.textarea} readOnly={!editing} rows={5} maxLength={512} onChange={(e) => setFormData((prev: typeof formData) => ({ ...prev, observation: e.target.value }))} />
                        </div>

                        {
                            type === 'create' ? (
                                <Dropdown title='Status do acompanhamento' options={clientFollowUpStatusEnum ?? []} selectedOption={clientFollowUpStatusEnum?.find(x => x.value.toString() === '1')} setSelectedOption={setClientFollowUpStatusOption} isDisabled={true} isObligatory={true} />
                            ) : (
                                <Dropdown title='Status do acompanhamento' options={clientFollowUpStatusEnum ?? []} selectedOption={clientFollowUpStatusEnum?.find(x => x.value.toString() === formData?.clientFollowUpStatus?.toString())} setSelectedOption={setClientFollowUpStatusOption} isDisabled={!editing} isObligatory={true} />
                            )
                        }

                        <Dropdown title='Este acompanhamento faz parte do <b>agendamento</b> ocorrido em...' options={schedulesOptions ?? []} selectedOption={schedulesOptions?.find(x => x.value.toString() === formData?.scheduleId?.toString())} setSelectedOption={setScheduleIdOption} isDisabled={!editing} />
                        <InputImage title='Anexos' fieldName='imagesFormFile' formData={formData} setFormData={setFormData} isDisabled={!editing} placeholder='Selecionar anexos' isMultiple={true} />
                    </div>
                </main>

                <ModalGenericFooter
                    type={type}
                    saving={saving}
                    editing={editing}
                    handleClose={handleClose}
                    handleSave={handleSave}
                    setEditing={setEditing}
                />
            </div>
        </ModalGeneric>
    )
}