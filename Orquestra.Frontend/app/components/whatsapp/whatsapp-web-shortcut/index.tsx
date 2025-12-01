import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import ROUTES from '@/app/consts/routes';
import handleIsMobile from '@/app/functions/check.isMobile';
import { handleBuildWaMeUrl, handleBuildWhatsappWebUrl, handleIsValidE164, handleNormalizePhoneToE164 } from '@/app/functions/normalize.phone';
import toast from '@/app/functions/toast';
import { Guid } from 'guid-typescript';
import { useRouter } from 'next/navigation';
import { MouseEvent, useMemo } from 'react';
import styles from './index.module.scss';

export interface iProps {
    phone?: string;
    message?: string;
    label?: string;
    clientId?: Guid;
}

export default function WhatsappWebShortcut({ phone, message = '', label = 'Abrir no WhatsApp', clientId }: iProps) {

    const router = useRouter();

    const { phoneNormalized, isValidPhone } = useMemo(() => {
        const normalized = handleNormalizePhoneToE164(phone);
        const valid = handleIsValidE164(normalized);
        // console.log(phone, phoneNormalized, isValid);

        return { phoneNormalized: normalized, isValidPhone: valid };
    }, [phone]);

    function handleClick(event: MouseEvent<HTMLButtonElement>) {
        event.preventDefault();

        if (!phoneNormalized || !isValidPhone) {
            toast({ content: 'Número de telefone inválido.' });
            return;
        }

        if (handleIsMobile()) {
            window.location.href = `whatsapp://send?phone=${phoneNormalized}&text=${encodeURIComponent(message)}`;

            setTimeout(() => {
                window.location.href = handleBuildWaMeUrl(phoneNormalized, message);
            }, 700);

            return;
        }

        window.open(handleBuildWhatsappWebUrl(phoneNormalized, message), '_blank', 'noopener,noreferrer');
    }

    return (
        <div className={styles.container}>
            {
                isValidPhone ? (
                    <Button
                        label={label}
                        handleFunction={handleClick}
                        icon_feather={<Icon icon='message-square' />}
                    />
                ) : (
                    clientId && (
                        <Button
                            label='Atualizar telefone'
                            handleFunction={() => router.push(`${ROUTES.EMPRESA_CLIENTES}/${clientId}`)}
                            isStyleSimple={true}
                            icon_feather={<Icon icon='phone' />}
                        />
                    )
                )
            }
        </div>
    )
}