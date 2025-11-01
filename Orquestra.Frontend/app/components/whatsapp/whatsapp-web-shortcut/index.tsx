import Icon from '@/app/components/icon';
import Button from '@/app/components/input/button';
import handleIsMobile from '@/app/functions/check.isMobile';
import { handleBuildWaMeUrl, handleBuildWhatsappWebUrl, handleNormalizePhoneToE164 } from '@/app/functions/normalize.phone';
import toast from '@/app/functions/toast';
import { MouseEvent } from 'react';
import styles from './index.module.scss';

export interface iProps {
    phone?: string;
    message?: string;
    label?: string;
}

export default function WhatsappWebShortcut({ phone, message = '', label = 'Abrir no WhatsApp' }: iProps) {

    function handleClick(event: MouseEvent<HTMLButtonElement>) {
        event.preventDefault();
        const digits = handleNormalizePhoneToE164(phone);

        if (!digits) {
            toast({ content: 'Número de telefone inválido.' });
            return;
        }

        if (handleIsMobile()) {
            window.location.href = `whatsapp://send?phone=${digits}&text=${encodeURIComponent(message)}`;

            setTimeout(() => {
                window.location.href = handleBuildWaMeUrl(digits, message);
            }, 700);

            return;
        }

        window.open(handleBuildWhatsappWebUrl(digits, message), '_blank', 'noopener,noreferrer');
    }

    return (
        <div className={styles.container}>
            <Button label={label} handleFunction={handleClick} icon_feather={<Icon icon='message-square' />} />
        </div>
    )
}