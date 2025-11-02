'use client';
import { iMe } from '@/app/api/consts/auth';
import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import InputMask from '@/app/components/input/text';
import SYSTEM from '@/app/consts/system';
import swal from '@/app/functions/swal';
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

    const [formData, setFormData] = useState({
        userName: me?.userName,
        email: me?.email
    });

    async function handleSave() {
        if (!formData.userName || !formData.email) {
            swal({
                content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS,
                icon: 'error'
            });

            return;
        }

        alert('Informações pessoais salvas com sucesso!');
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
                        <InputMask title='Nome completo' fieldName='userName' formData={formData} setFormData={setFormData} isObligatory={true} />
                    </div>

                    <div>
                        <InputMask title='E-mail' fieldName='email' formData={formData} setFormData={setFormData} isObligatory={true} isDisabled={true} />
                    </div>
                </div>

                <div className={styles.buttonGroup}>
                    <Button label='Salvar alterações' handleFunction={handleSave} />
                </div>
            </div>
        </div>
    )
}

function PasswordForm() {

    const [formData, setFormData] = useState({
        newPassword: '',
        newPasswordConfirmation: ''
    });

    async function handleSave() {
        if (!formData.newPassword || !formData.newPasswordConfirmation) {
            swal({
                content: SYSTEM.WARN_FILL_OBLIGATORY_FIELDS,
                icon: 'error'
            });

            return;
        }

        if (formData.newPassword !== formData.newPasswordConfirmation) {
            swal({
                content: 'As senhas não coincidem.',
                icon: 'error'
            });

            return;
        }

        alert('Senha alterada com sucesso!');
        setFormData({ newPassword: '', newPasswordConfirmation: '' });
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
                        <InputMask title='Nova senha' type='password' fieldName='newPassword' formData={formData} setFormData={setFormData} isObligatory={true} />
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
                    />
                </div>
            </div>
        </div>
    )
}