import React from 'react';
import styles from './index.module.scss';

export interface iProps {
    phone?: string;
    message?: string;
    label?: string;
}

function normalizeToE164(raw?: string) {
    if (!raw) return '';
    let digits = raw.replace(/\D+/g, '');
    if (!digits.startsWith('55')) digits = '55' + digits;
    return digits;
}

function isMobile() {
    if (typeof navigator === 'undefined') return false;
    return /android|iphone|ipad|ipod|windows phone/i.test(navigator.userAgent || '');
}

function buildWhatsappWebUrl(phoneDigits: string, text: string) {
    return `https://web.whatsapp.com/send?phone=${phoneDigits}&text=${encodeURIComponent(text || '')}`;
}

function buildWaMeUrl(phoneDigits: string, text: string) {
    return `https://wa.me/${phoneDigits}?text=${encodeURIComponent(text || '')}`;
}

export default function WhatsappWebShortcut({ phone, message = '', label = 'Abrir WhatsApp' }: iProps) {
    const handleClick = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        const digits = normalizeToE164(phone);
        if (!digits) return alert('Número de telefone inválido');

        if (isMobile()) {
            window.location.href = `whatsapp://send?phone=${digits}&text=${encodeURIComponent(message)}`;
            setTimeout(() => {
                window.location.href = buildWaMeUrl(digits, message);
            }, 700);
            return;
        }

        window.open(buildWhatsappWebUrl(digits, message), '_blank', 'noopener,noreferrer');
    };

    const handleCopyLink = (event: React.MouseEvent<HTMLButtonElement>) => {
        event.preventDefault();
        const url = buildWaMeUrl(normalizeToE164(phone), message);
        if (navigator.clipboard) {
            navigator.clipboard.writeText(url).then(() => alert('Link copiado pro clipboard'));
        } else {
            const tmp = document.createElement('textarea');
            tmp.value = url;
            document.body.appendChild(tmp);
            tmp.select();
            try { document.execCommand('copy'); alert('Link copiado pro clipboard'); } catch { alert('Não foi possível copiar o link'); }
            document.body.removeChild(tmp);
        }
    };

    return (
        <div className={styles.container}>
            <button type="button" className={styles.openBtn} onClick={handleClick}>{label}</button>
            <button type="button" className={styles.copyBtn} onClick={handleCopyLink}>Copiar link</button>
        </div>
    )
}