'use client';
import { iMe } from '@/app/api/consts/auth';
import { CONSTS_USER, iUserInput } from '@/app/api/consts/user';
import { Fetch } from '@/app/api/fetch';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import ROUTES from '@/app/consts/routes';
import SYSTEM from '@/app/consts/system';
import swal from '@/app/functions/swal';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import styles from './index.module.scss';

interface iProps {
    me: iMe;
}

export default function UsuarioConfiguracoesTabConta({ me }: iProps) {
    return (
        <div className={styles.container}>
            <div className={styles.wrapper}>
                <PersonalInfoForm me={me} />
                <PasswordForm />
            </div>
        </div>
    )
}

function PersonalInfoForm({ me }: iProps) {

    const router = useRouter();
    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iUserInput>({
        fullName: me?.userName,
        email: me?.email
    });

    async function handleSave() {
        if (!formData.fullName) {
            swal({
                content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS,
                icon: 'error'
            });

            return;
        }

        swal({
            content: 'Ao salvar as alterações, sua sessão ficará inválida e você precisará fazer login novamente.',
            confirmBtnText: 'Continuar',
            confirmFunction: async () => {
                setSaving(true);

                const input = {
                    fullName: formData.fullName
                };

                const output = await Fetch.put({ url: CONSTS_USER.put, body: input });

                if (output) {
                    swal({
                        content: 'Informações pessoais salvas com sucesso.',
                        confirmFunction: () => router.push(ROUTES.LOGOUT),
                        allowOutsideClick: false,
                        icon: 'success'
                    });

                    setSaving(false);
                    return;
                }

                setSaving(false);
                return;
            },
            cancelBtnText: 'Voltar',
            icon: 'info'
        });
    }

    return (
        <div className={styles.card}>
            <div className={styles.cardHeader}>
                <h2 className={styles.cardTitle}>Informações pessoais</h2>
                <p className={styles.cardSubtitle}>
                    Atualize suas informações pessoais.
                </p>
            </div>

            <div className={styles.form}>
                <div className={styles.formRow}>
                    <div>
                        <InputMask title='Nome completo' fieldName='fullName' formData={formData} setFormData={setFormData} isObligatory={true} />
                    </div>

                    <div>
                        <InputMask title='E-mail' fieldName='email' formData={formData} setFormData={setFormData} isObligatory={true} isDisabled={true} />
                    </div>
                </div>

                <div className={styles.buttonGroup}>
                    <Button label='Salvar alterações' handleFunction={handleSave} isDisabled={saving} />
                </div>
            </div>
        </div>
    )
}

function PasswordForm() {

    const [saving, setSaving] = useState<boolean>(false);

    const [formData, setFormData] = useState<iUserInput>({
        password: '',
        newPasswordConfirmation: ''
    });

    async function handleSave() {
        if (!formData.password || !formData.newPasswordConfirmation) {
            swal({
                content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS,
                icon: 'error'
            });

            return;
        }

        if (formData.password !== formData.newPasswordConfirmation) {
            swal({
                content: 'As senhas não coincidem.',
                icon: 'error'
            });

            return;
        }

        swal({
            content: 'Deseja prosseguir com a alteração de sua senha?',
            confirmBtnText: 'Continuar',
            confirmFunction: async () => {
                setSaving(true);

                const input = {
                    password: formData.password
                };

                const output = await Fetch.put({ url: CONSTS_USER.put, body: input });
                setFormData({ password: '', newPasswordConfirmation: '' });

                if (output) {
                    swal({
                        content: 'Senha atualizada com sucesso.',
                        icon: 'success'
                    });

                    setSaving(false);
                    return;
                }

                setSaving(false);
                return;
            },
            cancelBtnText: 'Voltar',
            icon: 'info'
        });
    }

    return (
        <div className={styles.card}>
            <div className={styles.cardHeader}>
                <h2 className={styles.cardTitle}>Alterar senha</h2>
                <p className={styles.cardSubtitle}>
                    Para sua segurança, escolha uma senha forte.
                </p>
            </div>

            <div className={styles.form}>
                <div className={styles.formRow}>
                    <div>
                        <InputMask title='Nova senha' type='password' fieldName='password' formData={formData} setFormData={setFormData} isObligatory={true} />
                    </div>

                    <div>
                        <InputMask title='Confirme sua nova senha' type='password' fieldName='newPasswordConfirmation' formData={formData} setFormData={setFormData} isObligatory={true} />
                    </div>
                </div>

                <div className={styles.buttonGroup}>
                    <Button
                        label='Salvar nova senha'
                        handleFunction={handleSave}
                        icon_feather={<Icon icon='lock' size='small' />}
                        isDisabled={saving}
                    />
                </div>
            </div>
        </div>
    )
}